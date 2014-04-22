using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trendingBot2
{
    /// <summary>
    /// Class storing all the code dealing with the analysis of the generated fits, that is: determining whether they are good enough or not and calculating the corresponding ratings
    /// </summary>
    public class Analysis
    {
        //The original combination ("Combination" class), generated in the combinatorics part, requires from quite a few actions to become a valid combination, that is:
        //a dependent variable in one of the output fits. This method performs this conversion
        public ValidCombination createValidComb(Combination curCombination, List<Input> inputs, CombValues xVals, CombValues yVals, Config curConfig, bool checkModify)
        {
            ValidCombination curValidComb = createValidComb2(curCombination, inputs, xVals, yVals, curConfig);

            if (curValidComb != null)
            {
                if(checkModify) curValidComb = modifyFit(curValidComb, inputs, yVals, curConfig);

                if(curValidComb != null) curValidComb = assessComb(curValidComb, curConfig);
            }

            return curValidComb;
        }

        //Method in charge of performing the initial actions to convert a "Combination" variable into a ValidCombination one, that is: regression and first crosscheck of the
        //calculated values against the applicable error thresholds
        private ValidCombination createValidComb2(Combination curCombination, List<Input> inputs, CombValues xVals, CombValues yVals, Config curConfig)
        {
            ValidCombination curValidComb = new ValidCombination();

            //Performing the corresponding regression to determine the polynomial fit matching the input conditions
            CurveFitting curFitting = new CurveFitting();
            PolCurve curCurve = curFitting.performPolRegress(xVals, yVals);

            List<double> errors = new List<double>();
            curValidComb.dependentVars.items = new List<CombinationItem>(curCombination.items);
            curValidComb.independentVar = curCurve.yValues.combination.items[0].variable;
            curValidComb.coeffs = curCurve.coeffs;

            //Loop applying the newly-created fit to all the input values and calculating the associated variables
            for (int i = 0; i < xVals.values.Count; i++)
            {
                double realVal = yVals.values[i].value;
                curValidComb.realVals.Add(realVal);
                RowVal curCalcVal = new RowVal();
                curCalcVal.value = Common.valueFromPol(curCurve.coeffs, xVals.values[i].value);
                curCalcVal.weights = xVals.values[i].weights;
                curCalcVal.weights2 = xVals.values[i].weights2;
                curValidComb.calcVals.Add(curCalcVal);
                double curError = Common.errorCalcs(realVal, curCalcVal.value);
                curValidComb.errors.Add(curError);
                if (curError <= curConfig.fitConfig.averLimit) curValidComb.lowErrorCount = curValidComb.lowErrorCount + 1;
            }
            curValidComb.averError = curValidComb.errors.Average(x => x);

            bool isOK = false;
            if (curValidComb.averError <= curConfig.fitConfig.globalAver)
            {
                if ((double)curValidComb.lowErrorCount / (double)curValidComb.errors.Count >= curConfig.fitConfig.minPercBelowLimit)
                {
                    isOK = true;
                }
            }

            if (!isOK) curValidComb = null;

            return curValidComb;
        }

        //After a new ValidCombination has been created (and thus the preliminary thresholds have been met), further analysis are required to determine whether it is actually right 
        private ValidCombination modifyFit(ValidCombination curValidComb, List<Input> inputs, CombValues yVals, Config curConfig)
        {
            if (fitIsConstant(curValidComb, curConfig))
            {
                //y = A represents accurately the current fit; no further information needs to be accounted for
                if (curValidComb.averError >= curConfig.limitVals.maxErrorConstant)
                {
                    //Having a too big error for a constant fit might indicate an "understanding error". Example: 10001, 9999, 10000... modelled with  y = 10000
                    curValidComb = null;
                }
                else
                {
                    curValidComb.dependentVars = new Combination();
                    curValidComb.coeffs.B = 0;
                    curValidComb.coeffs.C = 0;
                }
            }
            else
            {
                //The current fit is certainly not constant, but some of the variables might not be required. Example: y = x1 + 0.00000000000001 * x2
                curValidComb = removeRedundantVars(curValidComb, inputs, yVals, curConfig);
            }

            return curValidComb;
        }

        //Method determining whether the current fit (as stored in a ValidCombination variable) can be considered as constant (i.e., y = A)
        private bool fitIsConstant(ValidCombination curValidComb, Config curConfig)
        {
            bool fitIsConstant = false;
            if (Math.Abs(curValidComb.coeffs.B) <= curConfig.limitVals.valueIsZero && Math.Abs(curValidComb.coeffs.C) <= curConfig.limitVals.valueIsZero)
            {
                fitIsConstant = true;
            }
            else
            {
                double averError = 0;
                foreach (var val in curValidComb.realVals)
                {
                    averError = averError + 100 * Math.Abs((val - curValidComb.coeffs.A) / curValidComb.coeffs.A);
                }

                if (averError <= 0.01)
                {
                    fitIsConstant = true;
                }
            }

            return fitIsConstant;
        }

        //Method determining whether all the variables forming the corresponding combinations are actually required
        private ValidCombination removeRedundantVars(ValidCombination curValidComb, List<Input> inputs, CombValues yVals, Config curConfig)
        {
            if (curValidComb.dependentVars.items.Count > 1)
            {
                if (curValidComb.calcVals.FirstOrDefault(x => x.weights2.Count > 0) != null)
                {
                    //Removal of redundant groups of variables (i.e., products of 2 or more variables)
                    curValidComb = removeMultiRedundant(curValidComb, inputs, yVals, curConfig);
                }
                if(curValidComb != null) curValidComb = removeSingleRedundant(curValidComb, inputs, yVals, curConfig);
            }

            return curValidComb;
        }

        //Looking at the weights2 variable to determine whether there are group of factors (e.g., var1*var2) which should better be removed
        private ValidCombination removeMultiRedundant(ValidCombination curValidComb, List<Input> inputs, CombValues yVals, Config curConfig)
        {
            List<Weight2> allWeights2 = new List<Weight2>();
            for (int i = 0; i < curValidComb.calcVals[0].weights2.Count; i++)
            {
                //i2 avoids the warning "Using the iteration variable in a lambda expression is..."
                //which, curiously, is only shown in VB.NET (even with Option Strict Off!), but not in C# (even though this "problem" is common to both)
                int i2 = i;
                Weight2 curWeight2 = new Weight2();
                curWeight2.combWeight = curValidComb.calcVals.Average(x => x.weights2[i2].combWeight);
                curWeight2.combItems = new List<CombinationItem>(curValidComb.calcVals[0].weights2[i].combItems);
                allWeights2.Add(curWeight2);
            }

            bool goAhead = allWeights2.FirstOrDefault(x => x.combWeight >= curConfig.limitVals.tooRelevantWeight) != null;
            goAhead = goAhead ? goAhead : allWeights2.FirstOrDefault(x => x.combWeight <= 0.5 * curConfig.limitVals.maxErrorToIgnoreVar) != null;
            if (goAhead)
            {
                //One of the variables contributes in a much more relevant way (to the final calculated value) than all the other ones in the combination
                List<int> itemsToIgnore = new List<int>();
                for (int i = 0; i < allWeights2.Count; i++)
                {
                    if (allWeights2[i].combWeight < curConfig.limitVals.tooRelevantWeight)
                    {
                        //The current variable might be irrelevant. The analysis performed in the following lines consists basically in removing this variable and comparing the resulting errors (with vs. without it)
                        Combination tempDependent = new Combination();
                        tempDependent.items = curValidComb.dependentVars.items.Except(allWeights2[i].combItems).ToList();

                        if (ignoreItem(tempDependent, curValidComb, inputs, yVals, curConfig))
                        {
                            itemsToIgnore.AddRange(allWeights2[i].combItems.Select(x => x.variable.index));
                        }
                    }
                }

                if (itemsToIgnore.Count > 0)
                {
                    curValidComb = validCombWithIgnoredItems(curValidComb, itemsToIgnore, inputs, yVals, curConfig, true);
                }
            }

            return curValidComb;
        }

        //This function is part of the redundant-variable-removal process: after the given variable (or group of variables) has been (temporarily) removed, this
        //function checks whether the resulting combination is still valid or not (and thus the removal process can go further)
        private bool ignoreItem(Combination tempDependent, ValidCombination curValidComb, List<Input> inputs, CombValues yVals, Config curConfig)
        {
            bool ignoreIt = false;

            CombValues xVals = Common.getCombinationListVals(tempDependent, inputs);
            ValidCombination tempValid = createValidComb(tempDependent, inputs, xVals, yVals, curConfig, false);
            if (tempValid != null)
            {
                ignoreIt = tempValid.averError < curValidComb.averError || (tempValid.averError <= curConfig.limitVals.maxErrorToIgnoreVar && curValidComb.averError <= curConfig.limitVals.maxErrorToIgnoreVar);
                ignoreIt = ignoreIt ? ignoreIt : Common.valsAreEquivalent(tempValid.averError, curValidComb.averError, curConfig.limitVals.similarity.medium);
            }

            return ignoreIt;
        }

        //This function forms also part of the redundant-variable-removal process: the removal of the given variable (or group of variables) has been confirmed to be fine and thus
        //this function will update the corresponding ValidCombination variable
        private ValidCombination validCombWithIgnoredItems(ValidCombination curValidComb, List<int> itemsToIgnore, List<Input> inputs, CombValues yVals, Config curConfig, bool isMulti)
        {
            //At least one of the variables in the original combination is redundant and thus the corresponding ValidCombination variable has to be re-calculated
            for (int i = curValidComb.dependentVars.items.Count - 1; i >= 0; i--)
            {
                if (itemsToIgnore.Contains(i)) curValidComb.dependentVars.items.RemoveAt(i);
            }

            if (curValidComb.dependentVars.items.Count < 1)
            {
                curValidComb = null;
            }
            else
            {
                CombValues xVals = Common.getCombinationListVals(curValidComb.dependentVars, inputs);
                curValidComb = createValidComb(curValidComb.dependentVars, inputs, xVals, yVals, curConfig, !isMulti); //Rechecking for redundant variables only after single removals to avoid potential infinite loops
            }

            return curValidComb;
        }

        //Looking at the weights variable to determine whether there are single variables (e.g., var1 + var2) which should better be removed
        private ValidCombination removeSingleRedundant(ValidCombination curValidComb, List<Input> inputs, CombValues yVals, Config curConfig)
        {
            //Addition of the weights of all the variables in each row. This is a simplistic way to determine whether one of the variables
            //forming the given combination is much more relevant than all the other ones
            List<double> allWeights = new List<double>();
            for (int i = 0; i < curValidComb.dependentVars.items.Count; i++)
            {
                //i2 avoids the warning "Using the iteration variable in a lambda expression is..."
                //which, curiously, is only shown in VB.NET (even with Option Strict Off!), but not in C# (even though this "problem" is common to both)
                int i2 = i;
                allWeights.Add(curValidComb.calcVals.Average(x => x.weights[i2]));
            }

            double curMaxWeight = curConfig.limitVals.tooRelevantWeight;
            bool goAhead = allWeights.FirstOrDefault(x => x >= curConfig.limitVals.tooRelevantWeight) >= curConfig.limitVals.tooRelevantWeight;
            goAhead = goAhead ? goAhead : allWeights.FirstOrDefault(x => x <= curConfig.limitVals.maxErrorToIgnoreVar) <= curConfig.limitVals.maxErrorToIgnoreVar;
            if (!goAhead && curValidComb.dependentVars.items.Count > 2)
            {
                curMaxWeight = curConfig.limitVals.tooRelevantWeight2;
                goAhead = allWeights.FirstOrDefault(x => x >= curConfig.limitVals.tooRelevantWeight2) >= curConfig.limitVals.tooRelevantWeight2;
            }

            if (goAhead)
            {
                //One of the variables contributes in a much more relevant way (to the final calculated value) than all the other ones in the combination
                List<int> itemsToIgnore = new List<int>();
                for (int i = curValidComb.dependentVars.items.Count - 1; i >= 0; i--)
                {
                    if (allWeights[i] < curMaxWeight)
                    {
                        bool canBeRemoved = (i == curValidComb.dependentVars.items.Count - 1 || curValidComb.dependentVars.items[i].operation != Operation.Multiplication);
                        canBeRemoved = canBeRemoved ? (i == 0 || curValidComb.dependentVars.items[i - 1].operation != Operation.Multiplication) :  false;
                        if (canBeRemoved)
                        {
                            //The current variable might be irrelevant. The analysis performed in the following lines consists basically in removing this variable and comparing the resulting errors (with vs. without)
                            Combination tempDependent = new Combination();
                            tempDependent.items = new List<CombinationItem>(curValidComb.dependentVars.items);
                            tempDependent.items.RemoveAt(i);

                            if (ignoreItem(tempDependent, curValidComb, inputs, yVals, curConfig))
                            {
                                itemsToIgnore.Add(i);
                            }
                        }
                    }
                }

                if (itemsToIgnore.Count > 0)
                {
                    curValidComb = validCombWithIgnoredItems(curValidComb, itemsToIgnore, inputs, yVals, curConfig, false);
                }
            }

            return curValidComb;
        }

        //After a ValidCombination has been formed, it is necessary to assess its suitability/rating; this is what this method takes care of. 
        private ValidCombination assessComb(ValidCombination curValidComb, Config curConfig)
        {
            curValidComb.assessment = new Assessment();

            //The assessment of the given fit results from different factors, which account for different aspects of the input data, the predicted results, etc.

            //In-sample accuracy
            int curCount = 0;
            curValidComb.assessment = addFactor(curCount, curValidComb.assessment, curValidComb, curConfig);
            if (curValidComb.assessment.globalRating == 10.0)
            {
                //Perfect situation
                curValidComb.averError = 0.0;
                return curValidComb;
            }

            //Quality of dependent variable(s)
            curCount = curCount + 1;
            curValidComb.assessment = addFactor(curCount, curValidComb.assessment, curValidComb, curConfig);

            //Quality of independent variable
            curCount = curCount + 1;
            curValidComb.assessment = addFactor(curCount, curValidComb.assessment, curValidComb, curConfig);

            //Tunability
            curCount = curCount + 1;
            curValidComb.assessment = addFactor(curCount, curValidComb.assessment, curValidComb, curConfig);

            //Complexity of polynomial fit
            curCount = curCount + 1;
            curValidComb.assessment = addFactor(curCount, curValidComb.assessment, curValidComb, curConfig);

            foreach (AssessmentFactor factor in curValidComb.assessment.factors)
            {
                factor.weight = factor.weight * 100 / curValidComb.assessment.totWeight; //Converting the partial weights into a 0-100 scale
                curValidComb.assessment.globalRating = curValidComb.assessment.globalRating + (factor.rating * factor.weight / 100);
            }

            return curValidComb;
        }

        //Method called to add the assessing factors to the corresponding Assessment variable
        private Assessment addFactor(int curCount, Assessment curAssessment, ValidCombination curValidComb, Config curConfig)
        {
            AssessmentFactor curFactor = new AssessmentFactor();

            if (curCount == 0)
            {
                curFactor.name = "In-sample accuracy";
                curFactor.weight = 10.0;
                if (curValidComb.averError <= curConfig.limitVals.valueIsZero) //Zero cannot be considered because the (floating-point) double type might provoke errors
                {
                    //Too perfect to continue
                    curFactor.rating = 10.0;
                    curAssessment.factors.Add(curFactor);
                    curAssessment.totWeight = 10.0;
                    curAssessment.globalRating = 10.0;
                    return curAssessment;
                }

                if (curValidComb.averError <= curConfig.fitConfig.globalAver * 0.05)
                {
                    double curMinPercBelow = (double)curValidComb.lowErrorCount / (double)curValidComb.errors.Count;
                    if (curMinPercBelow >= 0.99) curFactor.rating = 9.0;
                    else if (curMinPercBelow >= 0.95) curFactor.rating = 8.0;
                }
                else
                {
                    curFactor.rating = 8.0 * (curConfig.fitConfig.globalAver - curValidComb.averError) / (0.95 * curConfig.fitConfig.globalAver);
                }
            }
            else if (curCount == 1 || curCount == 2)
            {
                curFactor.name = "Quality of independent variable";
                double averRatingVars = curValidComb.independentVar.input.preAnalysis.rating;
                if (curCount == 1)
                {
                    curFactor.name = "Quality of dependent variable(s)";
                    averRatingVars = curValidComb.dependentVars.items.Count == 0 ? 10 : curValidComb.dependentVars.items.Average(x => x.variable.input.preAnalysis.rating);
                }
                curFactor.weight = 8.0;
                if (curValidComb.errors.Count >= curConfig.fitConfig.minNoCases)
                {
                    curFactor.rating = 8.0;
                    if (curValidComb.errors.Count >= 1.25 * curConfig.fitConfig.minNoCases) curFactor.rating = 10.0;
                    else if (curValidComb.errors.Count >= 1.1 * curConfig.fitConfig.minNoCases) curFactor.rating = 9.0;
                }
                curFactor.rating = Math.Round(0.5 * (double)curFactor.rating + 0.5 * averRatingVars, 0);
            }
            else if (curCount == 3)
            {
                curFactor.name = "Solution tunability";
                curFactor.weight = 6.0;
                curFactor.rating = ratingTunability(curValidComb);
            }
            else if (curCount == 4)
            {
                curFactor.name = "Complexity of polynomial fit";
                curFactor.weight = 5.0;
                curFactor.rating = ratingFitComplexity(curValidComb);
            }

            if (curFactor.rating < 0.0) curFactor.rating = 0.0;
            curAssessment.totWeight = curAssessment.totWeight + curFactor.weight;
            curAssessment.factors.Add(curFactor);

            return curAssessment;
        }

        //Function taking care of the determination of the "Solution tunability" factor
        private double ratingTunability(ValidCombination curValidComb)
        {
            double outRating = 10.0;

            double valsAllVars = 0.0;
            bool allFine = true;

            foreach (var comb in curValidComb.dependentVars.items)
            {
                if (comb.variable.input.totDiffVals < curValidComb.independentVar.input.totDiffVals) allFine = false;
                valsAllVars = valsAllVars * (double)comb.variable.input.totDiffVals;
            }

            if (allFine)
            {
                if (curValidComb.dependentVars.items.Count == 1)
                {
                    outRating = 9.0;
                }
            }
            else
            {
                if (valsAllVars >= 3.0 * (double)curValidComb.independentVar.input.totDiffVals)
                {
                    outRating = 9.0;
                }
                else if (valsAllVars >= 1.5 * (double)curValidComb.independentVar.input.totDiffVals)
                {
                    outRating = 8.0;
                }
                else if (valsAllVars >= 0.75 * (double)curValidComb.independentVar.input.totDiffVals)
                {
                    outRating = 7.0;
                }
                else
                {
                    outRating = 6.0;
                    if (curValidComb.dependentVars.items.Count == 1)
                    {
                        outRating = 5.0;
                        if (valsAllVars >= 0.3 * (double)curValidComb.independentVar.input.totDiffVals)
                        {
                            outRating = 4.0;
                        }
                    }
                }
            }

            return outRating;
        }

        //Function taking care of the determination of the "Complexity of polynomial fit" factor
        private double ratingFitComplexity(ValidCombination curValidComb)
        {
            //Accounting for situations when the C coeff is different than zero but its "effect is compensated" (e.g., (var^0.5)^2 = var)
            bool noRealC = true;
            if ((curValidComb.dependentVars.items.Count > 1 && curValidComb.dependentVars.items.FirstOrDefault(x => x.operation != Operation.Multiplication) != null) || curValidComb.dependentVars.items.FirstOrDefault(x => x.exponent != 0.5) != null)
            {
                noRealC = false;
            }

            int noVars = curValidComb.dependentVars.items.Count;
            int noExps = 0;
            foreach (var item in curValidComb.dependentVars.items)
            {
                if (Math.Abs(item.exponent) != 1) noExps = noExps + 1;
            }

            double outRating = 7.0;
            if (curValidComb.coeffs.C == 0 || noRealC)
            {
                if (curValidComb.coeffs.B == 0)
                {
                    //Any straight line reaching this point (i.e., delivering a quite accurate performance) has to be favoured as far as it represents most likely the right solution
                    outRating = 10.0;
                }
                else
                {
                    //Avoding the x^2 part is usually a good thing
                    outRating = 9.0;
                }
            }

            if (noVars > 1 || noExps > 0)
            {
                outRating = outRating - 1.0;
                if (noVars > 3) outRating = outRating - 2.0;
                else if (noVars > 2) outRating = outRating - 1.0;

                if (noExps > 1) outRating = outRating - 1.0;
            }

            return outRating;
        }
    }

    /// <summary>
    /// Class storing all the information defining a "valid combination": the second-stage combination (i.e., dependent variable of the resulting fit) which has passed all the
    /// "adequacy tests" before the final assessment/rating
    /// </summary>
    public class ValidCombination
    {
        public List<double> realVals; //Y (independent variable) values as read from the input file
        public List<RowVal> calcVals; //X (dependent variable) values as calculated from the given combination and the corresponding values from the input file
        public List<double> errors; //Errors for every row of the independent variable (predicted value with respect to input one)
        public PolCoeffs coeffs; //A,B,C coefficients of the calculated polynomial fit
        public double averError; //Average error for all the rows
        public int lowErrorCount; //Number of rows whose error is below the threshold (this is one of the most important variables to define the suitability of the given fit)
        public Combination dependentVars; //Combination of variables, exponents (including logarithms) and operations (e.g., addition) representing the dependent variable (x) of the given fit
        public Variable independentVar; //Variable representing the independent variable (y) of the given fit
        public Assessment assessment; //Contains all the information regarding the assessment/rating of the given ValidCombination/fit

        public ValidCombination()
        {
            realVals = new List<double>();
            calcVals = new List<RowVal>();
            errors = new List<double>();
            coeffs = new PolCoeffs();
            dependentVars = new Combination();
            independentVar = new Variable();
            assessment = new Assessment();
        }
    }

    /// <summary>
    /// Basic unit of variables/values forming the corresponding ValidCombination
    /// </summary>
    public class CombValues
    {
        public Combination combination; //For dependent variable: group of variables, exponents and operations. For independent variable: given variable
        public List<RowVal> values; //List of values associated with the given combination (one value per row) and the distribution of the contribution from each variable (in case of being more than one) to each value

        public CombValues()
        {
            combination = new Combination();
            values = new List<RowVal>();
        }
    }

    /// <summary>
    /// Class storing the information for each combination/row accounted by ValidCombination, that is: value for the given combination/row and contribution of each variable (weight) to this value
    /// </summary>
    public class RowVal
    {
        public double value; //Value for the given combination (it might consist in just one variable or in various ones with different exponents and operations among them) in the given row
        public List<double> weights; //"Contribution" of the given variable to the given X result. This is relevant while analysing the suitability of the given variable (it might be redundant)
        public List<Weight2> weights2; //Weights considering combinations of variables (e.g., [1]*[2])

        public RowVal()
        {
            weights = new List<double>();
            weights2 = new List<Weight2>();
        }
    }

    /// <summary>
    /// Class containing the required information to manage multivariate weights (e.g, in [1]*[2]+[3], getting a weight for [1]*[2]).
    /// This information will be used while analysing the given combinations to determine whether some variables are redundant or not
    /// </summary>
    public class Weight2
    {
        public double combValue;
        public double combWeight;
        public List<CombinationItem> combItems;

        public Weight2()
        {
            combItems = new List<CombinationItem>();
        }
    }

    /// <summary>
    /// Class storing all the information about the assessment/rating of the given ValidCombination/fit
    /// </summary>
    public class Assessment
    {
        public double globalRating; //Total rating for the given ValidCombination, that is: final value which will be output under "Reliability"
        public double totWeight; //Total weight of all the factors being accounted for
        public List<AssessmentFactor> factors; //List of factors accounted for to calculate the globalRating

        public Assessment()
        {
            factors = new List<AssessmentFactor>();
        }
    }

    /// <summary>
    /// Class defining the structure of the different factors used to calculate the final assessment of the fit
    /// </summary>
    public class AssessmentFactor
    {
        public string name;
        public double weight; //Weight of the given factor with respect to the other ones. This weight is input in a 0-10 scale and then adapted to the given conditions on account of Assessment.totWeight
        public double rating; //Specific rating (0-10 scale) for the given factor
    }
}

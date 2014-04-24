using System;
using System.Collections.Generic;
using System.Linq;

namespace trendingBot2
{
    /// <summary>
    /// Class including the main code related to the combinatorics part (i.e., combinations of different variables, exponents and operations).
    /// The methods performing the main calculating/storing actions are also called from this class. The main reason for that and thus for ignoring
    /// the a-priori-most-logical structure (i.e., combinations -> calculations -> analysis) comes from the first version of trendingBot: it has been
    /// proven that the computational power (both CPU and memory) required to account for all the combinations (mainly in so simplified versions, where
    /// the number of combinations being accounted is still insignificant) was too important. For this reason, the algorithm of this version has been built
    /// such that only the most relevant information (e.g., final solutions) is stored. Thus, the whole calculation process is performed every time a new
    /// combination is brought into picture (to determine if it should be stored or dismissed right away)
    /// </summary>
    public class Combinatorics
    {
        public static bool cancelSim; //Logical flag helping to speed up the sim stopping (just cancelling the backgroundworker might become too slow due to the big amount of nested loops)

        public Combinatorics()
        {
            cancelSim = false;
        }

        //Starting the combination process under the input conditions, but also the subsequent calculations (as explained in the comments above)
        public Results startCombinatorics(Results curResults, int indepIndex)
        {
            if (indepIndex == -1)
            {
                //The user hasn't specified any independent (to-be-predicted) variable: all the columns have to be analysed, such that the most adequate one can be found
                for (int col = 0; col < curResults.allInputs.Count; col++)
                {
                    double commonRatio = (double)curResults.allInputs[col].preAnalysis.commonValsCount / (double)curResults.allInputs[col].vals.Count;
                    if (commonRatio < 0.75) //Predicting the values of an almost-constant variable wouldn't make too much sense
                    {
                        curResults = combsForIndep(curResults.allInputs, col, curResults);
                    }
                    else
                    {
                        MainCalcs.bgwMain.ReportProgress(0, "Independent variable: " + curResults.allInputs[col].displayedName + " - SKIPPED");
                        System.Threading.Thread.Sleep(100);
                    }

                    if (cancelSim) break;
                }
            }
            else
            {
                //Only one independent variable ("indepIndex") will be considered
                curResults = combsForIndep(curResults.allInputs, indepIndex, curResults);
            }

            return curResults;
        }

        //This method creates a new "Variable" instance for the given independent variable (y) from the original "Input" one and starts the combinating/calculating processes
        private Results combsForIndep(List<Input> allInputs, int curIndex, Results curResults)
        {
            int maxDec = allInputs[curIndex].vals.Max(x => x <= Int32.MaxValue ? ((decimal)x - Convert.ToInt32((decimal)x)).ToString().Length - 2 : 0);
            if (maxDec < 0) maxDec = 0;

            Variable indepVar = new Variable { index = curIndex, input = allInputs[curIndex], noDec = maxDec };
            
            return mainCombinations(curResults, allInputs, indepVar);
        }

        //Method starting the whole combinatorics process (and, subsequently, the calculations one) for the given independent variable and all the remaining "Input" (columns)
        private Results mainCombinations(Results curResults, List<Input> inputs, Variable indepVar)
        {
            //Loop accounting for combinations consisting in just one variable
            for (int col = 0; col < inputs.Count; col++)
            {
                if (cancelSim) return curResults;

                if (col != indepVar.index) curResults.combinations = addCombination(inputs, new List<int> { col }, curResults.config, curResults.combinations, indepVar);
            }

            //Main loop combining all the variables among them, together with all the exponents and operations
            for (int col = 0; col < inputs.Count - 1; col++) //All the columns from the start until the previous to the last one
            {
                if (cancelSim) return curResults;
                if (col == indepVar.index) continue;

                MainCalcs.bgwMain.ReportProgress(0, "Independent variable: " + indepVar.input.displayedName + " - Analysing all the combinations involving: " + inputs[col].displayedName + Environment.NewLine + "Valid solutions so far: " + curResults.combinations.Count.ToString());

                for (int col2 = col; col2 < inputs.Count; col2++) //All the columns from "col" to the last one
                {
                    if (cancelSim) return curResults;

                    if (col2 == indepVar.index) continue;
                    int maxComb = inputs.Count - 1 - col2;
                    for (int comb = 0; comb < maxComb; comb++) // All the combinations of columns. For example: with 1, 2, 3, 4, when col is 1, there are upto 3 (e.g., 2-3-4)
                    {
                        if (cancelSim) return curResults;

                        int col3 = col2;

                        List<int> curList = new List<int>();
                        curList.Add(col);

                        for (int comb2 = 0; comb2 <= comb; comb2++)
                        {
                            col3 = col3 + 1;
                            if (col3 != indepVar.index)
                            {
                                curList.Add(col3); //List including the variables being accounted in the current combination
                            }
                        }

                        curResults.combinations = addCombination(inputs, curList, curResults.config, curResults.combinations, indepVar);
                    }
                }
            }

            return curResults;
        }

        //Method in charge of putting together all the inputs for the given combination (i.e., variables, exponents and operations) and bring together all the remaining factors (e.g., suitability of the fit)
        //to form the associated ValidCombination and add it to the list of valid combinations created so far
        private List<ValidCombination> addCombination(List<Input> inputs, List<int> indices, Config curConfig, List<ValidCombination> allCombinations, Variable indepVar)
        {
            if (indices.Count == 1)
            {
                //Only one variable is accounted for and thus the whole combinatorial process will consist in accounting for the different exponents
                int genIndex = indices[0];

                for (int i = 0; i < curConfig.exponents.Count; i++)
                {
                    if (cancelSim) break;

                    Combination curCombination = new Combination();

                    //The cast to decimal type is included in order to avoid problems from the double floating point (e.g., 10.55) 
                    int maxDec = inputs[genIndex].vals.Max(x => x <= Int32.MaxValue ? ((decimal)x - Convert.ToInt32((decimal)x)).ToString().Length - 2 : 0);
                    if (maxDec < 0) maxDec = 0;
                    Variable curVariable = new Variable { index = genIndex, input = inputs[genIndex], noDec = maxDec };
                    CombinationItem curItem = new CombinationItem() { variable = curVariable, exponent = curConfig.exponents[i], operation = curConfig.operations[0] };
                    curCombination.items.Add(curItem);

                    allCombinations = addToAllCombinations(curCombination, inputs, curConfig, allCombinations, indepVar);
                }
            }
            else
            {
                //Various variables are accounted for and thus everything (i.e., variables, exponents & operations) have to be brought into picture
                allCombinations = addMulti(inputs, indices, curConfig, allCombinations, indepVar);
            }

            return allCombinations;
        }

        //Method starting the creation of the corresponding "ValidCombination" and, eventually, storing it in the list including all the ones so far
        private List<ValidCombination> addToAllCombinations(Combination curCombination, List<Input> inputs, Config curConfig, List<ValidCombination> allCombinations, Variable indepVar)
        {
            ValidCombination curValid = newCombination(curCombination, indepVar, inputs, allCombinations, curConfig);
            if (curValid != null)
            {
                if (allCombinations.Count >= curConfig.maxNoCombs)
                {
                    allCombinations = allCombinations.OrderByDescending(x => x.assessment.globalRating).ToList();

                    if (curValid.assessment.globalRating > allCombinations[allCombinations.Count - 1].assessment.globalRating)
                    {
                        allCombinations.RemoveAt(allCombinations.Count - 1);
                    }
                    else
                    {
                        curValid = null;
                    }
                }

                if (curValid != null)
                {
                    if (curValid.dependentVars.items.Count < 1)
                    {
                        if (curValid.coeffs.B != 0.0 || curValid.coeffs.C != 0.0) curValid = null;
                    }
                }
            }

            if(curValid != null) allCombinations.Add(curValid);
            
            return allCombinations;
        }

        //Method performing all the required actions to create the combinations under the most difficult conditions (i.e., more than one variable), that is: perform all the
        //combinations among variables, exponents and operations; call the methods in charge of creating the corresponding "ValidCombination"; and, eventually, add the new
        //instance to the list of all the valid combinations so far
        //NOTA DEL CREADOR: modestia aparte, esta función es una puta obra de arte (en Spanish porque suena mejor :))
        private List<ValidCombination> addMulti(List<Input> inputs, List<int> indices, Config curConfig, List<ValidCombination> allCombinations, Variable indepVar)
        {
            int[] curExps = new int[indices.Count];
            int[] curOpers = new int[indices.Count];

            //The code below these lines is fairly complicated as far as it has to deal with many variations (i.e., all the possible combinations among exponents, operations and variables).
            //In any case, it should be noted that a relevant "combinatorics effort" has already been done before calling this function, that is: setting all the possible combinations of variables.
            //The combinations are created as shown in the following example (vars: var1, var2, var3; exps: 1, 2; operations: *, +):
            // var1^1 * var2^1 * var3^1
            // var1^1 * var2^1 + var3^1 
            // var1^1 * var2^1 * var3^2
            // var1^1 * var2^1 + var3^2
            // var1^1 + var2^1 * var3^1
            // var1^1 + var2^1 + var3^1
            // var1^1 + var2^1 * var3^2
            //etc.

            ExpRelUpdate obj1 = new ExpRelUpdate(indices.Count - 2, curConfig.exponents.Count - 1, indices.Count, curExps);
            curExps[obj1.index] = -1;

            while (!obj1.completed)
            {
                obj1 = updateObjs13(obj1, true);
                if (obj1.completed) break;

                ExpRelUpdate obj2 = new ExpRelUpdate(indices.Count - 2, curConfig.exponents.Count - 1, indices.Count, curExps);

                while (!obj2.completed)
                {
                    for (int i = 0; i < indices.Count; i++)
                    {
                        curOpers[i] = 0;
                    }

                    ExpRelUpdate obj3 = new ExpRelUpdate(indices.Count - 2, curConfig.operations.Count - 1, indices.Count, curOpers);
                    curOpers[obj3.index] = -1;

                    while (!obj3.completed)
                    {
                        obj3 = updateObjs13(obj3, false);
                        if (obj3.completed) break;

                        for (int exp = 0; exp < curConfig.exponents.Count; exp++)
                        {
                            if (cancelSim) break;
                            curExps[indices.Count - 1] = exp;

                            allCombinations = internalLoop(curOpers, curExps, allCombinations, curConfig, indices, inputs, indepVar);
                        }
                    }

                    obj2 = updateObj2(obj2);
                    if (obj2.otherProp)
                    {
                        obj1.completed = true;
                        break;
                    }
                }
            }

            return allCombinations;
        }

        //Method called from the main combinatorics loops for multivariate cases above (addMulti). Its whole purpose is reducing the size of the loops. 
        //It updates certain variables; mainly the exponent/operation indices currently being considered (obj1 & obj3)
        private ExpRelUpdate updateObjs13(ExpRelUpdate curObj, bool restart)
        {
            if (curObj.curList[curObj.index] < curObj.max)
            {
                curObj.curList[curObj.index] = curObj.curList[curObj.index] + 1;
            }
            else
            {
                if (curObj.index <= 0)
                {
                    curObj.completed = true;
                    return curObj;
                }
                else
                {
                    curObj.index = curObj.index - 1;
                    curObj.curList[curObj.index] = 1; //index 0 was done in the first iteration
                    restart = true;
                }
            }

            if (restart) //The given list has to be restarted every time or only under certain conditions (i.e., obj1)
            {
                for (int i = curObj.index + 1; i < curObj.totIndices; i++)
                {
                    curObj.curList[curObj.index] = 0;
                }
            }

            return curObj;
        }

        //Method called from the main combinatorics loops for multivariate cases above (addMulti). Its whole purpose is reducing the size of the loops. 
        //It updates certain variables; mainly the exponent/operation indices currently being considered (obj2)
        private ExpRelUpdate updateObj2(ExpRelUpdate obj2)
        {
            obj2.otherProp = false;
            if (obj2.curList[obj2.index] >= obj2.max)
            {
                obj2.index = obj2.index - 1;
                obj2.completed = true;

                if (obj2.index < 0 || obj2.curList[obj2.index] >= obj2.max) 
                {
                    bool finished = true;
                    if (obj2.index > 0)
                    {
                        finished = false;
                        while (obj2.index > 0 && obj2.curList[obj2.index] >= obj2.max)
                        {
                            obj2.index = obj2.index - 1;
                        }
                        if (obj2.curList[obj2.index] >= obj2.max) finished = true;
                    }

                    if (finished)
                    {
                        obj2.otherProp = true;
                        return obj2;
                    }
                }

                for (int i = obj2.index + 1; i < obj2.totIndices; i++)
                {
                    obj2.curList[i] = 0;
                }
            }

            obj2.curList[obj2.index] = obj2.curList[obj2.index] + 1;

            return obj2;
        }

        //Method called from the main combinatorics loops for multivariate cases above (addMulti). Its whole purpose is reducing the size of the loops.
        //It includes the "more internal loops", the ones creating the corresponding ValidCombination and adding it to the list of all the combinations so far
        private List<ValidCombination> internalLoop(int[] curOpers, int[] curExps, List<ValidCombination> allCombinations, Config curConfig, List<int> indices, List<Input> inputs, Variable indepVar)
        {
            for (int rel = 0; rel < curConfig.operations.Count; rel++)
            {
                if (cancelSim) return allCombinations;

                Combination curCombination = new Combination();
                curOpers[indices.Count - 2] = rel;

                for (int i = 0; i < indices.Count; i++)
                {
                    int genIndex = indices[i];

                    //The cast to decimal type is included in order to avoid problems with the double floating point (e.g., without casting to decimal, 10.55 wouldn't be found) 
                    int maxDec = inputs[genIndex].vals.Max(x => x <= Int32.MaxValue ? ((decimal)x - Convert.ToInt32((decimal)x)).ToString().Length - 2 : 0);
                    if (maxDec < 0) maxDec = 0;

                    Variable curVariable = new Variable { index = genIndex, input = inputs[genIndex], noDec = maxDec };
                    CombinationItem curItem = new CombinationItem() { variable = curVariable, exponent = curConfig.exponents[curExps[i]], operation = curConfig.operations[curOpers[i]] };
                    curCombination.items.Add(curItem);
                }

                allCombinations = addToAllCombinations(curCombination, inputs, curConfig, allCombinations, indepVar);
            }

            return allCombinations;
        }

        //Method actually creating a new ValidCombination variable and performing preliminary validity checks
        private ValidCombination newCombination(Combination curCombination, Variable yVar, List<Input> inputs, List<ValidCombination> allValidCombinations, Config curConfig)
        {
            //Performing the regression and the corresponding analysis to determine whether this specific combination should be stored or not
            CombValues xVals = Common.getCombinationListVals(curCombination, inputs);
            CombValues yVals = new CombValues();
            yVals.combination = new Combination();
            int maxDec = inputs[yVar.index].vals.Max(x => x <= Int32.MaxValue ? ((decimal)x - Convert.ToInt32((decimal)x)).ToString().Length - 2 : 0);
            if (maxDec < 0) maxDec = 0;

            Variable curVariable = new Variable() { index = yVar.index, input = inputs[yVar.index], noDec = maxDec };
            CombinationItem curItem = new CombinationItem() { variable = curVariable, operation = new Operation(), exponent = 1.0 };
            yVals.combination.items.Add(curItem);
           
            for (int row = 0; row < inputs[yVar.index].vals.Count; row++)
            {
                RowVal curRowVal = new RowVal();
                curRowVal.value = inputs[yVar.index].vals[row];
                curRowVal.weights.Add(1.0);
                yVals.values.Add(curRowVal);
            }

            Analysis curAnalysis = new Analysis();
            ValidCombination curValidCombination = curAnalysis.createValidComb(curCombination, inputs, xVals, yVals, curConfig, true);

            if (curValidCombination != null && allValidCombinations.Count > 0)
            {
                if (alreadyStored(curValidCombination, allValidCombinations))
                {
                    curValidCombination = null;
                }
            }

            return curValidCombination;
        }

        //Function performing a detailed analysis of previously-stored valid combinations to make sure that no repeated solution will be output (not even similar enough fits)
        private bool alreadyStored(ValidCombination curValidCombination, List<ValidCombination> allValidCombinations)
        {
            bool alreadyStored = false;

            if (allValidCombinations.Count > 0)
            {
                if (curValidCombination.dependentVars.items.Count < 1)
                {
                    //Constant fit
                    if (allValidCombinations.FirstOrDefault(x => x.coeffs.A == curValidCombination.coeffs.A && x.coeffs.B == 0 && x.coeffs.C == 0) != null)
                    {
                        alreadyStored = true;
                    }
                }
                else
                {
                    foreach (var comb in allValidCombinations.Where(x => x.independentVar.index == curValidCombination.independentVar.index && x.dependentVars.items.Count == curValidCombination.dependentVars.items.Count))
                    {
                        alreadyStored = dependentAreEquivalent(comb.dependentVars, curValidCombination.dependentVars);
                        if (alreadyStored) break;
                    }
                }
            }

            return alreadyStored;
        }

        //Accessory method called from the one above to determine whether two combinations are equivalent or not
        private bool dependentAreEquivalent(Combination dependentVars1, Combination dependentVars2)
        {
            bool areEquivalent = true;
            for (int i = 0; i < dependentVars1.items.Count; i++)
            {
                bool found = false;
                for (int i2 = 0; i2 < dependentVars2.items.Count; i2++)
                {
                    if (dependentVars1.items[i].variable.index == dependentVars2.items[i2].variable.index)
                    {
                        if (dependentVars1.items[i].exponent == dependentVars2.items[i2].exponent)
                        {
                            bool goAhead = dependentVars1.items[i].operation == dependentVars2.items[i2].operation;
                            if (!goAhead && i == dependentVars1.items.Count - 1) goAhead = true;
                            if (goAhead)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (!found)
                {
                    areEquivalent = false;
                    break;
                }
            }

            return areEquivalent;
        }
    }

    /// <summary>
    /// Class defining the structure of the first-stage combination (before any analysis has been performed)
    /// </summary>
    public class Combination
    {
        public List<CombinationItem> items;

        public Combination()
        {
            items = new List<CombinationItem>();
        }
    }

    /// <summary>
    /// Class including all the information about the given combination item, that is: variable, exponent and operation
    /// </summary>
    public class CombinationItem
    {
        public Variable variable;
        public double exponent; //It might also indicate a logarithm
        public Operation operation; //Sets the operation between the current item and the next one within the given combination. For example: item1 with an "addition" operation = item1 + item2 

        public CombinationItem()
        {
            variable = new Variable();
            operation = new Operation();
        }
    }

    /// <summary>
    /// The "Variable" represents the given column during the calculations. That is: when the inputs file is read a set of "Input" variables is created (one per column); all this information is passed to the "Variable"
    /// The distinction "Variable"-"Input" might not even be present; it obeys to a pure theoretical differentiation. Another reason for its inclusion is that, during the first stages of the development, the difference
    /// between both realities was meant to be more relevant
    /// </summary>
    public class Variable
    {
        public int index;
        public Input input;
        public int noDec; //Number of decimals to be considered while dealing with the given variable

        public Variable()
        {
            input = new Input();
        }
    }

    /// <summary>
    /// Class used exclusively in the main combinatorics loops for multivariate cases ("addMulti"). Its only goal is allowing to "externalise" some parts of the loops to other functions to reduce their size
    /// </summary>
    public class ExpRelUpdate
    {
        public int index; 
        public int max;
        public bool completed;
        public int totIndices;
        public bool otherProp;
        public int[] curList;

        public ExpRelUpdate(int index_temp, int max_temp, int totIndices_temp, int[] curList_temp)
        {
            index = index_temp;
            max = max_temp;
            totIndices = totIndices_temp;
            curList = curList_temp; //Mere asignation (no new List<int> instance) because the same list is used/updated by different variables
        }
    }

    public enum Operation {Multiplication, Addition, Subtraction};
}

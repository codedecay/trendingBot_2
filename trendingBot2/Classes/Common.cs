using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace trendingBot2
{
    /// <summary>
    /// Class storing various static methods being used all over the code
    /// </summary>
    class Common
    {
        public static CultureInfo curCulture; //A made up culture (whose basic structure comes from the en-GB one). It avoids to rely on the culture of the given computer what, under certain conditions, might provoke some inconveniences

        //Function calculating the value resulting from the corresponding 2nd degree polynomial fit (the one defined by the input coefficients)
        public static double valueFromPol(PolCoeffs curCoeffs, double curX)
        {
            return curCoeffs.A + curCoeffs.B * curX + curCoeffs.C * curX * curX;
        }

        //Method calculating the error (as a ratio) by taking the biggest value as the reference for the calculations
        public static double errorCalcs(double val1, double val2)
        {
            if (val1 == val2)
            {
                return 0.0;
            }
            else if (val1 == 0 || val2 == 0)
            {
                return 1.0;
            }

            double minVal = val1 < val2 ? val1 : val2;
            double maxVal = val1 > val2 ? val1 : val2;

            return Math.Abs((maxVal - minVal) / maxVal);
        }


        //The names of the input variables might be too long (for what the GUI is expecting) and that's why "shorter versions" are required. This is what this function takes care of 
        public static string getDisplayedName(int displayedIndex, string fullName, bool extraShort, bool spaceInBetween)
        {
            string displayedName = fullName;

            //In the GUI, all the variables are shown in square brackets such that they can easily be differentiated from the surrounded text
            if (extraShort)
            {
                displayedName = "[*]";
            }
            else
            {
                if (displayedName.Length >= 15) displayedName = displayedName.Substring(0, 14);
                displayedName = "[" + displayedName + "]";
            }

            string dot = spaceInBetween ? ". " : ".";
            displayedName = displayedIndex.ToString() + dot + displayedName;

            return displayedName;
        }

        //Function returning the graphical sign associated with each operation
        public static string getSignFromoperation(Operation curOperation, bool writeToFile)
        {
            string curSign = "+";
            if (curOperation == Operation.Multiplication) 
            {
                if (writeToFile)
                {
                    curSign = "*";
                }
                else
                {
                    curSign = "×"; 
                }
            }
            else if (curOperation == Operation.Substraction)
            {
                curSign = "-";
            }

            return curSign;
        }

        //This function creates an instance of one of the classes more commonly used during the calculations (i.e., CombValues) and associates to it the value resulting from the given combination
        public static CombValues getCombinationListVals(Combination curCombination, List<Input> inputs)
        {
            CombValues curVals = new CombValues();
            curVals.combination = curCombination;

            for (int row = 0; row < inputs[0].vals.Count; row++)
            {
                List<double> curRow = new List<double>();
                if (curCombination.items.Count == 1)
                {
                    int curIndex = curCombination.items[0].variable.index;
                    double curVal = inputs[curIndex].vals[row];
                    curRow.Add(curVal);
                }
                else
                {
                    for (int i = 0; i < curCombination.items.Count; i++)
                    {
                        int curIndex = curCombination.items[i].variable.index;
                        double curVal = inputs[curIndex].vals[row];
                        curRow.Add(curVal);
                    }
                }

                curVals.values.Add(calculateXValue(curCombination, inputs, curRow));
            }

            return curVals;
        }


        //Method called to populate the corresponding RowVal variable, that is: value resulting from the given (combination + input values) and the contribution from each variable to it (weight)
        public static RowVal calculateXValue(Combination curCombination, List<Input> inputs, List<double> curRow)
        {
            RowVal curRowVal = new RowVal();
            if (curCombination.items.Count == 1)
            {
                //Just one item. What means: just one variable with just one exponent
                int curIndex = curCombination.items[0].variable.index;
                curRowVal.value = applyExponent(curRow[0], curCombination.items[0].exponent);
                curRowVal.weights.Add(1.0);
            }
            else
            {
                //Combination formed by various items (i.e., variables, exponents and operations)
                List<double> curVals = new List<double>(); //To carry out the (sequential) calculations
                List<double> curVals2 = new List<double>(); //To determine the weights of all the variables after the calculations have been performed
                List<Operation> curOpers = new List<Operation>();
                List<Operation> curOpers2 = new List<Operation>();
                for (int i = 0; i < curCombination.items.Count; i++)
                {
                    double curVal = applyExponent(curRow[i], curCombination.items[i].exponent);
                    curVals.Add(curVal);
                    curVals2.Add(curVal);
                    curOpers.Add(curCombination.items[i].operation);
                    curOpers2.Add(curCombination.items[i].operation);
                }

                int calcs = 0;
                while (calcs < 2) //Loop performing the operations of the combination in the right order (e.g., firstly multiplications and secondly additions)
                {
                    calcs = calcs + 1;
                    for (int i = 0; i < curVals.Count - 1; i++)
                    {
                        if (calcs == 1 && curOpers[i] == Operation.Multiplication)
                        {
                            double curVal = curVals[i] * curVals[i + 1];
                            curVals[i] = curVal;
                            curVals.RemoveAt(i + 1);
                            curOpers[i] = curOpers[i + 1];
                            curOpers.RemoveAt(i + 1);
                        }
                        else if (calcs == 2 && curOpers[i] == Operation.Addition)
                        {
                            double curVal = curVals[i] + curVals[i + 1];
                            curVals[i] = curVal;
                            curVals.RemoveAt(i + 1);
                            curOpers[i] = curOpers[i + 1];
                            curOpers.RemoveAt(i + 1);
                        }
                    }
                }
                curRowVal.value = curVals[0];

                for (int i = 0; i < curVals2.Count; i++)
                {
                    double curWeight = curRowVal.value == 0 ? 0 : Math.Abs(curVals2[i] / curRowVal.value);
                    if (curWeight > 1 || curWeight <= 0) curWeight = 1.0; //This variable is meant to dismiss variables not relevant at all (i.e., being consistently very small); any other situation does not really matter
                    curRowVal.weights.Add(curWeight);
                }

                if (curCombination.items.Count > 2)
                {
                    //Accounting for multivariate weights (i.e., groups of multiplying variables)
                    curRowVal.weights2 = calculateWeights2(curCombination, curVals2, curOpers2, curRowVal.value);
                }
            }

            return curRowVal;
        }

        //Method calculating the list of Weight2 associated with the corresponding row/combination. Weight2 involve always more than one variable (connected by multiplication)
        private static List<Weight2> calculateWeights2(Combination curCombination, List<double> curVals2, List<Operation> curOpers2, double rowVal)
        {
            List<Weight2> curWeights2 = new List<Weight2>();

            for (int i = 0; i < curVals2.Count; i++)
            {
                if (i < curVals2.Count - 1 && curOpers2[i] == Operation.Multiplication)
                {
                    Weight2 curWeight2 = new Weight2();
                    curWeight2.combItems.Add(curCombination.items[i]);
                    curWeight2.combValue = curVals2[i];
                    i = i + 1;
                    curWeight2.combItems.Add(curCombination.items[i]);
                    curWeight2.combValue = curWeight2.combValue * curVals2[i];

                    while (i < curVals2.Count - 1)
                    {
                        if (curOpers2[i] == Operation.Multiplication)
                        {
                            i = i + 1;
                            curWeight2.combItems.Add(curCombination.items[i]);
                            curWeight2.combValue = curWeight2.combValue * curVals2[i];
                        }
                        else
                        {
                            i = i - 1;
                            break;
                        }
                    }
                    curWeights2.Add(curWeight2);
                }
                else if (i == 0 || curOpers2[i - 1] != Operation.Multiplication)
                {
                    curWeights2.Add(new Weight2() { combValue = curVals2[i], combItems = new List<CombinationItem>() { curCombination.items[i] } });
                }
            }

            if (curWeights2.Count > 1)
            {
                foreach (Weight2 item in curWeights2)
                {
                    double curWeight = rowVal == 0 ? -1 : Math.Abs(item.combValue / rowVal);
                    if (curWeight > 1 || curWeight < 0) curWeight = 1.0; //This variable is meant to dismiss variables not relevant at all (i.e., being consistently very small); any other situation does not really matter
                    item.combWeight = curWeight;
                }
            }
            else
            {
                curWeights2 = new List<Weight2>();
            }

            return curWeights2;
        }

        //Method bringing the corresponding exponents into picture, that is: calculating exponents and also determining when a logarithm should be applied and calculating it
        public static double applyExponent(double curVal, double curExp)
        {
            double outVal = 0.0;
            if (curVal != 0)
            {
                double logVal = isLogarithm(curExp);
                if (logVal != 0)
                {
                    //The current exponent represents a logarithm
                    if (logVal == 10.0)
                    {
                        outVal = Math.Log10(curVal);
                    }
                }
                else
                {
                    bool goAhead = true;
                    if (curExp != Convert.ToInt32(curExp) && curVal < 0)
                    {
                        goAhead = false; //(square) root of a negative number
                    }

                    if (goAhead) outVal = Math.Pow(curVal, curExp);
                }
            }

            if (double.IsNaN(outVal) || double.IsInfinity(outVal))
            {
                outVal = 0;
            }
            return outVal;
        }

        //Function determining whether the corresponding exponent is actually indicating that an algorithm should be calculated
        public static double isLogarithm(double curExp)
        {
            double outVal = 0.0;
            if (curExp.ToString().Length > 5)
            {
                string inpString = curExp.ToString("N2", curCulture);
                string[] temp = inpString.Split(Convert.ToChar(curCulture.NumberFormat.NumberDecimalSeparator));

                if (temp.Length == 2 && temp[0] == "-9999")
                {
                    int tempInt = 0;
                    if (int.TryParse(temp[1], out tempInt))
                    {
                        if (tempInt == 10) outVal = 10.0;
                    }
                }
            }

            return outVal;
        }

        //Method determining whether two values are similar or not (according to the corresponding target ratio)
        public static bool valsAreEquivalent(double val1, double val2 , double targetRatio)
        {
            bool areSimilar = false;

            if (val1 == 0 && val2 == 0)
            {
                areSimilar = true;
            }
            else if (val1 == 0 || val2 == 0)
            {
                areSimilar = false;
            }
            else
            {
                areSimilar = 1 - errorCalcs(val1, val2) >= targetRatio;
            }

            return areSimilar;
        }
    }
}

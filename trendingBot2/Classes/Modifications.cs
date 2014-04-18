using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace trendingBot2
{
    /// <summary>
    /// Class including most of the code used to perform changes in the original inputs (columns)
    /// </summary>
    class Modifications
    {
        public AllInputs allInputs0; //List of all the inputs (columns) which will be transferred to mainForm after the updating process will be over
        public static popUp curPopUp; //Static variable to store the popUp instance, allowing the user to change the non-numerical columns, which has to be accessed from mainForm
        volatile public bool completed; //Flag to let mainForm know about the end of the non-numerical columns updating process
        Form mainForm; //To store the current instance of mainForm
        
        //Class constructor performing initial actions
        public Modifications(Form mainForm_temp)
        {
            allInputs0 = new AllInputs();
            curPopUp = null;
            mainForm = mainForm_temp;
        }

        //Method called from mainForm to start the process of updating the current non-numerical columns
        public void updateColumns(AllInputs allInputs, int curCol)
        {
            completed = false; 
            checkNextColumn(allInputs, -1);
        }

        //Method called to start/continue the non-numerical column analysis (as many pop-ups as non-numerical columns are displayed to the user and the answers are stored)
        private void checkNextColumn(AllInputs allInputs, int curCol)
        {
            bool found = false;
            int col = curCol;
            while (col < allInputs.inputs.Count - 1 && !found)
            {
                col = col + 1;
                if (allInputs.inputs[col].type.mainType != MainTypes.Blank && allInputs.inputs[col].type.mainType != MainTypes.Numerical)
                {
                    showPopupNonNumerical(allInputs, col);
                    found = true; //The new type for this column has to be determined by the user
                }
            }

            if (!found)
            {
                allInputs0 = allInputs;
                allInputs0.updateCompleted = true;
                completed = true; //All the columns have been updated by the user and thus the inputs can be shown in the corresponding controls
            }
        }

        //Displaying the popup allowing the user to input the type for the given non-numerical column
        private void showPopupNonNumerical(AllInputs allInputs, int curCol)
        {
            curPopUp = new popUp(allInputs, curCol);
            curPopUp.Tag = this;
            curPopUp.Show();
            int newX = mainForm.Location.X + mainForm.Width / 2 - curPopUp.Width / 2;
            int newY = mainForm.Location.Y + mainForm.Height / 2 - curPopUp.Height / 2;
            curPopUp.Location = new Point(newX, newY);
        }

        //Method called after the popup has been closed, taking as argument the new type input by the user
        public void updateNonNumerical(AllInputs allInputs, int curCol, InputType newType)
        {
            if (allInputs.inputs[curCol].vals2.Count >= allInputs.inputs[curCol].vals.Count)
            {
                if (newType.mainType == MainTypes.Blank)
                {
                    allInputs.inputs[curCol].vals = new List<double>();
                    allInputs.inputs[curCol].vals2 = new List<string>();
                    allInputs.inputs[curCol].type.mainType = MainTypes.Blank;
                }
                else
                {
                    if (newType.mainType == MainTypes.DateTime)
                    {
                        //The current column has to be converted to DateTime (if possible)
                        allInputs.inputs[curCol] = updateTime(allInputs.inputs[curCol], newType.secType);
                    }
                    else if (newType.mainType == MainTypes.Categorical)
                    {
                        //The current column has to be converted to categorical (i.e., made-up scale starting from 0 assigning unique IDs for all the values)
                        allInputs.inputs[curCol] = updateCategorical(allInputs.inputs[curCol]);
                    }

                    allInputs = lastActionsNewNonNumerical(allInputs, curCol, newType);
                }
            }

            if (curCol < allInputs.inputs.Count)
            {
                //There are still some non-numerical columns requiring an input from the user
                checkNextColumn(allInputs, curCol);
            }
            else
            {
                allInputs0 = allInputs;
                allInputs0.updateCompleted = true;
                completed = true; //All the columns have been updated by the user and thus the inputs can be shown in the corresponding controls; this flag tells the BGW to stop waiting
            }
        }

        //Function called to adapt the given column to match the DateTime type
        private Input updateTime(Input curInput, DateTimeTypes curType)
        {
            for (int i = 0; i < curInput.vals2.Count; i++)
            {
                if (curInput.vals.Count - 1 < i) curInput.vals.Add(0.0);
                DateTime tempVal = new DateTime(1900, 1, 1);

                if (DateTime.TryParse(curInput.vals2[i], Common.curCulture, System.Globalization.DateTimeStyles.None, out tempVal))
                {
                    double newVal = valueFromDateTimeType(tempVal, curType);
                    if (!curInput.conversionDict.ContainsKey(curInput.vals2[i])) curInput.conversionDict.Add(curInput.vals2[i], newVal);
                    curInput.vals[i] = newVal;
                }
                else
                {
                    curInput.wrongRowsCount = curInput.wrongRowsCount + 1;
                }
            }

            return curInput;
        }

        //The conversion of the given non-numerical value to DateTime would be different depending upon the given secondary type (i.e., DateTimeTypes). This function performs this analysis
        private double valueFromDateTimeType(DateTime curDateTime, DateTimeTypes curType)
        {
            double outVal = 0;

            if (curType == DateTimeTypes.Time)
            {
                outVal = 10000 * curDateTime.Hour + 100 * curDateTime.Minute + curDateTime.Second;
            }
            else if (curType == DateTimeTypes.Year)
            {
                outVal = curDateTime.Year;
            }
            else if (curType == DateTimeTypes.Month)
            {
                outVal = curDateTime.Month;
            }
            else if (curType == DateTimeTypes.Weekday)
            {
                outVal = Convert.ToDouble((int)curDateTime.DayOfWeek);
            }
            else if (curType == DateTimeTypes.Day)
            {
                outVal = curDateTime.Day;
            }

            return outVal;
        }

        //Function converting the values in the given column into categorical type, that is: assigning each value with a unique index (starting from zero)
        private Input updateCategorical(Input curInput)
        {
            curInput.conversionDict = new Dictionary<string, double>();

            double newCateg = -1.0;
            foreach (string item in curInput.vals2.Distinct().OrderBy(x => x))
            {
                newCateg = newCateg + 1.0;
                curInput.conversionDict.Add(item, newCateg);
            }

            for (int i = 0; i < curInput.vals2.Count; i++)
            {
                if (curInput.vals.Count - 1 < i) curInput.vals.Add(0.0);

                curInput.vals[i] = curInput.conversionDict[curInput.vals2[i]];
            }

            return curInput;
        }

        //Method called after the corresponding conversion (to categorical/DateTime) has been completed to perform some final actions
        private AllInputs lastActionsNewNonNumerical(AllInputs allInputs, int curCol, InputType newType)
        {
            if (allInputs.inputs[curCol].wrongRowsCount > 0)
            {
                allInputs.inputs[curCol].vals = new List<double>();
                allInputs.inputs[curCol].vals2 = new List<string>();
                allInputs.inputs[curCol].type.mainType = MainTypes.Blank;

                string nameToShow = "\"" + allInputs.inputs[curCol].displayedName + "\"";
                string typeToShow = newType.mainType.ToString();
                if (newType.mainType == MainTypes.DateTime)
                {
                    if (newType.secType == DateTimeTypes.Time)
                    {
                        typeToShow = typeToShow + " (expected format: hh:mm:ss or equivalent)";
                    }
                    else
                    {
                        typeToShow = typeToShow + " (expected format: dd-mm-yyyy or equivalent)";
                    }
                }
                MessageBox.Show("Some values in " + nameToShow + " cannot be converted into " + typeToShow + "." + Environment.NewLine + "This column will not be considered during the calculations.");
            }
            else
            {
                allInputs.inputs[curCol].type.mainType = newType.mainType;
                allInputs.inputs[curCol].type.secType = newType.secType;
            }

            return allInputs;
        }

        //Function called after the column updating process has finished to perform some actions in the definitive column
        public AllInputs finalColumnActions(AllInputs allInputs)
        {
            for (int i = 0; i < allInputs.inputs.Count; i++ )
            {
                if (allInputs.inputs[i].type.mainType != MainTypes.Blank)
                {
                    allInputs.inputs[i] = preAnalyseColumn(allInputs.inputs[i]);
                    allInputs.inputs[i].min = allInputs.inputs[i].vals.Min();
                    allInputs.inputs[i].max = allInputs.inputs[i].vals.Max();
                    allInputs.inputs[i].totDiffVals = allInputs.inputs[i].vals.Distinct().Count();
                }
            }

            return allInputs;
        }

        //Most of the (data) assessment is performed at a later stage (i.e., while analysing the given combination/fit).
        //This is a pre-analysis of the raw input information which will (slightly) be taken into account during the later assessment
        private Input preAnalyseColumn(Input curInput)
        {
            curInput.preAnalysis.averValue = curInput.vals.Average();
            curInput.preAnalysis.commonValsCount = curInput.vals.Count - curInput.vals.Distinct().Count() + 1;
            if (curInput.preAnalysis.commonValsCount == curInput.vals.Count)
            {
                curInput.type.mainType = MainTypes.Constant;
                return curInput;
            }
            curInput.preAnalysis.averVariabilityValue = curInput.vals.Select(x => Math.Abs(x - curInput.preAnalysis.averValue)).Average();

            //Preliminary rating of the given input (column) on account of the number of common values and the variability of its error
            curInput.preAnalysis.rating = 10;
            double ratio = (double)curInput.preAnalysis.commonValsCount / (double)curInput.vals.Count;
            if (curInput.preAnalysis.commonValsCount > 5 || ratio >= 0.05)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 1;

                if (ratio >= 0.7)
                {
                    curInput.preAnalysis.rating = 4;
                }
                else if (ratio >= 0.5)
                {
                    curInput.preAnalysis.rating = 5;
                }
                else if (ratio >= 0.35)
                {
                    curInput.preAnalysis.rating = 6;
                }
                else if (ratio >= 0.2)
                {
                    curInput.preAnalysis.rating = 7;
                }
            }

            double diffRatio = Common.errorCalcs(curInput.preAnalysis.averVariabilityValue, curInput.preAnalysis.averValue);
            if (diffRatio <= 0.05)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 5;
            }
            else if (diffRatio <= 0.1)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 4;
            }
            else if (diffRatio <= 0.2)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 3;
            }
            else if (diffRatio <= 0.3)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 2;
            }
            else if (diffRatio <= 0.5)
            {
                curInput.preAnalysis.rating = curInput.preAnalysis.rating - 1;
            }

            if (curInput.preAnalysis.rating < 0) curInput.preAnalysis.rating = 0;

            return curInput;
        }
    }
}

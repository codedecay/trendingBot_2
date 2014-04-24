using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace trendingBot2
{
    /// <summary>
    /// Class including some methods used by the interface ("mainForm" class). The main motivation of this class is reducing the size of the already-quite-big "mainForm.cs" file
    /// </summary>
    class GUI
    {
        mainForm mainForm; //Current instance of the mainForm class
        RichTextBox rtbCombinations; //RichTextBox showing all the information of the currently-selected solution (fit and dependent variable), which is profusely used in this class
        Font rtbSmall, rtbNormal; //Two different variations of the original rtbCombinations font
        
        //Class constructor populating the corresponding global variables
        public GUI(mainForm main, Font small, Font normal)
        {
            mainForm = main;
            rtbSmall = small;
            rtbNormal = normal;
            rtbCombinations = (RichTextBox)mainForm.Controls.Find("rtbCombinations", true)[0];
        }

        //Method called from mainForm.inputsToUpdate (in charge of determining whether some columns of values should be changed) to perform accessory actions.
        //It returns a list of the non-numerical columns (if any); that is: columns which have to be modified (e.g., categorical columns) or ones which have to be
        //ignored (e.g., blank/constant ones)
        public List<Input> inputsToUpdate2(AllInputs inputCols)
        {
            string blankMessage = "";
            string constantMessage = "";
            List<Input> nonNumList = new List<Input>();

            //Loop iterating through all the columns ("Input") to determine whether there is any suitable one (i.e, non-numerical)
            for (int i = inputCols.inputs.Count - 1; i >= 0; i--)
            {
                if (inputCols.inputs[i].type.mainType == MainTypes.Blank)
                {
                    blankMessage = inputCols.inputs[i].displayedName + ", " + blankMessage;
                    inputCols.inputs.RemoveAt(i);
                }
                else if (inputCols.inputs[i].type.mainType == MainTypes.Constant)
                {
                    constantMessage = inputCols.inputs[i].displayedName + ", " + constantMessage;
                    inputCols.inputs.RemoveAt(i);
                }
                else if (inputCols.inputs[i].type.mainType == MainTypes.Categorical || inputCols.inputs[i].type.mainType == MainTypes.DateTime)
                {
                    nonNumList.Add(inputCols.inputs[i]);
                }
            }

            //Showing a message in case of having ignored columns (e.g., blank or constant)
            showNonNumericalMessage(blankMessage, constantMessage);

            return nonNumList;
        }

        //Method called by the one above to show a MessageBox listing the columns being ignored (if any)
        private void showNonNumericalMessage(string blankMessage, string constantMessage)
        {
            string curMessage = "";

            if (blankMessage != "")
            {
                blankMessage = blankMessage.Trim();
                blankMessage = blankMessage.Substring(0, blankMessage.Length - 1);
                blankMessage = "(blank) " + blankMessage;
            }
            if (constantMessage != "")
            {
                constantMessage = constantMessage.Trim();
                constantMessage = constantMessage.Substring(0, constantMessage.Length - 1);
                constantMessage = "(constant) " + constantMessage;
            }

            if (blankMessage != "" || constantMessage != "")
            {
                curMessage = "The following columns will not be considered during the calculations: " + blankMessage;
                if (constantMessage != "") curMessage = curMessage + constantMessage;

                curMessage = curMessage + ".";
                MessageBox.Show(curMessage);
            }
        }

        //Method called every time a new solution is selected from the corresponding list in the GUI to delete all the NumericUpDown (and associated Label) controls
        public void deleteInputNumUpDown(Panel pnlPredInputs)
        {
            for (int i = pnlPredInputs.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlPredInputs.Controls[i].GetType() == typeof(NumericUpDown) && pnlPredInputs.Controls[i].Name != "numPredInput0")
                {
                    pnlPredInputs.Controls.RemoveAt(i);
                }
                else if (pnlPredInputs.Controls[i].GetType() == typeof(Label) && pnlPredInputs.Controls[i].Name != "lblPredInp0")
                {
                    pnlPredInputs.Controls.RemoveAt(i);
                }
                else
                {
                    pnlPredInputs.Controls[i].Visible = false;
                }
            }
        }

        //Method called every time the user selects a solution in the ListBox to update the equation displayed in the corresponding RTB (rtbCombinations)
        public void writeEquationInRTB(ValidCombination curValidComb)
        {
            rtbCombinations.Text = "";
            rtbText(curValidComb.independentVar.input.displayedName);
            rtbCombinations.AppendText(" = ");

            bool additionPending = false;
            if (curValidComb.coeffs.A != 0.0 || (curValidComb.coeffs.B == 0.0 && curValidComb.coeffs.C == 0.0))
            {
                additionPending = writeCoeffInRTB(curValidComb, curValidComb.coeffs.A, additionPending);
            }
            if (curValidComb.coeffs.B != 0.0)
            {
                additionPending = writeCoeffInRTB(curValidComb, curValidComb.coeffs.B, additionPending);
                rtbOperation(Operation.Multiplication);
                rtbText("VAR");
            }
            if (curValidComb.coeffs.C != 0.0)
            {
                additionPending = writeCoeffInRTB(curValidComb, curValidComb.coeffs.C, additionPending);
                rtbOperation(Operation.Multiplication);
                rtbSuperIndices(2, true, "VAR");
            }
        }

        //Function called from the method writeEquationInRTB to write the corresponding equation coefficient (A, B, C) in rtbCombinations 
        private bool writeCoeffInRTB(ValidCombination curValidComb, double curCoeff, bool additionPending)
        {
            double curVal = curCoeff;
            if (additionPending)
            {
                Operation curOperation = Operation.Addition;
                if (curCoeff < 0)
                {
                    curOperation = Operation.Subtraction;
                    curVal = Math.Abs(curVal);
                }
                rtbOperation(curOperation);
                additionPending = false;
            }
            rtbNumbers(curVal);
            additionPending = true;

            return additionPending;
        }

        //When the fit is not constant, the corresponding dependent variable (combination of variables, exponents & operations) has also to be displayed.
        //This is what this method takes care of: displaying the dependent variable by following the expected format
        public List<VarValue> writeVarInRTB(Combination curComb)
        {
            rtbCombinations.AppendText(Environment.NewLine);
            rtbCombinations.AppendText(Environment.NewLine);
            rtbText("Where: ");

            bool noExponents = true;
            int i = -1;
            while (i < curComb.items.Count - 1 && noExponents)
            {
                i = i + 1;
                if (curComb.items[i].exponent != 1.0 && curComb.items[i].exponent > -9999) noExponents = false;
            }

            if (noExponents) rtbCombinations.AppendText(Environment.NewLine); //Additional space to avoid when both lines are too close (= all the variables have 1.0 as exponent)

            rtbCombinations.AppendText(Environment.NewLine);
            rtbText("VAR = ");

            List<VarValue> curVarVals = new List<VarValue>();

            //Loop iterating through all the elements of the given combination and performing the corresponding actions (e.g., updating the text in rtbCombinations or setting as many NumericUpDowns as dependent variables)
            for (i = 0; i < curComb.items.Count; i++)
            {
                rtbSuperIndices(curComb.items[i].exponent, true, curComb.items[i].variable.input.displayedName);
                if (i < curComb.items.Count - 1) rtbOperation(curComb.items[i].operation);

                while (curVarVals.Count - 1 < curComb.items[i].variable.index)
                {
                    curVarVals.Add(new VarValue() { variable = new Variable(), value = 0.0 });
                }
                curVarVals.Insert(curComb.items[i].variable.index, new VarValue() { variable = curComb.items[i].variable, value = curComb.items[i].variable.input.min });

                mainForm.setInputNumUpDown(i, curComb.items[i].variable); //Setting up the corresponding NumericUpDowns and Labels
            }

            return curVarVals;
        }

        //Method adding super-indices (i.e., exponents) to rtbCombinations.
        //It deals also with logarithms (stored as exponents of the given variable)
        private void rtbSuperIndices(double exponent, bool doubles, string varName)
        {
            string exponentString = "";
            double logVal = Common.isLogarithm(exponent); //Logarithms are stored also as exponents
            if (logVal != 0.0)
            {
                string logValString = logVal == 10 ? "10": "";

                if (logValString != "")
                {
                    rtbText("Log");
                    rtbText(logValString);
                    rtbCombinations.SelectionStart = rtbCombinations.Text.Length - logValString.Length;
                    rtbCombinations.SelectionLength = logValString.Length;
                    rtbCombinations.SelectionCharOffset = -3;
                    rtbCombinations.SelectionFont = rtbSmall;
                    rtbText(varName);
                }
            }
            else
            {
                rtbText(varName);
                if (exponent != 1)
                {
                    if (doubles) exponentString = exponent.ToString("0.0");
                    else exponentString = exponent.ToString("0");
                }

                if (exponentString != "")
                {
                    if (exponent == 0.0) exponentString = "";
                    rtbCombinations.AppendText(exponentString);
                    rtbCombinations.SelectionStart = rtbCombinations.Text.Length - exponentString.Length;
                    rtbCombinations.SelectionLength = exponentString.Length;
                    rtbCombinations.SelectionCharOffset = 7;
                    rtbCombinations.SelectionFont = rtbSmall;
                }
            }
        }

        //Method adding the symbol associated with the given operation to rtbCombinations
        private void rtbOperation(Operation curOperation)
        {
            string curSign = " " + Common.getSignFromoperation(curOperation, false) + " ";
            rtbText(curSign);
        }

        //Method adding a number (not exponent) to rtbCombinations
        private void rtbNumbers(double curVal)
        {
            string valString = numberToDisplay(curVal);
            rtbText(valString);
        }

        //Method adding simple text to rtbCombinations: a variable name, operation symbol or even a number
        private void rtbText(string varName)
        {
            rtbCombinations.AppendText(varName);
            rtbCombinations.SelectionStart = rtbCombinations.Text.Length - varName.Length;
            rtbCombinations.SelectionLength = varName.Length;
            rtbCombinations.SelectionCharOffset = 0;
            rtbCombinations.SelectionFont = rtbNormal;
        }

        //Function converting the input number into string by applying some rules making sure that it will look properly in the interface
        public string numberToDisplay(double curVal)
        {
            string outString = "";

            if (Math.Abs(curVal) >= 10000.0 || (Math.Abs(curVal) <= 0.0001 && curVal != 0.0))
            {
                outString = curVal.ToString("E2", Common.curCulture); //Scientific notation for numbers consisting in many digits
            }
            else
            {
                string curPrecision = "N4";
                if (curVal == 0 || Math.Abs(curVal) > 1000) curPrecision = "N2";
                outString = curVal.ToString(curPrecision, Common.curCulture);
            }

            return outString;
        }
    }
}

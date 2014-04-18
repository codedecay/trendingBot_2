using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace trendingBot2
{
    public partial class mainForm : Form
    {
        //Most of the global variables used by this class are stored in the Tag properties of various controls
        string allIndItem;
        int smallHeight, bigHeight;
        Point mainFormIni;
        GUI curGUI;
        
        public mainForm()
        {
            InitializeComponent();
        }

#region mainForm

        //Method called at form load and performing some initial-population actions
        private void mainForm_Load(object sender, EventArgs e)
        {
            //-- Default fonts
            Font rtbSmall = new Font(rtbCombinations.Font.FontFamily, rtbCombinations.Font.Size - 1);
            Font rtbNormal = rtbCombinations.Font;

            dgvInputs.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            dgvInputs.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            //--------------

            Common.curCulture = new CultureInfo("en-GB");
            Common.curCulture.NumberFormat.NumberGroupSeparator = "";
            Common.curCulture.NumberFormat.PositiveSign = "";

            smallHeight = 377;
            bigHeight = 765;
            mainFormIni = this.Location;

            allIndItem = "Find the best options";

            numPredInput0.ValueChanged += new System.EventHandler(numPredInput_ValueChanged);
            numPredInput0.Tag = new Variable();
            this.Tag = new ValidCombination();
            lblPredValOutput.Tag = new List<VarValue>();
            dgvInputs.Tag = new List<Input>();
            cmbBxAccuracy.Tag = new FitConfig() { expectedAccuracy = Accuracy.High };

            cmbBxAccuracy.SelectedIndex = 0;

            curGUI = new GUI(this, rtbSmall, rtbNormal); //Instance of the "GUI" class which will be used during the whole execution. Note that this class was created to reduce the size of the current file

            startUpdate(); //Reading the input file and populating all the corresponding controls accordingly
        }

        //Method triggered every time the form is active.
        //It is only used when the popUp (to allow the user to modify the non-numerical columns) is opened, to force it to always be on top of the main form.
        //Note that this approach (i.e., forcing the popUp to not be selected) is required because the popUp is instantiated as a non-modal form (in order to minimise the limitations associated with the modal alternative)
        private void mainForm_Activated(object sender, EventArgs e)
        {
            if (Modifications.curPopUp != null)
            {
                Modifications.curPopUp.BringToFront();
            }

            if (cmbBxAccuracy.Tag != null)
            {
                closeFitConfigPopUp();
            }
        }

        //Method triggered every time the size of the form is changed to relocate it accordingly to the current size (such that it is always shown in a centered location)
        private void mainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.Height == bigHeight)
            {
                this.Location = new Point(mainFormIni.X, mainFormIni.Y - (bigHeight - smallHeight) / 2);
            }
            else
            {
                this.Location = mainFormIni;
            }
        }

        //Method called to vary the height of the main form, for the transition "calculating" (small) -> "showing results" (big)
        private void resizeMainForm(bool makeBig)
        {
            this.Height = makeBig ? bigHeight : smallHeight;
        }

#endregion

#region bgwMain

#region Events

        //Method called when bgwMain is started. It has to account for different situations because bgwMain is used to perform different tasks
        private void bgwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = null;
            if (e.Argument == null)
            {
                //Called to read the input file
                IO curIO = new IO();
                AllInputs curInputs = curIO.readInputs();
                e.Result = curInputs;
            }
            else if (e.Argument.GetType() == typeof(Modifications))
            {
                //Called to modify the non-numerical columns
                Modifications curModif = (Modifications)e.Argument;
                while (!curModif.completed)
                {
                    //Waiting for the user to specify the type for the non-numerical columns
                }
                e.Result = curModif.allInputs0;
            }
            else if (e.Argument.GetType() == typeof(List<Input>))
            {
                //Called to start the calculations
                List<Input> curInputs = (List<Input>)e.Argument;
                if (curInputs.Count > 0)
                {
                    MainCalcs curCalcs = new MainCalcs(bgwMain);
                    ComboItem curItem = (ComboItem)cmbBxIndependent.Tag;
                    int indepIndex = curItem.selectedIndex;
                    if (curItem.selectedItem == allIndItem) indepIndex = -1;
                    Results curResults = curCalcs.startCalcs(curInputs, indepIndex, (FitConfig)cmbBxAccuracy.Tag);

                    curResults.sw = (Stopwatch)btnRun.Tag;
                    curResults.sw.Stop();
                    curResults.totTime = curResults.sw.Elapsed.Hours.ToString("00") + ":" + curResults.sw.Elapsed.Minutes.ToString("00") + ":" + curResults.sw.Elapsed.Seconds.ToString("00");
                    if (curResults.sw.Elapsed.Hours == 0 && curResults.sw.Elapsed.Minutes == 0) curResults.totTime = "< 1 minute";

                    IO curIO = new IO();
                    curIO.writeOutputs(curResults);
                    e.Result = curResults;
                }
            }
        }

        //Method called when bgwMain finishes the given task. It has to account for the different situations where bgwMain is used
        private void bgwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRun.Enabled = true;
            if (e.Result != null)
            {
                if (e.Result.GetType() == typeof(AllInputs))
                {
                    //The input reading process has been completed
                    AllInputs curInputs = (AllInputs)e.Result;
                    bgwCompleteUpdate(curInputs); //Method performing the corresponding actions on account of the type of inputs
                }
                else if (e.Result.GetType() == typeof(Results))
                {
                    //The calculations have been completed
                    Results curResults = (Results)e.Result;
                    lblReliability.ForeColor = getAccuracyColor(curResults.config.fitConfig.expectedAccuracy);
                    lblReliability.Refresh();
                    populateCombinatoricsLstBx(curResults.combinations, "Total time: " + curResults.totTime); //Populating the main ListBox with all the valid solutions (if any)
                    enableControls(true, true); //Enable/disable some controls
                }
            }
        }

        //Method called every time bgwMain.ReportProgress is executed. It is used to update lblStatus without triggering a "cross thread exception"
        private void bgwMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Text = e.UserState.ToString();
            lblStatus.Refresh();
        }

#endregion

#region Associated methods

        //Method called after the input-file reading has been completed to update the information in the required variables/GUI-controls accordingly 
        private void bgwCompleteUpdate(AllInputs curInputs)
        {
            if (curInputs.inputs.Count > 0)
            {
                bool updateNonNum = true;
                if (curInputs.updateCompleted || curInputs.inputs.FirstOrDefault(x => x.type.mainType == MainTypes.NonNumerical) == null)
                {
                    updateNonNum = false;
                }

                Modifications curModif = new Modifications(this);
                if (updateNonNum)
                {
                    //Non-numerical columns are present and thus some additional actions have to be carried out
                    curModif.updateColumns(curInputs, curInputs.inputs.Count);
                    bgwMain.RunWorkerAsync(curModif); //To wait while the user performs the corresponding actions
                }
                else
                {
                    //There is no non-numerical columns or the ones present have already been corrected
                    curInputs.updateCompleted = true;
                    curInputs = curModif.finalColumnActions(curInputs); //Analysis for all the valid columns
                    updateControlsWithInputs(curInputs); //Population of the corresponding controls and eventual display of error messages
                }
            }
            else
            {
                //No valid inputs
                enableControls(false, false);
                btnUpdate.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        //Updating the controls with the information from the inputs
        public void updateControlsWithInputs(AllInputs curInputs)
        {
            bool calculate = true;

            if (curInputs.maxRowConsidered > 0) MessageBox.Show("Only the first " + curInputs.maxRowConsidered.ToString() + " rows will be considered during the calculations.");

            inputsToUpdate(curInputs); //Valid non-numerical columns
            if (curInputs.inputs.Count <= 1)
            {
                MessageBox.Show("No valid inputs were found.");
            }
            else
            {
                populateInputDGV(curInputs); //DGV population
                populateVarsCombo(curInputs); //Independent variable comboBox population
                calculate = false;
            }

            enableControls(true, calculate);
        }

        //Method called in case that some (valid) non-numerical columns are present. It populates the controls in charge of displaying the performed conversions 
        private void inputsToUpdate(AllInputs inputCols)
        {
            cmbBxNonNumerical.Items.Clear();
            cmbBxNonNumerical.Tag = new List<Input>();

            List<Input> nonNumList = curGUI.inputsToUpdate2(inputCols);
            bool visible = false;
            if (nonNumList.Count > 0)
            {
                for (int i = nonNumList.Count - 1; i >= 0; i--)
                {
                    cmbBxNonNumerical.Items.Add(nonNumList[i].displayedName);
                    ((List<Input>)cmbBxNonNumerical.Tag).Add(nonNumList[i]);
                }
                visible = true;
                cmbBxNonNumerical.SelectedIndex = 0;
            }

            cmbBxNonNumerical.Visible = visible;
            lstVwNonNumerical.Visible = visible;
            lblNonNumerical1.Visible = visible;
            lblNonNumerical2.Visible = visible;
        }

        //Method populating the main DGV with all the (eventually-corrected) inputs
        private void populateInputDGV(AllInputs inputCols)
        {
            dgvInputs.Columns.Clear();

            dgvInputs.Tag = inputCols.inputs;
            //Adding as many columns as input variables to the DGV
            foreach (Input col in inputCols.inputs)
            {
                DataGridViewColumn curDGVCol = new DataGridViewTextBoxColumn();
                curDGVCol.HeaderText = col.displayedIndex.ToString() + ". " + col.name;
                curDGVCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                curDGVCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvInputs.Columns.Add(curDGVCol);
            }

            //Adding all the values
            for (int row = 0; row < inputCols.inputs[0].vals.Count; row++)
            {
                dgvInputs.Rows.Add();
                for (int col = 0; col < inputCols.inputs.Count; col++)
                {
                    dgvInputs[col, row].Value = inputCols.inputs[col].vals[row];
                }
            }
        }

        //Method called to populate the cmbBxIndependent (where the user selects the independent variable to be considered) with all the input columns
        private void populateVarsCombo(AllInputs allInputs)
        {
            if (cmbBxIndependent.Items.Count > 0) cmbBxIndependent.Items.Clear();
            foreach (Input col in allInputs.inputs)
            {
                cmbBxIndependent.Items.Add(Common.getDisplayedName(col.displayedIndex, col.name, false, true));
            }
            cmbBxIndependent.Items.Add(allIndItem);
            cmbBxIndependent.SelectedIndex = 0;
        }

        //Method called to populate the predictions ListBox with the list of valid solutions (i.e., "Solution No. " + counter)
        private void populateCombinatoricsLstBx(List<ValidCombination> combinations, string totTime)
        {
            if (combinations.Count > 0)
            {
                MessageBox.Show("Calculations completed. Some valid solutions have been found." + Environment.NewLine + totTime);

                resizeMainForm(true);

                lstBxCombinations.Items.Clear();

                lstBxCombinations.Tag = combinations;
                int count = 0;
                foreach (ValidCombination comb in combinations)
                {
                    count = count + 1;
                    lstBxCombinations.Items.Add("Solution No. " + count.ToString());
                }

                lstBxCombinations.SelectedIndex = 0;
            }
            else
            {
                //No valid trend was found
                MessageBox.Show("Calculations completed. No solution under the input conditions has been found." + Environment.NewLine + totTime);
            }
        }

#endregion

#endregion

#region btnRun

        //Method associated with the Click event of btnRun
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (btnRun.Text == "Run")
            {
                startCalcs();
            }
            else
            {
                stopCalcs();
            }
        }

        //Method called to start the calculations (i.e., when btnRun displays "Run")
        private void startCalcs()
        {
            Stopwatch curSw = new Stopwatch();
            curSw.Start();
            btnRun.Tag = curSw; //The Stopwatch variable, which will stored the calculation time 

            resizeMainForm(false);

            enableControls(false, true);
            bgwMain.RunWorkerAsync((List<Input>)dgvInputs.Tag);
        }

        //Method called to stop the calculations (i.e., when btnRun displays "Stop")
        private void stopCalcs()
        {
            btnRun.Enabled = false;
            Combinatorics.cancelSim = true;
            lblStatus.Text = "Stopping...";
            lblStatus.Refresh();
            bgwMain.CancelAsync();
        }

#endregion

#region btnUpdate

        //Method associated with the Click event of btnUpdate. It starts the update process (i.e., reading the inputs file)
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            startUpdate();
        }

        //Method reading the inputs file and performing all the required actions in the associated controls
        private void startUpdate()
        {
            resizeMainForm(false);
            enableControls(false, false);
            lblStatus.Text = "Analysing the \"inputs.csv\" file";
            lblStatus.Refresh();
            bgwMain.RunWorkerAsync(null);
        }

#endregion

#region cmbBxAccuracy

        //SelectedIndexChanged event associated with cmbBxAccuracy. It creates the associated FitConfig variable (to be used during the calculations) and also sets up the info-displaying popup
        private void cmbBxAccuracy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Accuracy curAccuracy = (Accuracy)Enum.Parse(typeof(Accuracy), cmbBxAccuracy.SelectedItem.ToString());
            cmbBxAccuracy.Tag = getAssociatedFitConfig(curAccuracy);
            setDisplayInfo(cmbBxAccuracy.Tag);
        }

        //Method returning the FitConfig instance (i.e., including the basic rules determining when a fit is valid) associated with the given input conditions
        private FitConfig getAssociatedFitConfig(Accuracy expectedAccuracy)
        {
            FitConfig curFitConfig = new FitConfig();

            if (expectedAccuracy == Accuracy.High)
            {
                curFitConfig.averLimit = 0.05;
                curFitConfig.minPercBelowLimit = 0.85;
                curFitConfig.globalAver = 0.10;
                curFitConfig.minNoCases = 50;
            }
            else if (expectedAccuracy == Accuracy.Medium)
            {
                curFitConfig.averLimit = 0.15;
                curFitConfig.minPercBelowLimit = 0.8;
                curFitConfig.globalAver = 0.25;
                curFitConfig.minNoCases = 35;
            }
            else if (expectedAccuracy == Accuracy.Low)
            {
                curFitConfig.averLimit = 0.3;
                curFitConfig.minPercBelowLimit = 0.7;
                curFitConfig.globalAver = 0.35;
                curFitConfig.minNoCases = 20;
            }
            curFitConfig.expectedAccuracy = expectedAccuracy;

            return curFitConfig;
        }

        //Method triggered by the MouseEnter event of cmbBxAccuracy. It sets up the popup displaying all the information about the selected item (Accuracy)
        private void cmbBxAccuracy_MouseEnter(object sender, EventArgs e)
        {
            setDisplayInfo(cmbBxAccuracy.Tag);
        }

        //Method triggered by the MouseLeave event of cmbBxAccuracy. It closes the popup created by the method above
        private void cmbBxAccuracy_MouseLeave(object sender, EventArgs e)
        {
            closeFitConfigPopUp();
        }

        //Called by the method above to close the opened popup displaying information about the selected accuracy level
        private void closeFitConfigPopUp()
        {
            if (lblAccuracy.Tag != null && lblAccuracy.Tag.GetType() == typeof(Form))
            {
                ((Form)lblAccuracy.Tag).Close();
                lblAccuracy.Tag = null;
            }
        }

        //Method creating a ListView including all the required information about the currently selected accuracy level.
        //This ListView will be added to the popup form created right before calling this function
        private ListView createFitConfigLstVw(int curHeight, FitConfig curFitConfig)
        {
            ListView curLstVw = new ListView();
            curLstVw.Width = 250;
            curLstVw.Height = curHeight;
            curLstVw.View = View.Details;

            curLstVw.Columns.Add("Factor to define a valid trend").Width = 150;
            curLstVw.Columns.Add("Value").TextAlign = HorizontalAlignment.Center;

            ListViewItem curItem = new ListViewItem();
            curItem.Text = "Target error/case (%)";
            curItem.SubItems.Add((curFitConfig.averLimit * 100).ToString("N0", Common.curCulture));
            curLstVw.Items.Add(curItem);
            curItem = new ListViewItem();
            curItem.Text = "Min. cases below target (%)";
            curItem.SubItems.Add((curFitConfig.minPercBelowLimit * 100).ToString("N0", Common.curCulture));
            curLstVw.Items.Add(curItem);
            curItem = new ListViewItem();
            curItem.Text = "Max. aver. global error (%)";
            curItem.SubItems.Add((curFitConfig.globalAver * 100).ToString("N0", Common.curCulture));
            curLstVw.Items.Add(curItem);
            curItem = new ListViewItem();
            curItem.Text = "Min. number of cases";
            curItem.SubItems.Add(curFitConfig.minNoCases.ToString("N0", Common.curCulture));

            curLstVw.Items.Add(curItem);

            return curLstVw;
        }

#endregion

#region lblReliability

        //Triggered by the MouseEnter event of lblReliability. It sets up the popup showing detailed information about the assessment factors
        private void lblReliability_MouseEnter(object sender, EventArgs e)
        {
            setDisplayInfo(this.Tag);
        }

        //MouseLeave event of lblReliability closing the popup opened by the method above
        private void lblReliability_MouseLeave(object sender, EventArgs e)
        {
            if (lblReliability.Tag != null && lblReliability.Tag.GetType() == typeof(Form))
            {
                ((Form)lblReliability.Tag).Close();
                lblReliability.Tag = null;
            }
        }

        //Method called to create and populate the ListView to be included in the popup displaying further information about the current reliability (lblReliability)
        private ListView createReliabilityLstVw(int curHeight, ValidCombination curValidComb)
        {
            ListView curLstVw = new ListView();
            curLstVw.Width = 299;
            curLstVw.Height = curHeight;
            curLstVw.View = View.Details;

            curLstVw.Columns.Add("Name").Width = 175;

            curLstVw.Columns.Add("Weight").TextAlign = HorizontalAlignment.Center;
            curLstVw.Columns.Add("Rating").TextAlign = HorizontalAlignment.Center;
            foreach (var factor in curValidComb.assessment.factors)
            {
                ListViewItem curItem = new ListViewItem();
                curItem.Text = factor.name;
                curItem.SubItems.Add(factor.weight.ToString("0.00"));
                curItem.SubItems.Add(factor.rating.ToString("0.00"));
                curLstVw.Items.Add(curItem);
            }

            if (curValidComb.assessment.factors.Count == 1)
            {
                curLstVw.Items.Add("PERFECT MATCH");
            }

            return curLstVw;
        }

#endregion

#region NumericUpDowns for prediction inputs

        //Method associated with the ValueChanged event of all the NumericUpDowns included in pnlPredInputs (inputs for the predictions)
        public void numPredInput_ValueChanged(object sender, EventArgs e)
        {
            performCalcs((NumericUpDown)sender);
        }

        //Method called every time the value of any NumericUpDown in pnlPredInputs changes, to perform the corresponding prediction (i.e., apply the given formula to the input values)
        private void performCalcs(NumericUpDown curNum)
        {
            if (((List<Input>)dgvInputs.Tag).Count > 0 && ((ValidCombination)this.Tag).errors.Count > 0)
            {
                chckBxExtrapol.Enabled = true;
                ValidCombination curValidComb = (ValidCombination)this.Tag;
                List<VarValue> curVarVals = (List<VarValue>)lblPredValOutput.Tag;
                Variable curVariable = (Variable)curNum.Tag;
                bool isConstant = curVariable == null ? true : false; //In the current version a constant fit is virtually impossible (i.e., constant inputs are removed)
                if (isConstant || ((List<VarValue>)lblPredValOutput.Tag).Count - 1 >= curVariable.index)
                {
                    double curX = 0.0;
                    if (!isConstant)
                    {
                        ((List<VarValue>)lblPredValOutput.Tag)[curVariable.index].value = (double)curNum.Value;
                        List<double> curVals = new List<double>();
                        foreach (VarValue curVarVal in curVarVals)
                        {
                            if (curVarVal.variable.input.name != null) curVals.Add(curVarVal.value);
                        }

                        curX = Common.calculateXValue(curValidComb.dependentVars, (List<Input>)dgvInputs.Tag, curVals).value;
                    }
                    else
                    {
                        chckBxExtrapol.Enabled = false;
                    }

                    lblPredTitOutput.Text = curValidComb.independentVar.input.displayedShortName + ":";
                    lblPredValOutput.Text = curGUI.numberToDisplay(Common.valueFromPol(curValidComb.coeffs, curX));
                    lblPredValOutput.Refresh();
                    lblReliability.Text = "Reliability (0-10): " + curValidComb.assessment.globalRating.ToString("N2", Common.curCulture);
                    lblAverError.Text = "Average error: " + curGUI.numberToDisplay(curValidComb.averError * 100) + "%";
                    lblAverError.Refresh();
                }

                //Updating the font color of the given NumericUpDown in case that extrapolation is allowed and the given value is outside the training set boundaries
                if (curNum.Value > (decimal)curVariable.input.max || curNum.Value < (decimal)curVariable.input.min)
                {
                    curNum.ForeColor = Color.Red;
                }
                else
                {
                    curNum.ForeColor = Color.Black;
                }
            }
        }

#endregion

#region cmbBxNonNumerical

        //Method triggered by the SelectedIndexChanged event of cmbBxNonNumerical.
        //This comboBox is shown when (valid) non-numerical columns are present (i.e., categorical or date-time) to show the equivalences to the user.
        //More specifically, this method updates the corresponding ListView with the (conversion) values associated with the selected item (i.e., non-numerical column)
        private void cmbBxNonNumerical_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstVwNonNumerical.Items.Clear();
            if (cmbBxNonNumerical.Tag != null)
            {
                Input curInput = ((List<Input>)cmbBxNonNumerical.Tag)[cmbBxNonNumerical.SelectedIndex];
                if (curInput.conversionDict.Count > 0)
                {
                    foreach (var dictItem in curInput.conversionDict)
                    {
                        ListViewItem lstVwItem = new ListViewItem();
                        lstVwItem.SubItems.Add(dictItem.Key.ToString());
                        lstVwItem.SubItems.Add(dictItem.Value.ToString());

                        lstVwNonNumerical.Items.Add(lstVwItem);
                    }
                }

                operationFromMainSecType(curInput);
            }
        }

        //Method called by the one above to update the associated Label (lblNonNumerical2) with the corresponding message, explaining the performed conversion
        private void operationFromMainSecType(Input curInput)
        {
            if (curInput.type.mainType == MainTypes.Categorical)
            {
                lblNonNumerical2.Text = "Conversion [categorical]: unique index (" + curInput.min.ToString() + "-" + curInput.max.ToString() + ").";
            }
            else if (curInput.type.mainType == MainTypes.DateTime)
            {
                if (curInput.type.secType == DateTimeTypes.Time)
                {
                    lblNonNumerical2.Text = "Conversion [time]: 10000 " + Common.getSignFromoperation(Operation.Multiplication, false) + " hours + 100 " + Common.getSignFromoperation(Operation.Multiplication, false) + " minutes + seconds.";
                }
                else
                {
                    lblNonNumerical2.Text = "Conversion [date]: corresponding " + curInput.type.secType.ToString().ToLower() + ".";
                }
            }
        }

#endregion

#region lstBxCombinations

        //SelectedIndexChanged event of lstBxCombinations updating all the information-displaying & input-values controls (including performing the corresponding calculations) on account of the selected item
        private void lstBxCombinations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBxCombinations.Tag != null)
            {
                ValidCombination curValidComb = ((List<ValidCombination>)lstBxCombinations.Tag)[lstBxCombinations.SelectedIndex];
                this.Tag = curValidComb;
                Combination curComb = curValidComb.dependentVars;
                curGUI.writeEquationInRTB(curValidComb); //Writing the main equations to the RTB with the adequate format

                curGUI.deleteInputNumUpDown(pnlPredInputs); //Deleting NumericUpDowns & Labels from previous seletions
                List<VarValue> curVarVals = new List<VarValue>();
                lblPredValOutput.Tag = new List<VarValue>();

                bool isConstant = curValidComb.coeffs.B == 0 && curValidComb.coeffs.C == 0;
                if (!isConstant)
                {
                    //The solution is not a constant and thus all the variables forming the dependent combinations have to be accounted for
                    curVarVals = curGUI.writeVarInRTB(curComb);
                }

                lblPredValOutput.Tag = curVarVals;
                numPredInput0.Tag = curComb.items.Count > 0 ? curComb.items[0].variable : null;

                //After updating all the controls, the calculations are performed
                performCalcs(numPredInput0); 
            }
        }
        
#endregion

#region cmbBxIndependent

        //SelectedIndexChanged of cmbBxIndependent. It just stores the currently-selected information into a variable which can be accessed without any problem from the BGW thread
        private void cmbBxIndependent_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBxIndependent.Tag = new ComboItem() { selectedIndex = cmbBxIndependent.SelectedIndex, selectedItem = cmbBxIndependent.SelectedItem.ToString() };
        }

#endregion

#region chckBxExtrapol

        //Method associated with the CheckedChanged event of chckBxExtrapol.
        //It updates the min./max. values of all the NumericUpDowns accordingly, that is: min./max. of the corresponding input variable (unchecked) or any value (checked)
        private void chckBxExtrapol_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in pnlPredInputs.Controls)
            {
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    updateMinMaxInputNum((NumericUpDown)ctrl);
                }
            }
        }

#endregion

#region Miscellaneous methods

        //Method called every time the Backgroundworker is used (i.e., calculations or inputs reading) to update certain controls
        private void enableControls(bool enabled, bool calculate)
        {
            cmbBxIndependent.Enabled = enabled;
            cmbBxAccuracy.Enabled = enabled;
            btnUpdate.Enabled = enabled;
            pnlPredictions.Visible = enabled;
            lstBxCombinations.Enabled = enabled;
            cmbBxNonNumerical.Enabled = enabled;
            lstVwNonNumerical.Enabled = enabled;

            if (calculate) btnRun.Text = enabled ? "Run" : "Stop";
            btnRun.Enabled = true;

            lblStatus.Text = "";
            lblStatus.Refresh();

            if (!enabled)
            {
                this.Cursor = Cursors.WaitCursor;
                if (!calculate) btnRun.Enabled = false;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        //Method called every time a new numericUpDown (+ associated Label) has to be added to pnlPredInputs
        public void setInputNumUpDown(int count, Variable curVariable)
        {
            NumericUpDown curNum = new NumericUpDown();
            Label curLabel = new Label();
            if (count == 0)
            {
                curNum = numPredInput0;
                curNum.Visible = true;
                curLabel = lblPredInp0;
                curLabel.Visible = true;
            }
            else
            {
                //---- New numUpDown
                curNum.Name = "numPredInput" + count.ToString();
                curNum.TextAlign = numPredInput0.TextAlign;
                curNum.Width = numPredInput0.Width;
                curNum.Font = numPredInput0.Font;
                curNum.Cursor = numPredInput0.Cursor;
                curNum.ValueChanged += new System.EventHandler(numPredInput_ValueChanged);
                pnlPredInputs.Controls.Add(curNum);
                curNum.Location = new Point(numPredInput0.Location.X, numPredInput0.Location.Y + 50 * count);
                //-----

                //---- New Label
                curLabel.Name = "lblPredInp" + count.ToString();
                curLabel.Font = lblPredInp0.Font;
                curLabel.ForeColor = lblPredInp0.ForeColor;
                pnlPredInputs.Controls.Add(curLabel);
                curLabel.Location = new Point(lblPredInp0.Location.X, lblPredInp0.Location.Y + 50 * count);
                //----
            }

            curNum.Tag = curVariable;
            updateMinMaxInputNum(curNum);

            curLabel.Text = curVariable.input.displayedShortName + ":";
            curLabel.Refresh();
        }

        //Method performing the whole "additional info displaying" process: setting up both, the popup and the ListView 
        private void setDisplayInfo(object curObj)
        {
            Form curPopUp = null;
            int curHeight = 0;
            if (curObj.GetType() == typeof(ValidCombination))
            {
                //Reliability information (lblReliability)
                ValidCombination curValidComb = (ValidCombination)curObj;

                //The assessment factors can be changed relatively easy and that's why this height comes from the formula below
                curHeight = 75;
                if (curValidComb.assessment.factors.Count > 2) curHeight = curHeight + 14 * (curValidComb.assessment.factors.Count - 2);

                if (lblReliability.Tag == null)
                {
                    //Creating a new popup
                    curPopUp = createDisplayInfoPopUp(curHeight, 299, lblReliability); 
                }
                else
                {
                    //Popup is being currently displayed
                    curPopUp = (Form)lblReliability.Tag;
                    curPopUp.Controls.Clear();
                }

                ListView curLstVw = createReliabilityLstVw(curHeight, curValidComb);
                curPopUp.Controls.Add(curLstVw);

                lblReliability.Tag = curPopUp;
            }
            else if (curObj.GetType() == typeof(FitConfig))
            {
                //Accuracy information (cmbBxAccuracy)
                FitConfig curFitConfig = (FitConfig)cmbBxAccuracy.Tag;

                curHeight = 100;
                if (lblReliability.Tag == null)
                {
                    //Creating a new popup
                    curPopUp = createDisplayInfoPopUp(curHeight, 213, cmbBxAccuracy);
                }
                else
                {
                    //Popup is being currently displayed
                    curPopUp = (Form)lblAccuracy.Tag;
                    curPopUp.Controls.Clear();
                }

                cmbBxAccuracy.BackColor = getAccuracyColor(curFitConfig.expectedAccuracy);
                ListView curLstVw = createFitConfigLstVw(curHeight, curFitConfig);
                curPopUp.Controls.Add(curLstVw);

                lblAccuracy.Tag = curPopUp;
            }
        }

        //Method returning the color associated with the input accuracy (e.g., Green for High)
        private Color getAccuracyColor(Accuracy curAccuracy)
        {
            Color curColor = Color.DarkGreen;
            if (curAccuracy == Accuracy.Medium)
            {
                curColor = Color.DarkOrange;
            }
            else if (curAccuracy == Accuracy.Low)
            {
                curColor = Color.DarkRed;
            }

            return curColor;
        }

        //Method setting up the popup used to display additional information about reliability (lblReliability) and accuracy (cmbBxAccuracy)
        private Form createDisplayInfoPopUp(int curHeight, int curWidth, Control refControl)
        {
            Form curPopUp = new Form();

            curPopUp.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            curPopUp.BackColor = Color.Gray;
            curPopUp.Show();
            curPopUp.Width = curWidth;
            curPopUp.Height = curHeight;

            //Locating the popup close enough to the control referred by the displayed information
            curPopUp.Left = refControl.PointToScreen(new Point()).X - curPopUp.Width;
            curPopUp.Top = refControl.PointToScreen(new Point()).Y - curPopUp.Height / 2 + refControl.Height / 2;
            curPopUp.TopMost = true;

            return curPopUp;
        }

        //By default, the NumericUpDowns providing the inputs for the predictions are expected to allow only values within the min./max. of the given variable in the input set.
        //On the other hand, if the checkbox is checked, any value should be allowed. This method takes care of this issue
        private void updateMinMaxInputNum(NumericUpDown curNum)
        {
            Variable curVar = null;
            if (curNum.Tag != null) curVar = (Variable)curNum.Tag;

            if (curVar != null)
            {
                curNum.DecimalPlaces = curVar.noDec;
                curNum.Increment = curVar.noDec > 0 ? (decimal)Math.Pow(10, -1 * curVar.noDec) : 1;
                if (!chckBxExtrapol.Checked)
                {
                    curNum.Minimum = (decimal)curVar.input.min >= decimal.MinValue ? (decimal)curVar.input.min : decimal.MinValue;
                    curNum.Maximum = (decimal)curVar.input.max <= decimal.MaxValue ? (decimal)curVar.input.max : decimal.MaxValue;
                }
            }

            if (chckBxExtrapol.Checked)
            {
                curNum.Minimum = decimal.MinValue;
                curNum.Maximum = decimal.MaxValue;
            }
            else
            {
                curNum.Value = curNum.Minimum;
            }
        }

#endregion

    }

    /// <summary>
    /// Class storing the information about the current combobox selection. It is meant to be used from the Backgroundworker thread (to avoid a cross-thread exception)
    /// </summary>
    public class ComboItem
    {
        public string selectedItem;
        public int selectedIndex;
    }

    /// <summary>
    /// Class used to pass the required information among controls to perform the (prediction) calculations
    /// </summary>
    public class VarValue
    {
        public Variable variable;
        public double value;
    }
}

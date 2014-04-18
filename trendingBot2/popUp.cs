using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace trendingBot2
{
    /// <summary>
    /// Class for the popUp form, which is used to allow the user to update the non-numerical columns
    /// </summary>
    public partial class popUp : Form
    {
        AllInputs allInputs; //All the input columns
        int curCol; //Column about which the user will be prompted

        //The class constructor takes as arguments the two relevant variables from the mainForm: list of all the columns and index of the current columns
        public popUp(AllInputs allInputs_temp, int curCol_temp)
        {
            InitializeComponent();

            allInputs = allInputs_temp;
            curCol = curCol_temp;
        }

        //Method call at the form load including basic starting configuration
        private void popUp_Load(object sender, EventArgs e)
        {
            List<DateTimeTypes> curList = new List<DateTimeTypes>();
            curList.Add(DateTimeTypes.Time);
            curList.Add(DateTimeTypes.Year);
            curList.Add(DateTimeTypes.Month);
            curList.Add(DateTimeTypes.Weekday);
            curList.Add(DateTimeTypes.Day);

            cmbBx2.Tag = curList; //List with the (enum) equivalences for each element in the combobox

            string nameToShow = "\"" + allInputs.inputs[curCol].displayedName + "\"";
            lblPopUp.Text = nameToShow + " does not have the expected numerical format." + Environment.NewLine + "How should this column be treated?";

            cmbBxPopUp.SelectedIndex = 0;
        }

        //Method triggered when the popUp form is closed (because of clicking on the upper closing button or on btnPopUp), in charge of calling the corresponding method to update the information in mainForm
        private void popUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            InputType newType = new InputType();
            newType.mainType = MainTypes.Blank;
            if (cmbBxPopUp.SelectedIndex == 0)
            {
                newType.mainType = MainTypes.Categorical;
            }
            else if (cmbBxPopUp.SelectedIndex == 1)
            {
                //In case of being DateTime, cmbBx2 would also be considered to determine the secondary type
                newType.mainType = MainTypes.DateTime;
                newType.secType = ((List<DateTimeTypes>)cmbBx2.Tag)[cmbBx2.SelectedIndex];
            }

            Modifications curModif = (Modifications)this.Tag; //Current instance of the Modifications class, stored in the Tag of the form when it was created
            curModif.updateNonNumerical(allInputs, curCol, newType); //Method updating the information accounted (i.e., list of "Input") on account of the inputs from the user
        }

        //Clicking the button validates the current selection of the combobox(es) and, consequently, closes the popup to pass this information back to the mainForm
        private void btnPopUp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Method called every time a new item is selected in the cmbBxPopUp (, where the main type is being selected)
        private void cmbBxPopUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxPopUp.SelectedIndex == 1)
            {
                cmbBx2.SelectedIndex = 0; //For DateTime, the secondary combobox has also to be displayed
                cmbBx2.Visible = true;
                lblCmbx2.Visible = true;
            }
            else
            {
                cmbBx2.Visible = false;
                lblCmbx2.Visible = false;
            }
        }
    }
}

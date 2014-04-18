namespace trendingBot2
{
    partial class popUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPopUp = new System.Windows.Forms.Button();
            this.cmbBxPopUp = new System.Windows.Forms.ComboBox();
            this.lblPopUp = new System.Windows.Forms.Label();
            this.cmbBx2 = new System.Windows.Forms.ComboBox();
            this.lblCmbx2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPopUp
            // 
            this.btnPopUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPopUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPopUp.Location = new System.Drawing.Point(378, 80);
            this.btnPopUp.Name = "btnPopUp";
            this.btnPopUp.Size = new System.Drawing.Size(62, 36);
            this.btnPopUp.TabIndex = 2;
            this.btnPopUp.Text = "OK";
            this.btnPopUp.UseVisualStyleBackColor = true;
            this.btnPopUp.Click += new System.EventHandler(this.btnPopUp_Click);
            // 
            // cmbBxPopUp
            // 
            this.cmbBxPopUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbBxPopUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxPopUp.FormattingEnabled = true;
            this.cmbBxPopUp.Items.AddRange(new object[] {
            "Categorical",
            "Date/Time",
            "Ignore this column"});
            this.cmbBxPopUp.Location = new System.Drawing.Point(138, 81);
            this.cmbBxPopUp.Name = "cmbBxPopUp";
            this.cmbBxPopUp.Size = new System.Drawing.Size(208, 32);
            this.cmbBxPopUp.TabIndex = 0;
            this.cmbBxPopUp.SelectedIndexChanged += new System.EventHandler(this.cmbBxPopUp_SelectedIndexChanged);
            // 
            // lblPopUp
            // 
            this.lblPopUp.AutoSize = true;
            this.lblPopUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPopUp.Location = new System.Drawing.Point(21, 19);
            this.lblPopUp.Name = "lblPopUp";
            this.lblPopUp.Size = new System.Drawing.Size(0, 18);
            this.lblPopUp.TabIndex = 2;
            // 
            // cmbBx2
            // 
            this.cmbBx2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbBx2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBx2.FormattingEnabled = true;
            this.cmbBx2.Items.AddRange(new object[] {
            "Time (h, m, s)",
            "Years",
            "Months",
            "Weekdays",
            "Days"});
            this.cmbBx2.Location = new System.Drawing.Point(209, 132);
            this.cmbBx2.Name = "cmbBx2";
            this.cmbBx2.Size = new System.Drawing.Size(137, 26);
            this.cmbBx2.TabIndex = 1;
            this.cmbBx2.Visible = false;
            // 
            // lblCmbx2
            // 
            this.lblCmbx2.AutoSize = true;
            this.lblCmbx2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCmbx2.Location = new System.Drawing.Point(58, 137);
            this.lblCmbx2.Name = "lblCmbx2";
            this.lblCmbx2.Size = new System.Drawing.Size(145, 16);
            this.lblCmbx2.TabIndex = 4;
            this.lblCmbx2.Text = "Information to consider:";
            this.lblCmbx2.Visible = false;
            // 
            // popUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 192);
            this.Controls.Add(this.lblCmbx2);
            this.Controls.Add(this.cmbBx2);
            this.Controls.Add(this.lblPopUp);
            this.Controls.Add(this.cmbBxPopUp);
            this.Controls.Add(this.btnPopUp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "popUp";
            this.ShowIcon = false;
            this.Text = "Non-numerical Inputs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.popUp_FormClosing);
            this.Load += new System.EventHandler(this.popUp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPopUp;
        private System.Windows.Forms.ComboBox cmbBxPopUp;
        private System.Windows.Forms.Label lblPopUp;
        private System.Windows.Forms.ComboBox cmbBx2;
        private System.Windows.Forms.Label lblCmbx2;
    }
}
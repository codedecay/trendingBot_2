namespace trendingBot2
{
    partial class mainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.lstBxCombinations = new System.Windows.Forms.ListBox();
            this.rtbCombinations = new System.Windows.Forms.RichTextBox();
            this.pnlPredictions = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlPredOutput = new System.Windows.Forms.Panel();
            this.lblAverError = new System.Windows.Forms.Label();
            this.lblReliability = new System.Windows.Forms.Label();
            this.lblPredValOutput = new System.Windows.Forms.Label();
            this.lblPredTitOutput = new System.Windows.Forms.Label();
            this.pnlPredInputs = new System.Windows.Forms.Panel();
            this.numPredInput0 = new System.Windows.Forms.NumericUpDown();
            this.lblPredInp0 = new System.Windows.Forms.Label();
            this.grpBxResults = new System.Windows.Forms.GroupBox();
            this.lblNonNumerical2 = new System.Windows.Forms.Label();
            this.lblNonNumerical1 = new System.Windows.Forms.Label();
            this.lstVwNonNumerical = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmbBxNonNumerical = new System.Windows.Forms.ComboBox();
            this.chckBxExtrapol = new System.Windows.Forms.CheckBox();
            this.grpBxInputs = new System.Windows.Forms.GroupBox();
            this.lblAccuracy = new System.Windows.Forms.Label();
            this.cmbBxAccuracy = new System.Windows.Forms.ComboBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBxIndependent = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvInputs = new System.Windows.Forms.DataGridView();
            this.pnlPredictions.SuspendLayout();
            this.pnlPredOutput.SuspendLayout();
            this.pnlPredInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPredInput0)).BeginInit();
            this.grpBxResults.SuspendLayout();
            this.grpBxInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).BeginInit();
            this.SuspendLayout();
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            this.bgwMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMain_DoWork);
            this.bgwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwMain_ProgressChanged);
            this.bgwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMain_RunWorkerCompleted);
            // 
            // lstBxCombinations
            // 
            this.lstBxCombinations.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lstBxCombinations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstBxCombinations.FormattingEnabled = true;
            this.lstBxCombinations.ItemHeight = 16;
            this.lstBxCombinations.Location = new System.Drawing.Point(38, 70);
            this.lstBxCombinations.Name = "lstBxCombinations";
            this.lstBxCombinations.Size = new System.Drawing.Size(158, 132);
            this.lstBxCombinations.TabIndex = 3;
            this.lstBxCombinations.SelectedIndexChanged += new System.EventHandler(this.lstBxCombinations_SelectedIndexChanged);
            // 
            // rtbCombinations
            // 
            this.rtbCombinations.BackColor = System.Drawing.SystemColors.Control;
            this.rtbCombinations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbCombinations.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbCombinations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCombinations.Location = new System.Drawing.Point(35, 229);
            this.rtbCombinations.Name = "rtbCombinations";
            this.rtbCombinations.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            this.rtbCombinations.Size = new System.Drawing.Size(363, 108);
            this.rtbCombinations.TabIndex = 3;
            this.rtbCombinations.TabStop = false;
            this.rtbCombinations.Text = "";
            // 
            // pnlPredictions
            // 
            this.pnlPredictions.BackColor = System.Drawing.Color.LightGray;
            this.pnlPredictions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPredictions.Controls.Add(this.panel1);
            this.pnlPredictions.Controls.Add(this.pnlPredOutput);
            this.pnlPredictions.Controls.Add(this.pnlPredInputs);
            this.pnlPredictions.Location = new System.Drawing.Point(276, 67);
            this.pnlPredictions.Name = "pnlPredictions";
            this.pnlPredictions.Size = new System.Drawing.Size(487, 140);
            this.pnlPredictions.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(245, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 100);
            this.panel1.TabIndex = 7;
            // 
            // pnlPredOutput
            // 
            this.pnlPredOutput.BackColor = System.Drawing.Color.LightGray;
            this.pnlPredOutput.Controls.Add(this.lblAverError);
            this.pnlPredOutput.Controls.Add(this.lblReliability);
            this.pnlPredOutput.Controls.Add(this.lblPredValOutput);
            this.pnlPredOutput.Controls.Add(this.lblPredTitOutput);
            this.pnlPredOutput.Location = new System.Drawing.Point(253, 11);
            this.pnlPredOutput.Name = "pnlPredOutput";
            this.pnlPredOutput.Size = new System.Drawing.Size(224, 121);
            this.pnlPredOutput.TabIndex = 7;
            // 
            // lblAverError
            // 
            this.lblAverError.AutoSize = true;
            this.lblAverError.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAverError.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAverError.ForeColor = System.Drawing.Color.Black;
            this.lblAverError.Location = new System.Drawing.Point(18, 35);
            this.lblAverError.Name = "lblAverError";
            this.lblAverError.Size = new System.Drawing.Size(0, 18);
            this.lblAverError.TabIndex = 12;
            // 
            // lblReliability
            // 
            this.lblReliability.AutoSize = true;
            this.lblReliability.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblReliability.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReliability.ForeColor = System.Drawing.Color.Black;
            this.lblReliability.Location = new System.Drawing.Point(18, 10);
            this.lblReliability.Name = "lblReliability";
            this.lblReliability.Size = new System.Drawing.Size(0, 20);
            this.lblReliability.TabIndex = 11;
            this.lblReliability.MouseEnter += new System.EventHandler(this.lblReliability_MouseEnter);
            this.lblReliability.MouseLeave += new System.EventHandler(this.lblReliability_MouseLeave);
            // 
            // lblPredValOutput
            // 
            this.lblPredValOutput.AutoSize = true;
            this.lblPredValOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPredValOutput.ForeColor = System.Drawing.Color.Black;
            this.lblPredValOutput.Location = new System.Drawing.Point(85, 70);
            this.lblPredValOutput.Name = "lblPredValOutput";
            this.lblPredValOutput.Size = new System.Drawing.Size(0, 25);
            this.lblPredValOutput.TabIndex = 10;
            // 
            // lblPredTitOutput
            // 
            this.lblPredTitOutput.AutoSize = true;
            this.lblPredTitOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPredTitOutput.ForeColor = System.Drawing.Color.Black;
            this.lblPredTitOutput.Location = new System.Drawing.Point(15, 70);
            this.lblPredTitOutput.Name = "lblPredTitOutput";
            this.lblPredTitOutput.Size = new System.Drawing.Size(0, 25);
            this.lblPredTitOutput.TabIndex = 9;
            // 
            // pnlPredInputs
            // 
            this.pnlPredInputs.AutoScroll = true;
            this.pnlPredInputs.BackColor = System.Drawing.Color.LightGray;
            this.pnlPredInputs.Controls.Add(this.numPredInput0);
            this.pnlPredInputs.Controls.Add(this.lblPredInp0);
            this.pnlPredInputs.Location = new System.Drawing.Point(10, 22);
            this.pnlPredInputs.Name = "pnlPredInputs";
            this.pnlPredInputs.Size = new System.Drawing.Size(214, 102);
            this.pnlPredInputs.TabIndex = 7;
            // 
            // numPredInput0
            // 
            this.numPredInput0.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numPredInput0.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPredInput0.Location = new System.Drawing.Point(75, 18);
            this.numPredInput0.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numPredInput0.Name = "numPredInput0";
            this.numPredInput0.Size = new System.Drawing.Size(105, 26);
            this.numPredInput0.TabIndex = 1;
            this.numPredInput0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPredInp0
            // 
            this.lblPredInp0.AutoSize = true;
            this.lblPredInp0.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPredInp0.ForeColor = System.Drawing.Color.Black;
            this.lblPredInp0.Location = new System.Drawing.Point(8, 20);
            this.lblPredInp0.Name = "lblPredInp0";
            this.lblPredInp0.Size = new System.Drawing.Size(0, 20);
            this.lblPredInp0.TabIndex = 2;
            // 
            // grpBxResults
            // 
            this.grpBxResults.Controls.Add(this.lblNonNumerical2);
            this.grpBxResults.Controls.Add(this.lblNonNumerical1);
            this.grpBxResults.Controls.Add(this.lstVwNonNumerical);
            this.grpBxResults.Controls.Add(this.cmbBxNonNumerical);
            this.grpBxResults.Controls.Add(this.chckBxExtrapol);
            this.grpBxResults.Controls.Add(this.lstBxCombinations);
            this.grpBxResults.Controls.Add(this.rtbCombinations);
            this.grpBxResults.Controls.Add(this.pnlPredictions);
            this.grpBxResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxResults.Location = new System.Drawing.Point(27, 345);
            this.grpBxResults.Name = "grpBxResults";
            this.grpBxResults.Size = new System.Drawing.Size(797, 357);
            this.grpBxResults.TabIndex = 9;
            this.grpBxResults.TabStop = false;
            this.grpBxResults.Text = "Predictions";
            // 
            // lblNonNumerical2
            // 
            this.lblNonNumerical2.AutoSize = true;
            this.lblNonNumerical2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNonNumerical2.ForeColor = System.Drawing.Color.DimGray;
            this.lblNonNumerical2.Location = new System.Drawing.Point(420, 325);
            this.lblNonNumerical2.Name = "lblNonNumerical2";
            this.lblNonNumerical2.Size = new System.Drawing.Size(0, 15);
            this.lblNonNumerical2.TabIndex = 22;
            // 
            // lblNonNumerical1
            // 
            this.lblNonNumerical1.AutoSize = true;
            this.lblNonNumerical1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNonNumerical1.Location = new System.Drawing.Point(424, 252);
            this.lblNonNumerical1.Name = "lblNonNumerical1";
            this.lblNonNumerical1.Size = new System.Drawing.Size(128, 15);
            this.lblNonNumerical1.TabIndex = 21;
            this.lblNonNumerical1.Text = "Non-numerical inputs:";
            // 
            // lstVwNonNumerical
            // 
            this.lstVwNonNumerical.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1,
            this.col2,
            this.col3});
            this.lstVwNonNumerical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lstVwNonNumerical.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstVwNonNumerical.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstVwNonNumerical.Location = new System.Drawing.Point(573, 245);
            this.lstVwNonNumerical.Name = "lstVwNonNumerical";
            this.lstVwNonNumerical.Size = new System.Drawing.Size(180, 64);
            this.lstVwNonNumerical.TabIndex = 20;
            this.lstVwNonNumerical.UseCompatibleStateImageBehavior = false;
            this.lstVwNonNumerical.View = System.Windows.Forms.View.Details;
            this.lstVwNonNumerical.Visible = false;
            // 
            // col1
            // 
            this.col1.Text = "";
            this.col1.Width = 0;
            // 
            // col2
            // 
            this.col2.Text = "Original";
            this.col2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.col2.Width = 71;
            // 
            // col3
            // 
            this.col3.Text = "Conversion";
            this.col3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.col3.Width = 88;
            // 
            // cmbBxNonNumerical
            // 
            this.cmbBxNonNumerical.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbBxNonNumerical.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxNonNumerical.FormattingEnabled = true;
            this.cmbBxNonNumerical.Items.AddRange(new object[] {
            "High",
            "Medium ",
            "Low"});
            this.cmbBxNonNumerical.Location = new System.Drawing.Point(422, 278);
            this.cmbBxNonNumerical.Name = "cmbBxNonNumerical";
            this.cmbBxNonNumerical.Size = new System.Drawing.Size(130, 23);
            this.cmbBxNonNumerical.TabIndex = 19;
            this.cmbBxNonNumerical.Visible = false;
            this.cmbBxNonNumerical.SelectedIndexChanged += new System.EventHandler(this.cmbBxNonNumerical_SelectedIndexChanged);
            // 
            // chckBxExtrapol
            // 
            this.chckBxExtrapol.AutoSize = true;
            this.chckBxExtrapol.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chckBxExtrapol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckBxExtrapol.Location = new System.Drawing.Point(282, 35);
            this.chckBxExtrapol.Name = "chckBxExtrapol";
            this.chckBxExtrapol.Size = new System.Drawing.Size(431, 20);
            this.chckBxExtrapol.TabIndex = 5;
            this.chckBxExtrapol.Text = "Allow values outside the training set boundaries (not recommended).";
            this.chckBxExtrapol.UseVisualStyleBackColor = true;
            this.chckBxExtrapol.CheckedChanged += new System.EventHandler(this.chckBxExtrapol_CheckedChanged);
            // 
            // grpBxInputs
            // 
            this.grpBxInputs.Controls.Add(this.lblAccuracy);
            this.grpBxInputs.Controls.Add(this.cmbBxAccuracy);
            this.grpBxInputs.Controls.Add(this.btnUpdate);
            this.grpBxInputs.Controls.Add(this.label1);
            this.grpBxInputs.Controls.Add(this.cmbBxIndependent);
            this.grpBxInputs.Controls.Add(this.btnRun);
            this.grpBxInputs.Controls.Add(this.lblStatus);
            this.grpBxInputs.Controls.Add(this.dgvInputs);
            this.grpBxInputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxInputs.Location = new System.Drawing.Point(27, 17);
            this.grpBxInputs.Name = "grpBxInputs";
            this.grpBxInputs.Size = new System.Drawing.Size(797, 305);
            this.grpBxInputs.TabIndex = 10;
            this.grpBxInputs.TabStop = false;
            this.grpBxInputs.Text = "Inputs";
            // 
            // lblAccuracy
            // 
            this.lblAccuracy.AutoSize = true;
            this.lblAccuracy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccuracy.Location = new System.Drawing.Point(539, 143);
            this.lblAccuracy.Name = "lblAccuracy";
            this.lblAccuracy.Size = new System.Drawing.Size(126, 16);
            this.lblAccuracy.TabIndex = 16;
            this.lblAccuracy.Text = "Expected accuracy:";
            // 
            // cmbBxAccuracy
            // 
            this.cmbBxAccuracy.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbBxAccuracy.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBxAccuracy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbBxAccuracy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxAccuracy.ForeColor = System.Drawing.Color.White;
            this.cmbBxAccuracy.FormattingEnabled = true;
            this.cmbBxAccuracy.Items.AddRange(new object[] {
            "High",
            "Medium ",
            "Low"});
            this.cmbBxAccuracy.Location = new System.Drawing.Point(671, 140);
            this.cmbBxAccuracy.Name = "cmbBxAccuracy";
            this.cmbBxAccuracy.Size = new System.Drawing.Size(82, 24);
            this.cmbBxAccuracy.TabIndex = 1;
            this.cmbBxAccuracy.SelectedIndexChanged += new System.EventHandler(this.cmbBxAccuracy_SelectedIndexChanged);
            this.cmbBxAccuracy.MouseEnter += new System.EventHandler(this.cmbBxAccuracy_MouseEnter);
            this.cmbBxAccuracy.MouseLeave += new System.EventHandler(this.cmbBxAccuracy_MouseLeave);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Location = new System.Drawing.Point(392, 30);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(108, 26);
            this.btnUpdate.TabIndex = 20;
            this.btnUpdate.Text = "Update inputs";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(529, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 25);
            this.label1.TabIndex = 13;
            this.label1.Text = "Variable to predict:";
            // 
            // cmbBxIndependent
            // 
            this.cmbBxIndependent.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbBxIndependent.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBxIndependent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbBxIndependent.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxIndependent.ForeColor = System.Drawing.Color.Black;
            this.cmbBxIndependent.FormattingEnabled = true;
            this.cmbBxIndependent.Location = new System.Drawing.Point(529, 92);
            this.cmbBxIndependent.Name = "cmbBxIndependent";
            this.cmbBxIndependent.Size = new System.Drawing.Size(239, 33);
            this.cmbBxIndependent.TabIndex = 0;
            this.cmbBxIndependent.SelectedIndexChanged += new System.EventHandler(this.cmbBxIndependent_SelectedIndexChanged);
            // 
            // btnRun
            // 
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(590, 183);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(113, 55);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(35, 255);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 18);
            this.lblStatus.TabIndex = 10;
            // 
            // dgvInputs
            // 
            this.dgvInputs.AllowUserToAddRows = false;
            this.dgvInputs.AllowUserToDeleteRows = false;
            this.dgvInputs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInputs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvInputs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvInputs.Location = new System.Drawing.Point(35, 65);
            this.dgvInputs.Name = "dgvInputs";
            this.dgvInputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInputs.Size = new System.Drawing.Size(465, 174);
            this.dgvInputs.TabIndex = 20;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 349);
            this.Controls.Add(this.grpBxInputs);
            this.Controls.Add(this.grpBxResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "trendingBot 2.0";
            this.Activated += new System.EventHandler(this.mainForm_Activated);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.SizeChanged += new System.EventHandler(this.mainForm_SizeChanged);
            this.pnlPredictions.ResumeLayout(false);
            this.pnlPredOutput.ResumeLayout(false);
            this.pnlPredOutput.PerformLayout();
            this.pnlPredInputs.ResumeLayout(false);
            this.pnlPredInputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPredInput0)).EndInit();
            this.grpBxResults.ResumeLayout(false);
            this.grpBxResults.PerformLayout();
            this.grpBxInputs.ResumeLayout(false);
            this.grpBxInputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bgwMain;
        private System.Windows.Forms.ListBox lstBxCombinations;
        private System.Windows.Forms.RichTextBox rtbCombinations;
        private System.Windows.Forms.Panel pnlPredictions;
        private System.Windows.Forms.Label lblPredInp0;
        private System.Windows.Forms.NumericUpDown numPredInput0;
        private System.Windows.Forms.Panel pnlPredInputs;
        private System.Windows.Forms.Panel pnlPredOutput;
        private System.Windows.Forms.Label lblReliability;
        private System.Windows.Forms.Label lblPredValOutput;
        private System.Windows.Forms.Label lblPredTitOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grpBxResults;
        private System.Windows.Forms.GroupBox grpBxInputs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBxIndependent;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvInputs;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox chckBxExtrapol;
        private System.Windows.Forms.ComboBox cmbBxAccuracy;
        private System.Windows.Forms.Label lblAccuracy;
        private System.Windows.Forms.Label lblAverError;
        private System.Windows.Forms.Label lblNonNumerical1;
        private System.Windows.Forms.ListView lstVwNonNumerical;
        private System.Windows.Forms.ColumnHeader col1;
        private System.Windows.Forms.ColumnHeader col2;
        private System.Windows.Forms.ColumnHeader col3;
        private System.Windows.Forms.ComboBox cmbBxNonNumerical;
        private System.Windows.Forms.Label lblNonNumerical2;

    }
}


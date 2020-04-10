namespace Revit_ManageElectricalCircuit
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpperLevelPanel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LowerLevelPanel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpperLevelElement = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LowerLevelElement = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.checkAll = new System.Windows.Forms.Button();
            this.checkNone = new System.Windows.Forms.Button();
            this.accept = new System.Windows.Forms.Button();
            this.Close = new System.Windows.Forms.Button();
            this.Apply = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(764, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.UpperLevelPanel,
            this.LowerLevelPanel,
            this.Column3,
            this.Column4,
            this.UpperLevelElement,
            this.LowerLevelElement});
            this.dataGridView1.Location = new System.Drawing.Point(9, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(789, 227);
            this.dataGridView1.TabIndex = 2;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Select";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 50;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Panel Name";
            this.Column2.Name = "Column2";
            // 
            // UpperLevelPanel
            // 
            this.UpperLevelPanel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.UpperLevelPanel.DisplayStyleForCurrentCellOnly = true;
            this.UpperLevelPanel.HeaderText = "Upper Level Panel";
            this.UpperLevelPanel.Name = "UpperLevelPanel";
            this.UpperLevelPanel.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UpperLevelPanel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // LowerLevelPanel
            // 
            this.LowerLevelPanel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.LowerLevelPanel.DisplayStyleForCurrentCellOnly = true;
            this.LowerLevelPanel.HeaderText = "Lower Level Panel";
            this.LowerLevelPanel.Name = "LowerLevelPanel";
            this.LowerLevelPanel.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LowerLevelPanel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Circuit Number";
            this.Column3.Name = "Column3";
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Circuit Name";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // UpperLevelElement
            // 
            this.UpperLevelElement.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.UpperLevelElement.DisplayStyleForCurrentCellOnly = true;
            this.UpperLevelElement.HeaderText = "Upper Level Element";
            this.UpperLevelElement.Name = "UpperLevelElement";
            this.UpperLevelElement.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UpperLevelElement.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // LowerLevelElement
            // 
            this.LowerLevelElement.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.LowerLevelElement.DisplayStyleForCurrentCellOnly = true;
            this.LowerLevelElement.HeaderText = "Lower Level Element";
            this.LowerLevelElement.Name = "LowerLevelElement";
            this.LowerLevelElement.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LowerLevelElement.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // checkAll
            // 
            this.checkAll.Location = new System.Drawing.Point(809, 188);
            this.checkAll.Name = "checkAll";
            this.checkAll.Size = new System.Drawing.Size(74, 23);
            this.checkAll.TabIndex = 3;
            this.checkAll.Text = "Check All";
            this.checkAll.UseVisualStyleBackColor = true;
            this.checkAll.Click += new System.EventHandler(this.CheckAll_Click);
            // 
            // checkNone
            // 
            this.checkNone.Location = new System.Drawing.Point(809, 217);
            this.checkNone.Name = "checkNone";
            this.checkNone.Size = new System.Drawing.Size(74, 23);
            this.checkNone.TabIndex = 4;
            this.checkNone.Text = "Check None";
            this.checkNone.UseVisualStyleBackColor = true;
            this.checkNone.Click += new System.EventHandler(this.CheckNone_Click);
            // 
            // accept
            // 
            this.accept.Location = new System.Drawing.Point(724, 272);
            this.accept.Name = "accept";
            this.accept.Size = new System.Drawing.Size(74, 23);
            this.accept.TabIndex = 6;
            this.accept.Text = "Accept";
            this.accept.UseVisualStyleBackColor = true;
            this.accept.Click += new System.EventHandler(this.Accept_Click);
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(809, 272);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(74, 23);
            this.Close.TabIndex = 7;
            this.Close.Text = "Close";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // Apply
            // 
            this.Apply.Location = new System.Drawing.Point(640, 272);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(74, 23);
            this.Apply.TabIndex = 8;
            this.Apply.Text = "Apply";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 245);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(417, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(435, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "0%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 272);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 300);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Apply);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.accept);
            this.Controls.Add(this.checkNone);
            this.Controls.Add(this.checkAll);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button checkAll;
        private System.Windows.Forms.Button checkNone;
        private System.Windows.Forms.Button accept;
        private System.Windows.Forms.Button Close;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewComboBoxColumn UpperLevelPanel;
        private System.Windows.Forms.DataGridViewComboBoxColumn LowerLevelPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewComboBoxColumn UpperLevelElement;
        private System.Windows.Forms.DataGridViewComboBoxColumn LowerLevelElement;
    }
}
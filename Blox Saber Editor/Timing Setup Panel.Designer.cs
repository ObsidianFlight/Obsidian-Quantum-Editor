﻿namespace Sound_Space_Editor
{
    partial class TimingsWindow
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
            this.PointList = new System.Windows.Forms.DataGridView();
            this.BPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.BPMLabel = new System.Windows.Forms.Label();
            this.OffsetLabel = new System.Windows.Forms.Label();
            this.OffsetBox = new System.Windows.Forms.TextBox();
            this.BPMBox = new System.Windows.Forms.TextBox();
            this.CurrentButton = new System.Windows.Forms.Button();
            this.MoveLabel = new System.Windows.Forms.Label();
            this.MoveBox = new System.Windows.Forms.NumericUpDown();
            this.MoveButton = new System.Windows.Forms.Button();
            this.ImportCH = new System.Windows.Forms.Button();
            this.ImportOSU = new System.Windows.Forms.Button();
            this.ImportADOFAI = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PointList
            // 
            this.PointList.AllowUserToAddRows = false;
            this.PointList.AllowUserToDeleteRows = false;
            this.PointList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.PointList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PointList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PointList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BPM,
            this.Offset});
            this.PointList.Location = new System.Drawing.Point(9, 9);
            this.PointList.Margin = new System.Windows.Forms.Padding(0);
            this.PointList.Name = "PointList";
            this.PointList.ReadOnly = true;
            this.PointList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PointList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.PointList.Size = new System.Drawing.Size(326, 409);
            this.PointList.TabIndex = 9;
            this.PointList.SelectionChanged += new System.EventHandler(this.PointList_SelectionChanged);
            // 
            // BPM
            // 
            this.BPM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BPM.HeaderText = "BPM";
            this.BPM.Name = "BPM";
            this.BPM.ReadOnly = true;
            this.BPM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BPM.ToolTipText = "The BPM of the point";
            // 
            // Offset
            // 
            this.Offset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Offset.HeaderText = "Position (ms)";
            this.Offset.Name = "Offset";
            this.Offset.ReadOnly = true;
            this.Offset.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Offset.ToolTipText = "The position of the point in milliseconds";
            // 
            // RemoveButton
            // 
            this.RemoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.RemoveButton.Location = new System.Drawing.Point(247, 421);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(88, 22);
            this.RemoveButton.TabIndex = 5;
            this.RemoveButton.Text = "Remove Point";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.UpdateButton.Location = new System.Drawing.Point(247, 444);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(88, 22);
            this.UpdateButton.TabIndex = 4;
            this.UpdateButton.Text = "Update Point";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AddButton.Location = new System.Drawing.Point(158, 421);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(88, 22);
            this.AddButton.TabIndex = 3;
            this.AddButton.Text = "Add Point";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // BPMLabel
            // 
            this.BPMLabel.Location = new System.Drawing.Point(9, 426);
            this.BPMLabel.Margin = new System.Windows.Forms.Padding(0);
            this.BPMLabel.Name = "BPMLabel";
            this.BPMLabel.Size = new System.Drawing.Size(68, 16);
            this.BPMLabel.TabIndex = 5;
            this.BPMLabel.Text = "BPM";
            // 
            // OffsetLabel
            // 
            this.OffsetLabel.Location = new System.Drawing.Point(9, 447);
            this.OffsetLabel.Margin = new System.Windows.Forms.Padding(0);
            this.OffsetLabel.Name = "OffsetLabel";
            this.OffsetLabel.Size = new System.Drawing.Size(68, 16);
            this.OffsetLabel.TabIndex = 7;
            this.OffsetLabel.Text = "Position (ms)";
            // 
            // OffsetBox
            // 
            this.OffsetBox.Location = new System.Drawing.Point(80, 444);
            this.OffsetBox.Name = "OffsetBox";
            this.OffsetBox.Size = new System.Drawing.Size(72, 20);
            this.OffsetBox.TabIndex = 2;
            // 
            // BPMBox
            // 
            this.BPMBox.Location = new System.Drawing.Point(80, 422);
            this.BPMBox.Name = "BPMBox";
            this.BPMBox.Size = new System.Drawing.Size(72, 20);
            this.BPMBox.TabIndex = 1;
            // 
            // CurrentButton
            // 
            this.CurrentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CurrentButton.Location = new System.Drawing.Point(158, 444);
            this.CurrentButton.Name = "CurrentButton";
            this.CurrentButton.Size = new System.Drawing.Size(88, 22);
            this.CurrentButton.TabIndex = 4;
            this.CurrentButton.Text = "Current Pos";
            this.CurrentButton.UseVisualStyleBackColor = true;
            this.CurrentButton.Click += new System.EventHandler(this.CurrentButton_Click);
            // 
            // MoveLabel
            // 
            this.MoveLabel.Location = new System.Drawing.Point(9, 470);
            this.MoveLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MoveLabel.Name = "MoveLabel";
            this.MoveLabel.Size = new System.Drawing.Size(143, 16);
            this.MoveLabel.TabIndex = 10;
            this.MoveLabel.Text = "Move Selected Points (ms)";
            // 
            // MoveBox
            // 
            this.MoveBox.Location = new System.Drawing.Point(158, 468);
            this.MoveBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.MoveBox.Minimum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            -2147483648});
            this.MoveBox.Name = "MoveBox";
            this.MoveBox.Size = new System.Drawing.Size(88, 20);
            this.MoveBox.TabIndex = 11;
            // 
            // MoveButton
            // 
            this.MoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoveButton.Location = new System.Drawing.Point(247, 467);
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.Size = new System.Drawing.Size(88, 22);
            this.MoveButton.TabIndex = 12;
            this.MoveButton.Text = "Move Points";
            this.MoveButton.UseVisualStyleBackColor = true;
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // ImportCH
            // 
            this.ImportCH.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportCH.Location = new System.Drawing.Point(230, 494);
            this.ImportCH.Name = "ImportCH";
            this.ImportCH.Size = new System.Drawing.Size(105, 47);
            this.ImportCH.TabIndex = 13;
            this.ImportCH.Text = "Import Clone Hero Timings";
            this.ImportCH.UseVisualStyleBackColor = true;
            this.ImportCH.Click += new System.EventHandler(this.ImportCH_Click);
            // 
            // ImportOSU
            // 
            this.ImportOSU.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportOSU.Location = new System.Drawing.Point(9, 494);
            this.ImportOSU.Name = "ImportOSU";
            this.ImportOSU.Size = new System.Drawing.Size(105, 47);
            this.ImportOSU.TabIndex = 14;
            this.ImportOSU.Text = "Import OSU Timings";
            this.ImportOSU.UseVisualStyleBackColor = true;
            this.ImportOSU.Click += new System.EventHandler(this.ImportOSU_Click);
            // 
            // ImportADOFAI
            // 
            this.ImportADOFAI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportADOFAI.Location = new System.Drawing.Point(120, 494);
            this.ImportADOFAI.Name = "ImportADOFAI";
            this.ImportADOFAI.Size = new System.Drawing.Size(104, 47);
            this.ImportADOFAI.TabIndex = 15;
            this.ImportADOFAI.Text = "Import ADOFAI Timings";
            this.ImportADOFAI.UseVisualStyleBackColor = true;
            this.ImportADOFAI.Click += new System.EventHandler(this.ImportADOFAI_Click);
            // 
            // TimingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 550);
            this.Controls.Add(this.ImportADOFAI);
            this.Controls.Add(this.ImportOSU);
            this.Controls.Add(this.ImportCH);
            this.Controls.Add(this.MoveButton);
            this.Controls.Add(this.MoveBox);
            this.Controls.Add(this.MoveLabel);
            this.Controls.Add(this.BPMBox);
            this.Controls.Add(this.OffsetBox);
            this.Controls.Add(this.OffsetLabel);
            this.Controls.Add(this.BPMLabel);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.CurrentButton);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.PointList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TimingsWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timing Setup Panel";
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView PointList;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Label BPMLabel;
        private System.Windows.Forms.Label OffsetLabel;
        private System.Windows.Forms.TextBox OffsetBox;
        private System.Windows.Forms.TextBox BPMBox;
		private System.Windows.Forms.Button CurrentButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn BPM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
        private System.Windows.Forms.Label MoveLabel;
        private System.Windows.Forms.NumericUpDown MoveBox;
        private System.Windows.Forms.Button MoveButton;
        private System.Windows.Forms.Button ImportCH;
        private System.Windows.Forms.Button ImportOSU;
        private System.Windows.Forms.Button ImportADOFAI;
    }
}
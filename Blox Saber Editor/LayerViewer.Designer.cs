namespace Sound_Space_Editor
{
    partial class LayerViewer
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
            this.LayerList = new System.Windows.Forms.DataGridView();
            this.Layer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alternates = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Colors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddLayerButton = new System.Windows.Forms.Button();
            this.RemoveLayerButton = new System.Windows.Forms.Button();
            this.LayerBox = new System.Windows.Forms.NumericUpDown();
            this.SendNotesButton = new System.Windows.Forms.Button();
            this.SendNotesBox = new System.Windows.Forms.NumericUpDown();
            this.FadeButton = new System.Windows.Forms.Button();
            this.FadeAmountBox = new System.Windows.Forms.NumericUpDown();
            this.KeepShift = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.LayerList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LayerBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendNotesBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FadeAmountBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LayerList
            // 
            this.LayerList.AllowUserToAddRows = false;
            this.LayerList.AllowUserToDeleteRows = false;
            this.LayerList.AllowUserToResizeColumns = false;
            this.LayerList.AllowUserToResizeRows = false;
            this.LayerList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LayerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LayerList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Layer,
            this.Alternates,
            this.Colors,
            this.Notes,
            this.Fade});
            this.LayerList.Location = new System.Drawing.Point(12, 12);
            this.LayerList.Name = "LayerList";
            this.LayerList.ReadOnly = true;
            this.LayerList.RowHeadersVisible = false;
            this.LayerList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.LayerList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LayerList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.LayerList.Size = new System.Drawing.Size(317, 284);
            this.LayerList.TabIndex = 0;
            this.LayerList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.LayerList_CellContentClick);
            // 
            // Layer
            // 
            this.Layer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Layer.HeaderText = "Layer";
            this.Layer.Name = "Layer";
            this.Layer.ReadOnly = true;
            this.Layer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Layer.ToolTipText = "A layer is an assortment of variables attached to notes.";
            // 
            // Alternates
            // 
            this.Alternates.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Alternates.HeaderText = "Alternates";
            this.Alternates.Name = "Alternates";
            this.Alternates.ReadOnly = true;
            this.Alternates.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Alternates.ToolTipText = "Amount of alternating Colors";
            // 
            // Colors
            // 
            this.Colors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Colors.HeaderText = "Colors";
            this.Colors.Name = "Colors";
            this.Colors.ReadOnly = true;
            this.Colors.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Colors.ToolTipText = "Colors contained within a Layer";
            // 
            // Notes
            // 
            this.Notes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Notes.HeaderText = "Notes";
            this.Notes.Name = "Notes";
            this.Notes.ReadOnly = true;
            this.Notes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Fade
            // 
            this.Fade.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Fade.HeaderText = "Fade";
            this.Fade.Name = "Fade";
            this.Fade.ReadOnly = true;
            this.Fade.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AddLayerButton
            // 
            this.AddLayerButton.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddLayerButton.Location = new System.Drawing.Point(12, 328);
            this.AddLayerButton.Name = "AddLayerButton";
            this.AddLayerButton.Size = new System.Drawing.Size(158, 36);
            this.AddLayerButton.TabIndex = 19;
            this.AddLayerButton.Text = "Add Layer";
            this.AddLayerButton.UseVisualStyleBackColor = true;
            this.AddLayerButton.Click += new System.EventHandler(this.AddLayerButton_Click);
            // 
            // RemoveLayerButton
            // 
            this.RemoveLayerButton.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveLayerButton.Location = new System.Drawing.Point(176, 328);
            this.RemoveLayerButton.Name = "RemoveLayerButton";
            this.RemoveLayerButton.Size = new System.Drawing.Size(158, 36);
            this.RemoveLayerButton.TabIndex = 20;
            this.RemoveLayerButton.Text = "Remove Layer";
            this.RemoveLayerButton.UseVisualStyleBackColor = true;
            this.RemoveLayerButton.Click += new System.EventHandler(this.RemoveLayerButton_Click);
            // 
            // LayerBox
            // 
            this.LayerBox.Location = new System.Drawing.Point(12, 302);
            this.LayerBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.LayerBox.Name = "LayerBox";
            this.LayerBox.Size = new System.Drawing.Size(317, 20);
            this.LayerBox.TabIndex = 22;
            // 
            // SendNotesButton
            // 
            this.SendNotesButton.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendNotesButton.Location = new System.Drawing.Point(12, 369);
            this.SendNotesButton.Name = "SendNotesButton";
            this.SendNotesButton.Size = new System.Drawing.Size(158, 36);
            this.SendNotesButton.TabIndex = 23;
            this.SendNotesButton.Text = "Send Notes to Layer";
            this.SendNotesButton.UseVisualStyleBackColor = true;
            this.SendNotesButton.Click += new System.EventHandler(this.SendNotesButton_Click);
            // 
            // SendNotesBox
            // 
            this.SendNotesBox.Location = new System.Drawing.Point(176, 380);
            this.SendNotesBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.SendNotesBox.Name = "SendNotesBox";
            this.SendNotesBox.Size = new System.Drawing.Size(72, 20);
            this.SendNotesBox.TabIndex = 25;
            // 
            // FadeButton
            // 
            this.FadeButton.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FadeButton.Location = new System.Drawing.Point(12, 412);
            this.FadeButton.Name = "FadeButton";
            this.FadeButton.Size = new System.Drawing.Size(158, 36);
            this.FadeButton.TabIndex = 26;
            this.FadeButton.Text = "Layer|Fade";
            this.FadeButton.UseVisualStyleBackColor = true;
            this.FadeButton.Click += new System.EventHandler(this.FadeButton_Click);
            // 
            // FadeAmountBox
            // 
            this.FadeAmountBox.Location = new System.Drawing.Point(176, 423);
            this.FadeAmountBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.FadeAmountBox.Name = "FadeAmountBox";
            this.FadeAmountBox.Size = new System.Drawing.Size(153, 20);
            this.FadeAmountBox.TabIndex = 27;
            // 
            // KeepShift
            // 
            this.KeepShift.AutoSize = true;
            this.KeepShift.Location = new System.Drawing.Point(254, 383);
            this.KeepShift.Name = "KeepShift";
            this.KeepShift.Size = new System.Drawing.Size(75, 17);
            this.KeepShift.TabIndex = 28;
            this.KeepShift.Text = "Keep Shift";
            this.KeepShift.UseVisualStyleBackColor = true;
            // 
            // LayerViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(341, 460);
            this.Controls.Add(this.KeepShift);
            this.Controls.Add(this.FadeAmountBox);
            this.Controls.Add(this.FadeButton);
            this.Controls.Add(this.SendNotesBox);
            this.Controls.Add(this.SendNotesButton);
            this.Controls.Add(this.LayerBox);
            this.Controls.Add(this.RemoveLayerButton);
            this.Controls.Add(this.AddLayerButton);
            this.Controls.Add(this.LayerList);
            this.MaximumSize = new System.Drawing.Size(357, 499);
            this.MinimumSize = new System.Drawing.Size(357, 499);
            this.Name = "LayerViewer";
            this.ShowIcon = false;
            ((System.ComponentModel.ISupportInitialize)(this.LayerList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LayerBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendNotesBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FadeAmountBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView LayerList;
        private System.Windows.Forms.Button AddLayerButton;
        private System.Windows.Forms.Button RemoveLayerButton;
        private System.Windows.Forms.NumericUpDown LayerBox;
        private System.Windows.Forms.Button SendNotesButton;
        private System.Windows.Forms.NumericUpDown SendNotesBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Layer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alternates;
        private System.Windows.Forms.DataGridViewTextBoxColumn Colors;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fade;
        private System.Windows.Forms.Button FadeButton;
        private System.Windows.Forms.NumericUpDown FadeAmountBox;
        private System.Windows.Forms.CheckBox KeepShift;
    }
}
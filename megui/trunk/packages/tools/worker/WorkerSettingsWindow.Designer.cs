// ****************************************************************************
// 
// Copyright (C) 2005-2017 Doom9 & al
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// 
// ****************************************************************************

namespace MeGUI
{
    partial class WorkerSettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkerSettingsWindow));
            this.workerSettingsListBox = new System.Windows.Forms.ListBox();
            this.btnAddSettings = new System.Windows.Forms.Button();
            this.btnDeleteSettings = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.workerJobsListBox = new System.Windows.Forms.CheckedListBox();
            this.workerMaximumCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.workerMaximumCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // workerSettingsListBox
            // 
            this.workerSettingsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workerSettingsListBox.FormattingEnabled = true;
            this.workerSettingsListBox.Location = new System.Drawing.Point(6, 19);
            this.workerSettingsListBox.Name = "workerSettingsListBox";
            this.workerSettingsListBox.Size = new System.Drawing.Size(562, 95);
            this.workerSettingsListBox.TabIndex = 1;
            this.workerSettingsListBox.SelectedIndexChanged += new System.EventHandler(this.workerSettingsListBox_SelectedIndexChanged);
            // 
            // btnAddSettings
            // 
            this.btnAddSettings.Location = new System.Drawing.Point(6, 116);
            this.btnAddSettings.Name = "btnAddSettings";
            this.btnAddSettings.Size = new System.Drawing.Size(88, 23);
            this.btnAddSettings.TabIndex = 2;
            this.btnAddSettings.Text = "add rule set";
            this.btnAddSettings.UseVisualStyleBackColor = true;
            this.btnAddSettings.Click += new System.EventHandler(this.btnAddSettings_Click);
            // 
            // btnDeleteSettings
            // 
            this.btnDeleteSettings.Location = new System.Drawing.Point(6, 120);
            this.btnDeleteSettings.Name = "btnDeleteSettings";
            this.btnDeleteSettings.Size = new System.Drawing.Size(96, 23);
            this.btnDeleteSettings.TabIndex = 3;
            this.btnDeleteSettings.Text = "remove rule set";
            this.btnDeleteSettings.UseVisualStyleBackColor = true;
            this.btnDeleteSettings.Click += new System.EventHandler(this.btnDeleteSettings_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(230, 117);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(34, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.workerJobsListBox);
            this.groupBox1.Controls.Add(this.btnAddSettings);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Location = new System.Drawing.Point(12, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 149);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " worker rule ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(102, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "maximum parallel jobs:";
            // 
            // workerJobsListBox
            // 
            this.workerJobsListBox.FormattingEnabled = true;
            this.workerJobsListBox.Location = new System.Drawing.Point(6, 16);
            this.workerJobsListBox.Name = "workerJobsListBox";
            this.workerJobsListBox.Size = new System.Drawing.Size(258, 94);
            this.workerJobsListBox.TabIndex = 1;
            // 
            // workerMaximumCount
            // 
            this.workerMaximumCount.Location = new System.Drawing.Point(126, 19);
            this.workerMaximumCount.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.workerMaximumCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.workerMaximumCount.Name = "workerMaximumCount";
            this.workerMaximumCount.Size = new System.Drawing.Size(34, 20);
            this.workerMaximumCount.TabIndex = 7;
            this.workerMaximumCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.workerMaximumCount.ValueChanged += new System.EventHandler(this.workerMaximumCount_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnResetSettings);
            this.groupBox2.Controls.Add(this.workerSettingsListBox);
            this.groupBox2.Controls.Add(this.btnDeleteSettings);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(574, 157);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " worker rule set - defines how many jobs of specific types can run in parallel";
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.Location = new System.Drawing.Point(108, 120);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(162, 23);
            this.btnResetSettings.TabIndex = 10;
            this.btnResetSettings.Text = "load default worker settings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "maximum parallel jobs:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.workerMaximumCount);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(288, 175);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 149);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " global worker settings ";
            // 
            // WorkerSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 332);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkerSettingsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Worker Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.workerMaximumCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox workerSettingsListBox;
        private System.Windows.Forms.Button btnAddSettings;
        private System.Windows.Forms.Button btnDeleteSettings;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox workerJobsListBox;
        private System.Windows.Forms.NumericUpDown workerMaximumCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnResetSettings;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
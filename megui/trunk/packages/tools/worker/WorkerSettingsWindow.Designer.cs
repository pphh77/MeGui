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
            this.cbRemoveJob = new System.Windows.Forms.CheckBox();
            this.cbAutoStart = new System.Windows.Forms.CheckBox();
            this.cbAutoStartOnStartup = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbIOPriority = new System.Windows.Forms.ComboBox();
            this.cbProcessPriority = new System.Windows.Forms.ComboBox();
            this.cbJobType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.workerMaximumCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            this.workerSettingsListBox.SelectedIndexChanged += new System.EventHandler(this.WorkerSettingsListBox_SelectedIndexChanged);
            // 
            // btnAddSettings
            // 
            this.btnAddSettings.Location = new System.Drawing.Point(6, 116);
            this.btnAddSettings.Name = "btnAddSettings";
            this.btnAddSettings.Size = new System.Drawing.Size(227, 23);
            this.btnAddSettings.TabIndex = 2;
            this.btnAddSettings.Text = "add rule set";
            this.btnAddSettings.UseVisualStyleBackColor = true;
            this.btnAddSettings.Click += new System.EventHandler(this.BtnAddSettings_Click);
            // 
            // btnDeleteSettings
            // 
            this.btnDeleteSettings.Location = new System.Drawing.Point(6, 120);
            this.btnDeleteSettings.Name = "btnDeleteSettings";
            this.btnDeleteSettings.Size = new System.Drawing.Size(227, 23);
            this.btnDeleteSettings.TabIndex = 3;
            this.btnDeleteSettings.Text = "remove rule set";
            this.btnDeleteSettings.UseVisualStyleBackColor = true;
            this.btnDeleteSettings.Click += new System.EventHandler(this.BtnDeleteSettings_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(149, 72);
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
            this.groupBox1.Size = new System.Drawing.Size(239, 149);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " worker rule ";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(108, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 46);
            this.label1.TabIndex = 5;
            this.label1.Text = "maximum parallel jobs of the selected jobs:";
            // 
            // workerJobsListBox
            // 
            this.workerJobsListBox.FormattingEnabled = true;
            this.workerJobsListBox.Location = new System.Drawing.Point(6, 16);
            this.workerJobsListBox.Name = "workerJobsListBox";
            this.workerJobsListBox.Size = new System.Drawing.Size(96, 94);
            this.workerJobsListBox.TabIndex = 1;
            // 
            // workerMaximumCount
            // 
            this.workerMaximumCount.Location = new System.Drawing.Point(123, 41);
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
            this.workerMaximumCount.ValueChanged += new System.EventHandler(this.WorkerMaximumCount_ValueChanged);
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
            this.btnResetSettings.Location = new System.Drawing.Point(354, 120);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(214, 23);
            this.btnResetSettings.TabIndex = 10;
            this.btnResetSettings.Text = "load default worker settings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.BtnResetSettings_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "maximum parallel jobs:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbRemoveJob);
            this.groupBox3.Controls.Add(this.cbAutoStart);
            this.groupBox3.Controls.Add(this.cbAutoStartOnStartup);
            this.groupBox3.Controls.Add(this.workerMaximumCount);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(257, 253);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(329, 71);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " global worker settings ";
            // 
            // cbRemoveJob
            // 
            this.cbRemoveJob.AutoSize = true;
            this.cbRemoveJob.Checked = true;
            this.cbRemoveJob.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRemoveJob.Location = new System.Drawing.Point(178, 43);
            this.cbRemoveJob.Name = "cbRemoveJob";
            this.cbRemoveJob.Size = new System.Drawing.Size(135, 17);
            this.cbRemoveJob.TabIndex = 22;
            this.cbRemoveJob.Text = "remove completed jobs";
            this.cbRemoveJob.CheckedChanged += new System.EventHandler(this.CbRemoveJob_CheckedChanged);
            // 
            // cbAutoStart
            // 
            this.cbAutoStart.AutoSize = true;
            this.cbAutoStart.Checked = true;
            this.cbAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoStart.Location = new System.Drawing.Point(178, 19);
            this.cbAutoStart.Name = "cbAutoStart";
            this.cbAutoStart.Size = new System.Drawing.Size(115, 17);
            this.cbAutoStart.TabIndex = 21;
            this.cbAutoStart.Text = "auto-start new jobs";
            this.cbAutoStart.CheckedChanged += new System.EventHandler(this.CbAutoStart_CheckedChanged);
            // 
            // cbAutoStartOnStartup
            // 
            this.cbAutoStartOnStartup.AutoSize = true;
            this.cbAutoStartOnStartup.Location = new System.Drawing.Point(6, 19);
            this.cbAutoStartOnStartup.Name = "cbAutoStartOnStartup";
            this.cbAutoStartOnStartup.Size = new System.Drawing.Size(162, 17);
            this.cbAutoStartOnStartup.TabIndex = 20;
            this.cbAutoStartOnStartup.Text = "auto-start on application start";
            this.cbAutoStartOnStartup.UseVisualStyleBackColor = true;
            this.cbAutoStartOnStartup.CheckedChanged += new System.EventHandler(this.CbAutoStartOnStartup_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.cbIOPriority);
            this.groupBox4.Controls.Add(this.cbProcessPriority);
            this.groupBox4.Controls.Add(this.cbJobType);
            this.groupBox4.Location = new System.Drawing.Point(257, 175);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(328, 72);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = " worker priority ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Process Priority";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "I/O Priority";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Job Type";
            // 
            // cbIOPriority
            // 
            this.cbIOPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIOPriority.FormattingEnabled = true;
            this.cbIOPriority.Items.AddRange(new object[] {
            "low",
            "normal"});
            this.cbIOPriority.Location = new System.Drawing.Point(218, 40);
            this.cbIOPriority.Name = "cbIOPriority";
            this.cbIOPriority.Size = new System.Drawing.Size(100, 21);
            this.cbIOPriority.TabIndex = 16;
            this.cbIOPriority.SelectedIndexChanged += new System.EventHandler(this.CbIOPriority_SelectedIndexChanged);
            // 
            // cbProcessPriority
            // 
            this.cbProcessPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProcessPriority.FormattingEnabled = true;
            this.cbProcessPriority.Items.AddRange(new object[] {
            "low",
            "below normal",
            "normal",
            "above normal"});
            this.cbProcessPriority.Location = new System.Drawing.Point(112, 40);
            this.cbProcessPriority.Name = "cbProcessPriority";
            this.cbProcessPriority.Size = new System.Drawing.Size(100, 21);
            this.cbProcessPriority.TabIndex = 15;
            this.cbProcessPriority.SelectedIndexChanged += new System.EventHandler(this.CbProcessPriority_SelectedIndexChanged);
            // 
            // cbJobType
            // 
            this.cbJobType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbJobType.FormattingEnabled = true;
            this.cbJobType.Location = new System.Drawing.Point(6, 40);
            this.cbJobType.Name = "cbJobType";
            this.cbJobType.Size = new System.Drawing.Size(100, 21);
            this.cbJobType.TabIndex = 14;
            this.cbJobType.SelectedIndexChanged += new System.EventHandler(this.CbJobType_SelectedIndexChanged);
            // 
            // WorkerSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 332);
            this.Controls.Add(this.groupBox4);
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
            ((System.ComponentModel.ISupportInitialize)(this.workerMaximumCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbIOPriority;
        private System.Windows.Forms.ComboBox cbProcessPriority;
        private System.Windows.Forms.ComboBox cbJobType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbAutoStartOnStartup;
        private System.Windows.Forms.CheckBox cbAutoStart;
        private System.Windows.Forms.CheckBox cbRemoveJob;
    }
}
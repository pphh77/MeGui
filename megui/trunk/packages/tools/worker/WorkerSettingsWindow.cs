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

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MeGUI
{
    public partial class WorkerSettingsWindow : Form
    {
        public WorkerSettingsWindow()
        {
            InitializeComponent();

            workerMaximumCount.Value = MainForm.Instance.Settings.WorkerMaximumCount;
            workerJobsListBox.DataSource = EnumProxy.CreateArray(new object[] { JobType.Audio, JobType.Demuxer, JobType.Indexer, JobType.Muxer, JobType.OneClick, JobType.Video });
            cbJobType.DataSource = EnumProxy.CreateArray(new object[] { JobType.Audio, JobType.Demuxer, JobType.Indexer, JobType.Muxer, JobType.OneClick, JobType.Video });
            cbAutoStartOnStartup.Checked = MainForm.Instance.Settings.WorkerAutoStartOnStartup;
            cbAutoStart.Checked = MainForm.Instance.Settings.WorkerAutoStart;
            cbRemoveJob.Checked = MainForm.Instance.Settings.WorkerRemoveJob;
            UpdateWorkerSettingsListBox();
        }

        private void UpdateWorkerSettingsListBox()
        {
            workerSettingsListBox.Items.Clear();
            foreach (WorkerSettings oSettings in MainForm.Instance.Settings.WorkerSettings)
            {
                string strText = string.Empty;
                foreach (JobType oType in oSettings.JobTypes)
                {
                    strText += oType.ToString() + " Jobs + ";
                }
                strText = strText.Substring(0, strText.Length - 2) + " <= " + oSettings.MaxCount + " Job(s) in parallel";
                workerSettingsListBox.Items.Add(strText);
            }
        }

        private void BtnAddSettings_Click(object sender, EventArgs e)
        {
            if (workerJobsListBox.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one job type.", "Add Worker Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WorkerSettings oSettings = new WorkerSettings();
            for (int i = 0; i < workerJobsListBox.Items.Count; i++)
            {
                if (workerJobsListBox.GetItemCheckState(i) != CheckState.Checked)
                    continue;
                JobType oType = (JobType)((EnumProxy)workerJobsListBox.Items[i]).RealValue;
                oSettings.JobTypes.Add(oType);
            }
            oSettings.MaxCount = (int)numericUpDown1.Value;

            foreach (WorkerSettings oSettingsExisting in MainForm.Instance.Settings.WorkerSettings)
            {
                if (oSettingsExisting.HasSameJobTypeList(oSettings))
                {
                    if (oSettingsExisting.MaxCount != oSettings.MaxCount)
                    {
                        oSettingsExisting.MaxCount = oSettings.MaxCount;
                        UpdateWorkerSettingsListBox();
                    }
                    else
                        MessageBox.Show("This rule set is already in the list.", "Add Worker Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            MainForm.Instance.Settings.WorkerSettings.Add(oSettings);
            UpdateWorkerSettingsListBox();
        }

        private void BtnDeleteSettings_Click(object sender, EventArgs e)
        {
            int i = workerSettingsListBox.SelectedIndex;
            if (i < 0)
            {
                MessageBox.Show("Please select a rule set first.", "Remove Worker Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MainForm.Instance.Settings.WorkerSettings.RemoveAt(i);
            UpdateWorkerSettingsListBox();
        }

        private void WorkerMaximumCount_ValueChanged(object sender, EventArgs e)
        {
            if (MainForm.Instance.Settings.WorkerMaximumCount == (int)workerMaximumCount.Value)
                return;

            MainForm.Instance.Settings.WorkerMaximumCount = (int)workerMaximumCount.Value;
            MainForm.Instance.Jobs.AdjustWorkerCount(MainForm.Instance.Jobs.IsAnyWorkerRunning);
        }

        private void WorkerSettingsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteSettings.Enabled = workerSettingsListBox.SelectedIndex >= 0;
        }

        private void BtnResetSettings_Click(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.ResetWorkerSettings();
            MainForm.Instance.Settings.ResetWorkerPriority();
            workerMaximumCount.Value = MainForm.Instance.Settings.WorkerMaximumCount;
            UpdateWorkerSettingsListBox();
            CbJobType_SelectedIndexChanged(null, null);
            MainForm.Instance.Jobs.AdjustWorkerCount(MainForm.Instance.Jobs.IsAnyWorkerRunning);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        bool doNotSave = false;
        private void CbJobType_SelectedIndexChanged(object sender, EventArgs e)
        {
            doNotSave = true;
            JobType oType = (JobType)((EnumProxy)cbJobType.SelectedItem).RealValue;
            foreach (WorkerPriority oPriority in MainForm.Instance.Settings.WorkerPriority)
            {
                if (oType == oPriority.JobType)
                {
                    cbProcessPriority.SelectedIndex = (int)oPriority.Priority;
                    cbIOPriority.SelectedIndex = oPriority.LowIOPriority ? 0 : 1;
                }
            }
            doNotSave = false;
        }

        private void CbProcessPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (doNotSave)
                return;

            JobType oType = (JobType)((EnumProxy)cbJobType.SelectedItem).RealValue;
            foreach (WorkerPriority oPriority in MainForm.Instance.Settings.WorkerPriority)
            {
                if (oType == oPriority.JobType)
                {
                    oPriority.Priority = (WorkerPriorityType)cbProcessPriority.SelectedIndex;
                }
            }
        }

        private void CbIOPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (doNotSave)
                return;

            JobType oType = (JobType)((EnumProxy)cbJobType.SelectedItem).RealValue;
            foreach (WorkerPriority oPriority in MainForm.Instance.Settings.WorkerPriority)
            {
                if (oType == oPriority.JobType)
                {
                    oPriority.LowIOPriority = (cbIOPriority.SelectedIndex == 0);
                }
            }
        }

        private void CbAutoStartOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.WorkerAutoStartOnStartup = cbAutoStartOnStartup.Checked;
        }

        private void CbAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.WorkerAutoStart = cbAutoStart.Checked;
        }

        private void CbRemoveJob_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.WorkerRemoveJob = cbRemoveJob.Checked;
        }
    }
}

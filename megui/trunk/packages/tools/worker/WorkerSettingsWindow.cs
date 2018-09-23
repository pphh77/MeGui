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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeGUI
{
    public partial class WorkerSettingsWindow : Form
    {
        public WorkerSettingsWindow()
        {
            InitializeComponent();

            workerMaximumCount.Value = MainForm.Instance.Settings.WorkerMaximumCount;
            workerJobsListBox.DataSource = EnumProxy.CreateArray(new object[] { WorkerSettings.JobTyp.Audio, WorkerSettings.JobTyp.Demuxer, WorkerSettings.JobTyp.Indexer, WorkerSettings.JobTyp.Muxer, WorkerSettings.JobTyp.OneClick, WorkerSettings.JobTyp.Video });
            UpdateWorkerSettingsListBox();
        }

        private void UpdateWorkerSettingsListBox()
        {
            workerSettingsListBox.Items.Clear();
            foreach (WorkerSettings oSettings in MainForm.Instance.Settings.WorkerSettings)
            {
                string strText = string.Empty;
                foreach (WorkerSettings.JobTyp oTyp in oSettings.JobTypes)
                {
                    strText += oTyp.ToString() + " Jobs + ";
                }
                strText = strText.Substring(0, strText.Length - 2) + " <= " + oSettings.MaxCount + " Job(s) in parallel";
                workerSettingsListBox.Items.Add(strText);
            }
        }

        private void btnAddSettings_Click(object sender, EventArgs e)
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
                EnumProxy x = (EnumProxy)workerJobsListBox.Items[i];
                WorkerSettings.JobTyp oTyp = (WorkerSettings.JobTyp)x.RealValue;
                oSettings.JobTypes.Add(oTyp);
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

        private void btnDeleteSettings_Click(object sender, EventArgs e)
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

        private void workerMaximumCount_ValueChanged(object sender, EventArgs e)
        {
            if (MainForm.Instance.Settings.WorkerMaximumCount == (int)workerMaximumCount.Value)
                return;

            MainForm.Instance.Settings.WorkerMaximumCount = (int)workerMaximumCount.Value;
            MainForm.Instance.Jobs.AdjustWorkerCount(MainForm.Instance.Jobs.IsAnyWorkerRunning);
        }

        private void workerSettingsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteSettings.Enabled = workerSettingsListBox.SelectedIndex >= 0;
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.WorkerMaximumCount = 2;
            workerMaximumCount.Value = 2;

            MainForm.Instance.Settings.WorkerSettings.Clear();
            MainForm.Instance.Settings.WorkerSettings.Add(new WorkerSettings(1, new List<MeGUI.WorkerSettings.JobTyp>() { MeGUI.WorkerSettings.JobTyp.Audio }));
            MainForm.Instance.Settings.WorkerSettings.Add(new WorkerSettings(1, new List<MeGUI.WorkerSettings.JobTyp>() { MeGUI.WorkerSettings.JobTyp.Audio, MeGUI.WorkerSettings.JobTyp.Demuxer, MeGUI.WorkerSettings.JobTyp.Indexer, MeGUI.WorkerSettings.JobTyp.Muxer }));
            MainForm.Instance.Settings.WorkerSettings.Add(new WorkerSettings(1, new List<MeGUI.WorkerSettings.JobTyp>() { MeGUI.WorkerSettings.JobTyp.OneClick }));
            MainForm.Instance.Settings.WorkerSettings.Add(new WorkerSettings(1, new List<MeGUI.WorkerSettings.JobTyp>() { MeGUI.WorkerSettings.JobTyp.Video }));

            UpdateWorkerSettingsListBox();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}

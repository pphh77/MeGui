// ****************************************************************************
// 
// Copyright (C) 2005-2016 Doom9 & al
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
using System.IO;
using System.Windows.Forms;

using MeGUI.core.plugins.interfaces;
using MeGUI.core.util;

namespace MeGUI
{
    public partial class VobSubIndexWindow : Form
    {
        #region variables
        private bool dialogMode = false;
        private bool configured = false;
        #endregion

        #region start / stop
        public VobSubIndexWindow(MainForm mainForm)
        {
            InitializeComponent();
            this.chkSingleFileExport.Checked = MainForm.Instance.Settings.VobSubberSingleFileExport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        #endregion

        #region button handlers
        private void queueButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(output.Filename))
            {
                MessageBox.Show("Please select a propper output file", "Configuration Incomplete", MessageBoxButtons.OK);
                return;
            }

            if (!String.IsNullOrEmpty(output.Filename) && Drives.ableToWriteOnThisDrive(Path.GetPathRoot(output.Filename)))
            {
                if (configured)
                {
                    if (!dialogMode)
                    {
                        SubtitleIndexJob job = generateJob();
                        MainForm.Instance.Jobs.addJobsToQueue(job);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                    }
                }
                else
                    MessageBox.Show("You must select an Input and Output file to continue",
                        "Configuration incomplete", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("MeGUI cannot write on " + Path.GetPathRoot(output.Filename) +
                                ". Please select a propper output file.", "Configuration Incomplete", MessageBoxButtons.OK);
        }
        #endregion

        private void openVideo(string fileName)
        {
            input.Filename = fileName;
            subtitleTracks.Items.Clear();
            uint nbPGC = IFOparser.getPGCnb(fileName);
            pgc.Maximum = nbPGC;
            pgc.Enabled = (nbPGC > 1);
            subtitleTracks.Items.AddRange(IFOparser.GetSubtitlesStreamsInfos(input.Filename, Convert.ToInt32(pgc.Value), chkShowAllStreams.Checked));
            PreselectItems();
        }

        /// <summary>
        /// Selects the subtitles based on the language and options
        /// </summary>
        /// <returns></returns>
        private void PreselectItems()
        {
            // (un)select all based on keepAllTracks
            for (int i = 0; i < subtitleTracks.Items.Count; i++)
            {
                subtitleTracks.SetItemChecked(i, keepAllTracks.Checked);
            }

            // no need to check further if all tracks should be selected
            if (keepAllTracks.Checked)
                return;

            // check if any of the tracks should be selected based on the default MeGUI language(s)
            int x = -1;
            List<int> checkedItems = new List<int>();
            foreach (string item in subtitleTracks.Items)
            {
                x++;
                string[] temp = item.Split(new string[] { " - " }, StringSplitOptions.None);
                if (temp.Length < 2)
                    continue;

                if (temp[1].ToLowerInvariant().Trim().Equals(MainForm.Instance.Settings.DefaultLanguage1.ToLowerInvariant())
                    || temp[1].ToLowerInvariant().Trim().Equals(MainForm.Instance.Settings.DefaultLanguage2.ToLowerInvariant()))
                    checkedItems.Add(x);

            }
            foreach (int idx in checkedItems)
            {
                subtitleTracks.SetItemChecked(idx, true);
            }
        }

        private void checkIndexIO()
        {
            configured = (!input.Filename.Equals("") && !output.Filename.Equals(""));
            if (configured && dialogMode)
                queueButton.DialogResult = DialogResult.OK;
            else
                queueButton.DialogResult = DialogResult.None;
        }

        private SubtitleIndexJob generateJob()
        {
            List<int> trackIDs = new List<int>();
            foreach (string s in subtitleTracks.CheckedItems)
            {
                trackIDs.Add(Int32.Parse(s.Substring(1,2)));
            }
            return new SubtitleIndexJob(input.Filename, output.Filename, keepAllTracks.Checked, trackIDs, (int)pgc.Value, chkSingleFileExport.Checked);
        }

        public void setConfig(string input, string output, bool indexAllTracks, List<int> trackIDs, int pgc)
        {
            this.dialogMode = true;
            queueButton.Text = "Update";
            this.input.Filename = input;
            this.pgc.Value = pgc;
            openVideo(input);
            this.output.Filename = output;
            checkIndexIO();
            if (indexAllTracks)
                keepAllTracks.Checked = true;
            else
            {
                demuxSelectedTracks.Checked = true;
                int index = 0;
                List<int> checkedItems = new List<int>();
                foreach (object item in subtitleTracks.Items)
                {
                    SubtitleInfo si = (SubtitleInfo)item;
                    if (trackIDs.Contains(si.Index))
                        checkedItems.Add(index);
                    index++;
                }
                foreach (int idx in checkedItems)
                {
                    subtitleTracks.SetItemChecked(idx, true);
                }
            }
        }

        /// <summary>
        /// gets the index job created from the current configuration
        /// </summary>
        public SubtitleIndexJob Job
        {
            get { return generateJob(); }
        }

        private void input_FileSelected(FileBar sender, FileBarEventArgs args)
        {
            this.pgc.Value = 1;
            openVideo(input.Filename);

            // get proper pre- and postfix based on the input file
            string filePath = FileUtil.GetOutputFolder(input.Filename);
            string filePrefix = FileUtil.GetOutputFilePrefix(input.Filename);
            string fileName = Path.GetFileNameWithoutExtension(input.Filename);
            if (FileUtil.RegExMatch(fileName, @"_\d{1,2}\z", false))
                fileName = fileName.Substring(0, fileName.LastIndexOf('_') + 1);
            else
                fileName = fileName + "_";
            output.Filename = Path.Combine(filePath, filePrefix + fileName + this.pgc.Value + ".idx");

            checkIndexIO();
        }

        private void output_FileSelected(FileBar sender, FileBarEventArgs args)
        {
            checkIndexIO();
        }

        private void chkShowAllStreams_CheckedChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(input.Filename))
                return;

            openVideo(input.Filename);
            checkIndexIO();
            keepAllTracks_CheckedChanged(null, null);
        }

        private void pgc_ValueChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(input.Filename))
                return;

            openVideo(input.Filename);
            
            // check if the PGC number has to be changed in the output file name
            string fileName = Path.GetFileNameWithoutExtension(output.Filename);
            if (FileUtil.RegExMatch(fileName, @"_\d{1,2}\z", false))
            {
                // file ends with e.g. _11 as in VTS_01_11
                fileName = fileName.Substring(0, fileName.LastIndexOf('_') + 1) + this.pgc.Value + ".idx";
                output.Filename = Path.Combine(FileUtil.GetOutputFolder(output.Filename), fileName);
            }

            checkIndexIO();
        }

        private void chkSingleFileExport_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.VobSubberSingleFileExport = this.chkSingleFileExport.Checked;
        }

        private void keepAllTracks_CheckedChanged(object sender, EventArgs e)
        {
            subtitleTracks.Enabled = !keepAllTracks.Checked;
            PreselectItems();
        }
    }

    public class VobSubTool : ITool
    {

        #region ITool Members

        public string Name
        {
            get { return "VobSubber"; }
        }

        /// <summary>
        /// launches the vobsub indexer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Run(MainForm info)
        {
            new VobSubIndexWindow(info).Show();
        }

        public Shortcut[] Shortcuts
        {
            get { return new Shortcut[] { Shortcut.CtrlN }; }
        }

        #endregion

        #region IIDable Members

        public string ID
        {
            get { return "VobSubber"; }
        }

        #endregion
    }
}
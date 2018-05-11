// ****************************************************************************
// 
// Copyright (C) 2005-2018 Doom9 & al
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using MeGUI.core.util;

namespace MeGUI
{

	/// <summary>
	/// Summary description for ChapterCreator.
	/// </summary>
	public partial class ChapterCreator : Form
	{
		private string videoInput;
		private VideoPlayer player;
		private int introEndFrame = 0, creditsStartFrame = 0;
        private ChapterInfo pgc;
        private int intIndex;

		#region start / stop
		public ChapterCreator()
		{
			InitializeComponent();
            intIndex = 0;
            pgc = new ChapterInfo();
            pgc.FramesPerSecond = 23.976;
            chkCounter.Checked = MainForm.Instance.Settings.ChapterCreatorCounter;
        }

        private void ChapterCreator_Load(object sender, EventArgs e)
        {
            if (OSInfo.IsWindowsVistaOrNewer)
                OSInfo.SetWindowTheme(chapterListView.Handle, "explorer", null);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (player != null)
                player.Close();
            base.OnClosing(e);
        }
		#endregion

		#region helper methods
        /// <summary>
        /// Recreates the chapter view
        /// </summary>
        private void ResetChapterView()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                this.chapterListView.Items.Clear();
                
                //fill list
                foreach (Chapter c in pgc.Chapters)
                {
                    string strTime = GetStringFromTimeSpan(c.Time);
                    ListViewItem item = new ListViewItem(new string[] { strTime, c.Name });
                    chapterListView.Items.Add(item);
                    if (item.Index % 2 != 0)
                        item.BackColor = Color.White;
                    else
                        item.BackColor = Color.FromArgb(255, 245, 245, 245);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Gets the string of a time span
        /// </summary>
        /// <param name="ts">the TimeSpan to convert</param>
        /// <returns>the result string</returns>
        private string GetStringFromTimeSpan(TimeSpan ts)
        {
            string strTime = ts.ToString();
            if (ts.Milliseconds == 0)
                strTime = strTime + ".000";
            else if (strTime.Length > 12)
                strTime = strTime.Substring(0, 12);
            return strTime;
        }

        /// <summary>
        /// Gets the time span from a string
        /// </summary>
        /// <param name="strTime">the string to convert</param>
        /// <param name="ts">the output time span</param>
        /// <returns>true if the value can be converted</returns>
        private bool GetTimeSpanFromString(string strTime, out TimeSpan ts)
        {
            if (!TimeSpan.TryParse(strTime, out ts))
                return false;

            if (ts.Days > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the chapter name (with automatic counter if enabled)
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns>the chapter name</returns>
        private string GetChapterName(int iIndex)
        {
            string strChapterName = chapterName.Text;
            if (chkCounter.Checked)
                strChapterName = strChapterName + " " + (iIndex + 1).ToString("00");
            return strChapterName;
        }
		#endregion

		#region buttons
		private void removeZoneButton_Click(object sender, System.EventArgs e)
		{
            if (chapterListView.Items.Count < 1 || pgc.Chapters.Count < 1 || chapterListView.SelectedIndices.Count == 0)
                return;

            intIndex = chapterListView.SelectedIndices[0];
            pgc.Chapters.Remove(pgc.Chapters[intIndex]);
            if (intIndex != 0)
                intIndex--;

            ResetChapterView();
		}

		private void clearZonesButton_Click(object sender, System.EventArgs e)
		{
            pgc.Chapters.Clear();
            intIndex = 0;

            ResetChapterView();
        }

		private void chapterListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if (chapterListView.Items.Count < 1)
                return;

            chapterName.TextChanged -= new System.EventHandler(this.chapterName_TextChanged);
            startTime.TextChanged -= new System.EventHandler(this.chapterName_TextChanged);

            ListView lv = (ListView)sender;
            if (lv.SelectedItems.Count == 1)
                intIndex = lv.SelectedItems[0].Index;

            if (pgc.HasChapters)
            {
                this.startTime.Text = FileUtil.ToShortString(pgc.Chapters[intIndex].Time);
                string strChapterName = pgc.Chapters[intIndex].Name;
                if (chkCounter.Checked && FileUtil.RegExMatch(strChapterName, @" \d{2}$", false))
                    strChapterName = strChapterName.Substring(0, strChapterName.Length - 3);
                this.chapterName.Text = strChapterName;
            }

            chapterName.TextChanged += new System.EventHandler(this.chapterName_TextChanged);
            startTime.TextChanged += new System.EventHandler(this.chapterName_TextChanged); 
		}

		private void addZoneButton_Click(object sender, System.EventArgs e)
		{
            TimeSpan ts = new TimeSpan(0);
            if (!GetTimeSpanFromString(startTime.Text, out ts))
            { 
                // invalid time input
                startTime.Focus();
                startTime.SelectAll();
                MessageBox.Show("Cannot parse the timecode you have entered.\nIt must be given in the hh:mm:ss.ccc format", 
                                    "Incorrect timecode", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            GetStringFromTimeSpan(ts);

            intIndex = 0;
            foreach (Chapter oChapter in pgc.Chapters)
            {
                if (oChapter.Time == ts)
                    return;
                else if (oChapter.Time > ts)
                    break;
                else
                    intIndex++;
            }

            // create a new chapter
            Chapter c = new Chapter() { Time = ts, Name = GetChapterName(intIndex) };
            pgc.Chapters.Insert(intIndex, c);

            ResetChapterView();
		}
		#endregion

		#region saving files
		private void saveButton_Click(object sender, System.EventArgs e)
		{
            if (String.IsNullOrEmpty(output.Text))
            {
                MessageBox.Show("Please select the output file first", "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fpsChooserIn.Enabled && rbQPF.Checked)
            {
                if (MessageBox.Show("The FPS value for the input file is unknown. Please make sure that the correct value for the input is selected.\nCurrently " +
                    fpsChooserIn.Value + " will be applied.\n\nDo you want to continue?", "FPS unknown", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(output.Text)))
            {
                btOutput_Click(null, null);
                if (!Directory.Exists(Path.GetDirectoryName(output.Text)))
                    return;
            }

            if (!FileUtil.IsDirWriteable(Path.GetDirectoryName(output.Text)))
            {
                MessageBox.Show("MeGUI cannot write to the path " + Path.GetDirectoryName(output.Text) + "\n" +
                                    "Please select another output path to save your file.", "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double fps = pgc.FramesPerSecond;
            pgc.ChangeFps((double)fpsChooserOut.Value);
            if (rbQPF.Checked)
                pgc.SaveQpfile(output.Text);
            else if (rbXML.Checked)
                pgc.SaveXml(output.Text);
            else
                pgc.SaveText(output.Text);
            pgc.ChangeFps(fps);

            if (this.closeOnQueue.Checked)
                this.Close();
        }

        private void btOutput_Click(object sender, EventArgs e)
        {
            if (rbXML.Checked)
            {
                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.Filter = "Matroska Chapters files (*.xml)|*.xml";
            }
            else if (rbQPF.Checked)
            {
                saveFileDialog.DefaultExt = "qpf";
                saveFileDialog.Filter = "x264 qp Files (*.qpf)|*.qpf";
            }
            else
            {
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Chapter Files (*.txt)|*.txt";
            }
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = output.Text;

            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!FileUtil.IsDirWriteable(Path.GetDirectoryName(saveFileDialog.FileName)))
                    MessageBox.Show("MeGUI cannot write to the path " + Path.GetDirectoryName(saveFileDialog.FileName) + "\n" +
                "Please select another output path to save your file.", "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    output.Text = saveFileDialog.FileName;
            }
        }
		#endregion

        private void btInput_Click(object sender, EventArgs e)
        {
            if (rbFromFile.Checked)
            {
                openFileDialog.Filter = "IFO files (*.ifo)|*.ifo|Container files (*.mkv,*.mp4)|*.mkv;*.mp4|MPLS files (*.mpls)|*.mpls|Chapter files (*.txt,*.xml)|*.txt;*.xml|All supported files (*.ifo,*.mkv,*.mp4,*.mpls,*.txt,*.xml)|*.ifo;*.mkv;*.mp4;*.mpls;*.txt;*.xml";
                openFileDialog.FilterIndex = 5;

                if (this.openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                input.Text = openFileDialog.FileName;
            }
            else
            {
                using (FolderBrowserDialog d = new FolderBrowserDialog())
                {
                    d.ShowNewFolderButton = false;
                    d.Description = "Select DVD/BluRay disc or folder";
                    if (d.ShowDialog() != DialogResult.OK)
                        return;

                    input.Text = d.SelectedPath;
                }
            }

            using (frmStreamSelect frm = new frmStreamSelect(input.Text))
            {
                if (frm.TitleCount == 0)
                {
                    MessageBox.Show("No chapters found");
                    return;
                }

                DialogResult dr = DialogResult.OK;
                if (frm.TitleCountWithRequiredLength != 1)
                    dr = frm.ShowDialog();

                if (dr != DialogResult.OK)
                    return;

                pgc = frm.SelectedSingleChapterInfo;
            }

            bNoUpdates = true;
            if (pgc.FramesPerSecond > 0)
            {
                fpsChooserIn.Value = fpsChooserOut.Value = (decimal)pgc.FramesPerSecond;
                fpsChooserIn.Enabled = false;
            }
            else
            {
                fpsChooserIn.Value = fpsChooserOut.Value = (decimal)23.976;
                pgc.FramesPerSecond = (double)23.976;
                fpsChooserIn.Enabled = true;
            }
            bNoUpdates = false;

            ResetChapterView();

            chaptersGroupbox.Text = " Chapters ";
            if (chapterListView.Items.Count != 0)
                chapterListView.Items[0].Selected = true;

            string fileName = Path.GetFileNameWithoutExtension(input.Text);
            if (this.pgc.PGCNumber > 0)
            {
                chaptersGroupbox.Text += "- VTS " + pgc.TitleNumber.ToString("D2") + " - PGC " + pgc.PGCNumber.ToString("D2") + " ";
                if (FileUtil.RegExMatch(fileName, @"_\d{1,2}\z", false))
                {
                    // file ends with e.g. _1 as in VTS_01_1
                    fileName = fileName.Substring(0, fileName.LastIndexOf('_'));
                    fileName += "_" + this.pgc.PGCNumber;
                }
            }

            fileName = FileUtil.GetOutputFilePrefix(input.Text) + fileName + " - Chapter Information.txt";
            if (rbXML.Checked)
                fileName = Path.ChangeExtension(fileName, "xml");
            else if (rbQPF.Checked)
                fileName = Path.ChangeExtension(fileName, "qpf");

            output.Text = Path.Combine(FileUtil.GetOutputFolder(input.Text), fileName);
        }

        bool bNoUpdates = false;
        private void fpsChooserIn_SelectionChanged(object sender, string val)
        {
            if (bNoUpdates || pgc.FramesPerSecond == (double)fpsChooserIn.Value)
                return;

            if (fpsChooserIn.Enabled)
            {
                pgc.ChangeFps((double)fpsChooserIn.Value);
                ResetChapterView();
            }
            else
                pgc.FramesPerSecond = (double)fpsChooserIn.Value;
            fpsChooserOut.Value = fpsChooserIn.Value;
        }

        private void showVideoButton_Click(object sender, System.EventArgs e)
		{
            if (String.IsNullOrEmpty(this.videoInput) || player == null)
            {
                using (OpenFileDialog d = new OpenFileDialog())
                {
                    d.Filter = "AviSynth Script|*.avs";
                    d.Multiselect = false;
                    if (!String.IsNullOrEmpty(this.videoInput))
                        d.FileName = videoInput;
                    if (d.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("Please configure video input first", "No video input found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    this.videoInput = d.FileName;
                }
            }

            if (player == null)
				player = new VideoPlayer();

			bool videoLoaded = player.loadVideo(MainForm.Instance, videoInput, PREVIEWTYPE.CHAPTERS, false);
			if (videoLoaded)
			{
				player.Closed += new EventHandler(player_Closed);
				player.ChapterSet += new ChapterSetCallback(player_ChapterSet);
				if (introEndFrame > 0)
					player.IntroEnd = this.introEndFrame;
				if (creditsStartFrame > 0)
					player.CreditsStart = this.creditsStartFrame;
                player.Show();
                player.SetScreenSize();
                this.TopMost = player.TopMost = true;
                if (!MainForm.Instance.Settings.AlwaysOnTop)
                    player.TopMost = false;
                
                bNoUpdates = true;
                pgc.FramesPerSecond = player.Framerate;
                fpsChooserIn.Value = fpsChooserOut.Value = (decimal)player.Framerate;
                fpsChooserIn.Enabled = false;
                bNoUpdates = false;
            }
			else
				return;

            if (chapterListView.SelectedItems.Count == 1 && chapterListView.SelectedItems[0].Tag != null) 
			{
                // a zone has been selected, show that zone
                Chapter chap = (Chapter)chapterListView.SelectedItems[0].Tag;
				double framerate = player.Framerate;
                int frameNumber = Util.ConvertTimecodeToFrameNumber(chap.Time, framerate);
				player.CurrentFrame = frameNumber;
			}
			else if (TimeSpan.TryParse(startTime.Text, new System.Globalization.CultureInfo("en-US"), out TimeSpan result))
            {
                // show the first chapter if it is not 00:00:00.000
                if (result.TotalMilliseconds != 0)
                {
                    int frameNumber = Util.ConvertTimecodeToFrameNumber(result, player.Framerate);
                    player.CurrentFrame = frameNumber;
                }
            }
		}

		#region properties
		/// <summary>
		/// sets the video input to be used for a zone preview
		/// </summary>
		public string VideoInput
		{
			set { this.videoInput = value; }
		}

		/// <summary>
		/// gets / sets the start frame of the credits
		/// </summary>
		public int CreditsStartFrame
		{
			get { return this.creditsStartFrame; }
			set {creditsStartFrame = value; }
		}

		/// <summary>
		/// gets / sets the end frame of the intro
		/// </summary>
		public int IntroEndFrame
		{
			get { return this.introEndFrame; }
			set { introEndFrame = value; }
		}
		#endregion

		private void player_Closed(object sender, EventArgs e)
		{
            this.TopMost = false;
            player = null;
		}

		private void player_ChapterSet(int frameNumber)
		{
            startTime.Text = Util.converFrameNumberToTimecode(frameNumber, player.Framerate);
            addZoneButton_Click(null, null);
		}

        private void chapterName_TextChanged(object sender, EventArgs e)
        {
            if (chapterListView.SelectedIndices.Count == 0)
                return;

            TimeSpan ts = new TimeSpan(0);
            if (!GetTimeSpanFromString(startTime.Text, out ts))
                return;
            string strTime = GetStringFromTimeSpan(ts);

            intIndex = chapterListView.SelectedIndices[0];
            pgc.Chapters[intIndex] = new Chapter()
            {
                Time = ts,
                Name = GetChapterName(intIndex)
            };

            chapterListView.SelectedItems[0].SubItems[0].Text = strTime;
            chapterListView.SelectedItems[0].SubItems[1].Text = GetChapterName(intIndex);
        }

        private void rbTXT_CheckedChanged(object sender, EventArgs e)
        {
            output.Text = Path.ChangeExtension(output.Text, "txt");
        }

        private void rbQPF_CheckedChanged(object sender, EventArgs e)
        {
            output.Text = Path.ChangeExtension(output.Text, "qpf");
        }

        private void startTime_TextChanged(object sender, EventArgs e)
        {
            TimeSpan ts = new TimeSpan(0);
            if (!GetTimeSpanFromString(startTime.Text, out ts))
                startTime.ForeColor = Color.Red;
            else
                startTime.ForeColor = Color.Black;
        }

        private void chapterListView_DoubleClick(object sender, EventArgs e)
        {
            if (player == null || chapterListView.SelectedIndices.Count == 0)
                return;

            player.CurrentFrame = Util.ConvertTimecodeToFrameNumber(pgc.Chapters[chapterListView.SelectedIndices[0]].Time, pgc.FramesPerSecond);
        }

        private void chkCounter_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Instance.Settings.ChapterCreatorCounter = chkCounter.Checked;
        }

        private void rbXML_CheckedChanged(object sender, EventArgs e)
        {
            output.Text = Path.ChangeExtension(output.Text, "xml");
        }
	}

    [Serializable]
	public struct Chapter
	{
        public string Name { get; set; }

        [XmlIgnore]
        public TimeSpan Time { get; set; }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long TimeTicks
        {
            get { return Time.Ticks; }
            set { Time = new TimeSpan(value); }
        }

        //public string Lang { get; set; }
        public override string ToString()
        {
            return Time.ToString() + ": " + Name;
        }

        public void SetTimeBasedOnString(string strTimeCode)
        {
            if (strTimeCode.Length > 16)
                strTimeCode = strTimeCode.Substring(0, 16);

            if (TimeSpan.TryParse(strTimeCode, new System.Globalization.CultureInfo("en-US"), out TimeSpan result))
                Time = result;
        }
	}

    public class ChapterCreatorTool : MeGUI.core.plugins.interfaces.ITool
    {

        #region ITool Members

        public string Name
        {
            get { return "Chapter Creator"; }
        }

        public void Run(MainForm info)
        {
            ChapterCreator cc = new ChapterCreator();
            cc.VideoInput = info.Video.Info.VideoInput;
            cc.CreditsStartFrame = info.Video.Info.CreditsStartFrame;
            cc.IntroEndFrame = info.Video.Info.IntroEndFrame;
            cc.Show();
        }

        public Shortcut[] Shortcuts
        {
            get { return new Shortcut[] { Shortcut.CtrlH }; }
        }

        #endregion

        #region IIDable Members

        public string ID
        {
            get { return "chapter_creator"; }
        }

        #endregion
    }
}
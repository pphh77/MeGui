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
using System.IO;
using System.Text;
using System.Windows.Forms;

using MeGUI.core.details;
using MeGUI.core.util;

namespace MeGUI
{
    /// <summary>
    /// Summary description for File Indexer.
    /// </summary>
    public partial class FileIndexerWindow : Form
    {
        public enum IndexType
        {
            AVISOURCE, D2V, DGM, DGI, FFMS, LSMASH, NONE
        };

        #region variables
        private LogItem _oLog;
        private IndexType IndexerUsed = IndexType.D2V;
        private string strVideoCodec = "";
        private string strVideoScanType = "";
        private string strContainerFormat = "";
        private List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
        private bool dialogMode = false; // $%£%$^>*"%$%%$#{"!!! Affects the public behaviour!
        private bool configured = false;
        private MediaInfoFile iFile;
        #endregion

        #region start / stop
        public void setConfig(string input, string projectName, int demuxType,
            bool showCloseOnQueue, bool closeOnQueue, bool loadOnComplete, bool updateMode)
        {
            openVideo(input);
            if (!string.IsNullOrEmpty(projectName))
                this.output.Text = projectName;
            if (demuxType == 0)
                demuxNoAudiotracks.Checked = true;
            else
                demuxAll.Checked = true;
            this.loadOnComplete.Checked = loadOnComplete;
            if (updateMode)
            {
                this.dialogMode = true;
                queueButton.Text = "Update";
            }
            else
                this.dialogMode = false;
            checkIndexIO();
            if (!showCloseOnQueue)
            {
                this.closeOnQueue.Hide();
                this.Controls.Remove(this.closeOnQueue);
            }
            this.closeOnQueue.Checked = closeOnQueue;
        }

        public FileIndexerWindow(MainForm mainForm)
        {
            InitializeComponent();
            CheckDGIIndexer();
        }

        public FileIndexerWindow(MainForm mainForm, string fileName) : this(mainForm)
        {
            CheckDGIIndexer();
            openVideo(fileName);
        }

        public FileIndexerWindow(MainForm mainForm, string fileName, bool autoReturn) : this(mainForm, fileName)
        {
            CheckDGIIndexer();
            openVideo(fileName);
            this.loadOnComplete.Checked = true;
            this.closeOnQueue.Checked = true;
            checkIndexIO();
        }

        private void CheckDGIIndexer()
        {
            string filter = "All DGIndex supported files|*.vob;*.mpg;*.mpeg;*.m1v;*.m2v;*.mpv;*.tp;*.ts;*.trp;*.m2t;*.m2ts;*.pva;*.vro;*.ifo";
            filter += "|All FFMS Indexer supported files|*.mkv;*.avi;*.mp4;*.flv;*.wmv;*.ogm;*.vob;*.mpg;*.m2ts;*.ts;*.ifo";
            filter += "|All LSMASH Indexer supported files|*.mkv;*.avi;*.mp4;*.flv;*.wmv;*.ogm;*.vob;*.mpg;*.m2ts;*.ts;*.ifo";
            if (MainForm.Instance.Settings.IsDGIIndexerAvailable() || MainForm.Instance.Settings.IsDGMIndexerAvailable())
            {
                if (MainForm.Instance.Settings.IsDGIIndexerAvailable())
                    filter += "|All DGIndexNV supported files|*.264;*.h264;*.avc;*.m2v;*.mpv;*.vc1;*.mkv;*.vob;*.mp4;*.mpg;*.mpeg;*.m2t;*.m2ts;*.mts;*.tp;*.ts;*.trp;*.ifo";
                if (MainForm.Instance.Settings.IsDGMIndexerAvailable())
                    filter += "|All DGIndexIM supported files|*.264;*.h264;*.avc;*.m2v;*.mpv;*.vc1;*.mkv;*.vob;*.mp4;*.mpg;*.mpeg;*.m2t;*.m2ts;*.mts;*.tp;*.ts;*.trp;*.ifo";
                filter += "|All supported files|*.mkv;*.avi;*.mp4;*.flv;*.wmv;*.ogm;*.264;*.h264;*.avc;*.m2t*;*.m2ts;*.mts;*.tp;*.ts;*.trp;*.vob;*.mpg;*.mpeg;*.m1v;*.m2v;*.mpv;*.pva;*.vro;*.vc1;*.ifo";
                filter += "|All files|*.*";
                input.Filter = filter;
                if (MainForm.Instance.Settings.IsDGIIndexerAvailable() && MainForm.Instance.Settings.IsDGMIndexerAvailable())
                    input.FilterIndex = 6;
                else
                    input.FilterIndex = 5;
            }
            else
            {
                filter += "|All supported files|*.mkv;*.avi;*.mp4;*.flv;*.wmv;*.ogm;*.264;*.h264;*.avc;*.m2t*;*.m2ts;*.mts;*.tp;*.ts;*.trp;*.vob;*.mpg;*.mpeg;*.m1v;*.m2v;*.mpv;*.pva;*.vro;*.ifo";
                filter += "|All files|*.*";
                input.Filter = filter;
                input.FilterIndex = 4;
            }
        }

        private void changeIndexer(IndexType dgType)
        {
            switch (dgType)
            {
                case IndexType.DGI:
                    {
                        this.saveProjectDialog.Filter = "DGIndexNV project files|*.dgi";
                        if (this.demuxTracks.Checked)
                            this.demuxAll.Checked = true;
                        this.demuxTracks.Enabled = false;
                        //this.gbAudio.Enabled = true;
                        this.gbAudio.Text = " Audio Demux ";
                        this.gbOutput.Enabled = true;
                        this.demuxVideo.Enabled = true;
                        IndexerUsed = IndexType.DGI;
                        btnDGI.Checked = true;
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA"))
                            generateAudioList();
                        break;
                    }
                case IndexType.DGM:
                    {
                        this.saveProjectDialog.Filter = "DGIndexIM project files|*.dgi";
                        if (this.demuxTracks.Checked)
                            this.demuxAll.Checked = true;
                        this.demuxTracks.Enabled = false;
                        //this.gbAudio.Enabled = true;
                        this.gbAudio.Text = " Audio Demux ";
                        this.gbOutput.Enabled = true;
                        this.demuxVideo.Enabled = true;
                        IndexerUsed = IndexType.DGM;
                        btnDGM.Checked = true;
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA"))
                            generateAudioList();
                        break;
                    }
                case IndexType.D2V:
                    {
                        this.saveProjectDialog.Filter = "DGIndex project files|*.d2v";
                        this.demuxTracks.Enabled = true;
                        //this.gbOutput.Enabled = true;
                        this.gbAudio.Text = " Audio Demux ";
                        this.gbAudio.Enabled = true;
                        this.demuxVideo.Enabled = true;
                        IndexerUsed = IndexType.D2V;
                        btnD2V.Checked = true;
                        break;
                    }
                case IndexType.FFMS:
                    {
                        this.saveProjectDialog.Filter = "FFMSIndex project files|*.ffindex";
                        //this.gbOutput.Enabled = false;
                        this.gbAudio.Enabled = true;
                        if (this.demuxTracks.Checked)
                            this.demuxAll.Checked = true;
                        this.demuxTracks.Enabled = true;
                        this.demuxVideo.Checked = false;
                        this.demuxVideo.Enabled = false;
                        IndexerUsed = IndexType.FFMS;
                        btnFFMS.Checked = true;
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA"))
                        {
                            generateAudioList();
                            this.gbAudio.Text = " Audio Demux ";
                        }
                        else
                            this.gbAudio.Text = " Audio Encoding ";
                        break;
                    }
                case IndexType.LSMASH:
                    {
                        this.saveProjectDialog.Filter = "LSMASHIndex project files|*.lwi";
                        //this.gbOutput.Enabled = false;
                        this.gbAudio.Enabled = true;
                        if (this.demuxTracks.Checked)
                            this.demuxAll.Checked = true;
                        this.demuxTracks.Enabled = true;
                        this.demuxVideo.Checked = false;
                        this.demuxVideo.Enabled = false;
                        IndexerUsed = IndexType.LSMASH;
                        btnLSMASH.Checked = true;
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA"))
                        {
                            generateAudioList();
                            this.gbAudio.Text = " Audio Demux ";
                        }
                        else
                            this.gbAudio.Text = " Audio Encoding ";
                        break;
                    }
            }
            setOutputFileName();
            recommendSettings();
            if (!demuxTracks.Checked)
                rbtracks_CheckedChanged(null, null);
        }
        #endregion
        #region buttons
        private void pickOutputButton_Click(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(output.Text))
            {
                saveProjectDialog.InitialDirectory = Path.GetDirectoryName(output.Text);
                saveProjectDialog.FileName = Path.GetFileName(output.Text);
            }
            if (saveProjectDialog.ShowDialog() == DialogResult.OK)
            {
                output.Text = saveProjectDialog.FileName;
                checkIndexIO();
            }
        }

        private void input_FileSelected(FileBar sender, FileBarEventArgs args)
        {
            if (Path.GetExtension(input.Filename.ToUpperInvariant()) == ".VOB")
            {
                // switch to IFO if possible as a VOB file is used
                string videoIFO;
                if (Path.GetFileName(input.Filename).ToUpperInvariant().Substring(0, 4) == "VTS_")
                    videoIFO = input.Filename.Substring(0, input.Filename.LastIndexOf("_")) + "_0.IFO";
                else
                    videoIFO = Path.ChangeExtension(input.Filename, ".IFO");

                if (File.Exists(videoIFO))
                    input.Filename = videoIFO;
            }

            openVideo(input.Filename);
            checkIndexIO();
        }

        private void openVideo(string fileName)
        {
            this._oLog = MainForm.Instance.FileIndexerLog;
            if (_oLog == null)
            {
                _oLog = MainForm.Instance.Log.Info("FileIndexer");
                MainForm.Instance.FileIndexerLog = _oLog;
            }

            iFile = null;
            if (GetDVDSource(fileName, ref iFile))
            {
                if (iFile != null)
                    fileName = iFile.FileName;
            }
            else
                iFile = new MediaInfoFile(fileName, ref _oLog);

            if (iFile != null && iFile.HasVideo)
            {
                strVideoCodec = iFile.VideoInfo.Track.Codec;
                strVideoScanType = iFile.VideoInfo.ScanType;
                strContainerFormat = iFile.ContainerFileTypeString;
            }
            else
                strVideoCodec = strVideoScanType = strContainerFormat = string.Empty;

            if (String.IsNullOrEmpty(strVideoCodec))
                txtCodecInformation.Text = " unknown";
            else
                txtCodecInformation.Text = " " + strVideoCodec;
            if (String.IsNullOrEmpty(strContainerFormat))
                txtContainerInformation.Text = " unknown";
            else
                txtContainerInformation.Text = " " + strContainerFormat;
            if (String.IsNullOrEmpty(strVideoScanType))
                txtScanTypeInformation.Text = " unknown";
            else
                txtScanTypeInformation.Text = " " + strVideoScanType;

            if (iFile != null && iFile.HasAudio)
                audioTracks = iFile.AudioInfo.Tracks;
            else
                audioTracks = new List<AudioTrackInfo>();

            if (input.Filename != fileName)
                input.Filename = fileName;

            generateAudioList();

            if (iFile != null)
            {
                IndexType newType = IndexType.NONE;
                iFile.recommendIndexer(out newType, true);
                if (newType == IndexType.D2V || newType == IndexType.DGM ||
                    newType == IndexType.DGI || newType == IndexType.FFMS ||
                    newType == IndexType.LSMASH)
                {
                    btnD2V.Enabled = iFile.isD2VIndexable();
                    btnDGM.Enabled = iFile.isDGMIndexable();
                    btnDGI.Enabled = iFile.isDGIIndexable();
                    btnFFMS.Enabled = iFile.isFFMSIndexable();
                    btnLSMASH.Enabled = iFile.isLSMASHIndexable(true);

                    gbIndexer.Enabled = gbAudio.Enabled = gbOutput.Enabled = true;
                    changeIndexer(newType);
                    return;
                }
            }

            // source file not supported
            btnD2V.Enabled = btnDGM.Enabled = btnDGI.Enabled = btnFFMS.Enabled = btnLSMASH.Enabled = false;
            gbIndexer.Enabled = gbAudio.Enabled = gbOutput.Enabled = false;
            btnFFMS.Checked = btnD2V.Checked = btnDGM.Checked = btnDGI.Checked = btnLSMASH.Checked = false;
            output.Text = "";
            demuxNoAudiotracks.Checked = true;
            MessageBox.Show("No indexer for this file found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Checks if the source file if from a DVD stucture & asks for title list if needed
        /// </summary>
        /// <param name="fileName">input file name</param>
        /// <param name="iFileTemp">reference to the mediainfofile object</param>
        /// <returns>true if DVD source is found, false if no DVD source is available</returns>
        private bool GetDVDSource(string fileName, ref MediaInfoFile iFileTemp)
        {
            iFileTemp = null;
            using (frmStreamSelect frm = new frmStreamSelect(fileName, SelectionMode.One))
            {
                // check if playlists have been found
                if (frm.TitleCount == 0)
                    return false;

                // only continue if a DVD or Blu-ray structure is found
                if (!frm.IsDVDSource)
                    return false;

                // open the selection window if not exactly one title set with the desired minimum length is found
                DialogResult dr = DialogResult.OK;
                if (frm.TitleCountWithRequiredLength != 1)
                    dr = frm.ShowDialog();

                if (dr != DialogResult.OK)
                    return true;

                ChapterInfo oChapterInfo = frm.SelectedSingleChapterInfo;
                string strSourceFile = Path.Combine(oChapterInfo.SourcePath, oChapterInfo.Title + "_0.IFO");
                if (!File.Exists(strSourceFile))
                {
                    _oLog.LogEvent(strSourceFile + " cannot be found. skipping...");
                    return true;
                }
                iFileTemp = new MediaInfoFile(strSourceFile, ref _oLog, oChapterInfo.PGCNumber, oChapterInfo.AngleNumber);
            }
            return true;
        }

        private void generateAudioList()
        {
            AudioTracks.Items.Clear();

            foreach (AudioTrackInfo atrack in audioTracks)
                AudioTracks.Items.Add(atrack);
        }

        /// <summary>
        /// recommend input settings based upon the input file
        /// </summary>
        private void recommendSettings()
        {
            if (AudioTracks.Items.Count > 0)
            {
                if (IndexerUsed == IndexType.D2V)
                {
                    if (strContainerFormat.Equals("MPEG-PS"))
                    {
                        demuxTracks.Enabled = true;
                    }
                    else
                    {
                        if (demuxTracks.Checked)
                            demuxAll.Checked = true;
                        demuxTracks.Enabled = false;
                    }
                }
            }
            else
            {
                demuxNoAudiotracks.Checked = true;
                demuxTracks.Enabled = false;
            }
            AudioTracks.Enabled = demuxTracks.Checked;

            if (IndexerUsed == IndexType.FFMS)
            {
                if (!strContainerFormat.ToUpperInvariant().Equals("MATROSKA") &&
                    !strContainerFormat.ToUpperInvariant().Equals("AVI") &&
                    !strContainerFormat.ToUpperInvariant().Equals("MPEG-4") &&
                    !strContainerFormat.ToUpperInvariant().Equals("FLASH VIDEO"))
                {
                    MessageBox.Show("It is recommended to use a MKV, AVI, MP4 or FLV container to index files with the FFMS2 indexer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// sets the output file name
        /// </summary>
        private void setOutputFileName()
        {
            if (String.IsNullOrEmpty(this.input.Filename))
                return;

            string projectPath = FileUtil.GetOutputFolder(this.input.Filename);
            string fileNamePrefix = FileUtil.GetOutputFilePrefix(this.input.Filename);
            string fileNameNoPath = Path.GetFileName(this.input.Filename);

            string fileName = Path.GetFileNameWithoutExtension(this.input.Filename);
            if (Path.GetExtension(this.input.Filename).ToUpperInvariant().Equals(".IFO") 
                && FileUtil.RegExMatch(fileName, @"_\d{1,2}\z", false))
            {
                // DVD structure found &&
                // file ends with e.g. _1 as in VTS_01_1
                fileName = fileName.Substring(0, fileName.LastIndexOf('_') + 1) + iFile.VideoInfo.PGCNumber;
                if (iFile.VideoInfo.AngleNumber > 0)
                    fileName += "_" + iFile.VideoInfo.AngleNumber;

            }
            fileName = fileNamePrefix + fileName + Path.GetExtension(this.input.Filename);

            switch (IndexerUsed)
            {
                case IndexType.D2V: output.Text = Path.Combine(projectPath, Path.ChangeExtension(fileName, ".d2v")); break;
                case IndexType.DGM:
                case IndexType.DGI: output.Text = Path.Combine(projectPath, Path.ChangeExtension(fileName, ".dgi")); break;
                case IndexType.FFMS: output.Text = Path.Combine(projectPath, fileName + ".ffindex"); break;
                case IndexType.LSMASH: output.Text = Path.Combine(projectPath, fileName + ".lwi"); break;
            }
        }

        /// <summary>
        /// creates a project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueButton_Click(object sender, System.EventArgs e)
        {
            if (dialogMode)
                return;

            if (!configured)
            {
                MessageBox.Show("You must select the input and output file to continue",
                    "Configuration incomplete", MessageBoxButtons.OK);
                return;
            }

            if (!Drives.ableToWriteOnThisDrive(Path.GetPathRoot(output.Text)))
            {
                MessageBox.Show("MeGUI cannot write on the disc " + Path.GetPathRoot(output.Text) + "\n" +
                    "Please, select another output path to save your project...", "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            JobChain prepareJobs = null;
            string videoInput = input.Filename;

            // create pgcdemux job if needed
            if (Path.GetExtension(input.Filename).ToUpperInvariant().Equals(".IFO"))
            {
                if (iFile.VideoInfo.PGCCount > 0 || iFile.VideoInfo.AngleCount > 0)
                {
                    // pgcdemux must be used as either multiple PGCs or a multi-angle disc are found
                    string tempFile = Path.Combine(Path.GetDirectoryName(output.Text), Path.GetFileNameWithoutExtension(output.Text) + "_1.VOB");
                    prepareJobs = new SequentialChain(new PgcDemuxJob(videoInput, tempFile, iFile.VideoInfo.PGCNumber, iFile.VideoInfo.AngleNumber));
                    videoInput = tempFile;

                    string filesToOverwrite = string.Empty;
                    for (int i = 1; i < 10; i++)
                    {
                        string file = Path.Combine(Path.GetDirectoryName(output.Text), Path.GetFileNameWithoutExtension(output.Text) + "_" + i + ".VOB");
                        if (File.Exists(file))
                        {
                            filesToOverwrite += file + "\n";
                        }
                    }
                    if (!String.IsNullOrEmpty(filesToOverwrite))
                    {
                        DialogResult dr = MessageBox.Show("The pgc demux files already exist: \n" + filesToOverwrite + "\n" +
                                                            "Do you want to overwrite these files?", "Configuration Incomplete",
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr == DialogResult.No)
                            return;
                    }
                }
                else
                {
                    // change from _0.IFO to _1.VOB for the indexer
                    videoInput = input.Filename.Substring(0, input.Filename.Length - 5) + "1.VOB";
                    if (!File.Exists(videoInput))
                    {
                        MessageBox.Show("The file following file cannot be found: \n" + videoInput , "Configuration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            switch (IndexerUsed)
            {
                case IndexType.D2V:
                    {
                        prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(generateD2VIndexJob(videoInput)));
                        MainForm.Instance.Jobs.addJobsWithDependencies(prepareJobs, true);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                        break;
                    }
                case IndexType.DGI:
                    {
                        prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(generateDGNVIndexJob(videoInput)));
                        MainForm.Instance.Jobs.addJobsWithDependencies(prepareJobs, true);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                        break;
                    }
                case IndexType.DGM:
                    {
                        prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(generateDGMIndexJob(videoInput)));
                        MainForm.Instance.Jobs.addJobsWithDependencies(prepareJobs, true);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                        break;
                    }
                case IndexType.FFMS:
                    {
                        FFMSIndexJob job = generateFFMSIndexJob(videoInput);
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA") 
                            && job.DemuxMode > 0 && job.AudioTracks.Count > 0)
                        {
                            job.DemuxMode = 0;
                            job.AudioTracksDemux = job.AudioTracks;
                            job.AudioTracks = new List<AudioTrackInfo>();
                            MkvExtractJob extractJob = new MkvExtractJob(videoInput, Path.GetDirectoryName(this.output.Text), job.AudioTracksDemux);
                            prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(extractJob));
                        }
                        prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(job));
                        MainForm.Instance.Jobs.addJobsWithDependencies(prepareJobs, true);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                        break;
                    }
                case IndexType.LSMASH:
                    {
                        LSMASHIndexJob job = generateLSMASHIndexJob(videoInput);
                        if (txtContainerInformation.Text.Trim().ToUpperInvariant().Equals("MATROSKA")
                            && job.DemuxMode > 0 && job.AudioTracks.Count > 0)
                        {
                            job.DemuxMode = 0;
                            job.AudioTracksDemux = job.AudioTracks;
                            job.AudioTracks = new List<AudioTrackInfo>();
                            MkvExtractJob extractJob = new MkvExtractJob(videoInput, Path.GetDirectoryName(this.output.Text), job.AudioTracksDemux);
                            prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(extractJob));
                        }
                        prepareJobs = new SequentialChain(prepareJobs, new SequentialChain(job));
                        MainForm.Instance.Jobs.addJobsWithDependencies(prepareJobs, true);
                        if (this.closeOnQueue.Checked)
                            this.Close();
                        break;
                    }
            }              
        }
        #endregion
        #region helper methods
        private void checkIndexIO()
        {
            configured = (!input.Filename.Equals("") && !output.Text.Equals(""));
            if (configured && dialogMode)
                queueButton.DialogResult = DialogResult.OK;
            else
                queueButton.DialogResult = DialogResult.None;
        }

        public static bool isDGMFile(string input)
        {
            if (!File.Exists(input))
                return false;

            using (StreamReader sr = new StreamReader(input, System.Text.Encoding.Default))
            {
                string line = sr.ReadLine();
                if (line.ToLower().Contains("dgindexim"))
                    return true;
                else
                    return false;
            }
        }

        private D2VIndexJob generateD2VIndexJob(string videoInput)
        {
            int demuxType = 0;
            if (demuxTracks.Checked)
                demuxType = 1;
            else if (demuxNoAudiotracks.Checked)
                demuxType = 0;
            else
                demuxType = 2;

            List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
            foreach (AudioTrackInfo ati in AudioTracks.CheckedItems)
                audioTracks.Add(ati);

            return new D2VIndexJob(videoInput, this.output.Text, demuxType, audioTracks, loadOnComplete.Checked, demuxVideo.Checked);
        }

        private DGIIndexJob generateDGNVIndexJob(string videoInput)
        {
            int demuxType = 0;
            if (demuxTracks.Checked)
                demuxType = 1;
            else if (demuxNoAudiotracks.Checked)
                demuxType = 0;
            else
                demuxType = 2;

            List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
            foreach (AudioTrackInfo ati in AudioTracks.CheckedItems)
                audioTracks.Add(ati);

            return new DGIIndexJob(videoInput, this.output.Text, demuxType, audioTracks, loadOnComplete.Checked, demuxVideo.Checked);
        }

        private DGMIndexJob generateDGMIndexJob(string videoInput)
        {
            int demuxType = 0;
            if (demuxTracks.Checked)
                demuxType = 1;
            else if (demuxNoAudiotracks.Checked)
                demuxType = 0;
            else
                demuxType = 2;

            List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
            foreach (AudioTrackInfo ati in AudioTracks.CheckedItems)
                audioTracks.Add(ati);

            return new DGMIndexJob(videoInput, this.output.Text, demuxType, audioTracks, loadOnComplete.Checked, demuxVideo.Checked);
        }

        private FFMSIndexJob generateFFMSIndexJob(string videoInput)
        {
            int demuxType = 0;
            if (demuxTracks.Checked)
                demuxType = 1;
            else if (demuxNoAudiotracks.Checked)
                demuxType = 0;
            else
                demuxType = 2;

            List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
            foreach (AudioTrackInfo ati in AudioTracks.CheckedItems)
                audioTracks.Add(ati);

            return new FFMSIndexJob(videoInput, output.Text, demuxType, audioTracks, loadOnComplete.Checked);
        }

        private LSMASHIndexJob generateLSMASHIndexJob(string videoInput)
        {
            int demuxType = 0;
            if (demuxTracks.Checked)
                demuxType = 1;
            else if (demuxNoAudiotracks.Checked)
                demuxType = 0;
            else
                demuxType = 2;

            List<AudioTrackInfo> audioTracks = new List<AudioTrackInfo>();
            foreach (AudioTrackInfo ati in AudioTracks.CheckedItems)
                audioTracks.Add(ati);

            return new LSMASHIndexJob(videoInput, output.Text, demuxType, audioTracks, loadOnComplete.Checked);
        }
        #endregion

        private void rbtracks_CheckedChanged(object sender, EventArgs e)
        {
            // Now defaults to starting with every track selected
            for (int i = 0; i < AudioTracks.Items.Count; i++)
                AudioTracks.SetItemChecked(i, !demuxNoAudiotracks.Checked);
            AudioTracks.Enabled = demuxTracks.Checked;
        }

        private void btnFFMS_Click(object sender, EventArgs e)
        {
            changeIndexer(IndexType.FFMS);
        }

        private void btnDGI_Click(object sender, EventArgs e)
        {
            changeIndexer(IndexType.DGI);
        }

        private void btnDGM_Click(object sender, EventArgs e)
        {
            changeIndexer(IndexType.DGM);
        }

        private void btnD2V_Click(object sender, EventArgs e)
        {
            changeIndexer(IndexType.D2V);
        }

        private void btnLSMASH_Click(object sender, EventArgs e)
        {
            changeIndexer(IndexType.LSMASH);
        }
    }

    public class D2VCreatorTool : MeGUI.core.plugins.interfaces.ITool
    {

        #region ITool Members

        public string Name
        {
            get { return "File Indexer"; }
        }

        public void Run(MainForm info)
        {
            new FileIndexerWindow(info).Show();

        }

        public Shortcut[] Shortcuts
        {
            get { return new Shortcut[] { Shortcut.Ctrl2 }; }
        }

        #endregion

        #region IIDable Members

        public string ID
        {
            get { return "d2v_creator"; }
        }

        #endregion
    }

    public class d2vIndexJobPostProcessor
    {
        public static JobPostProcessor PostProcessor = new JobPostProcessor(postprocess, "D2V_postprocessor");
        private static LogItem postprocess(MainForm mainForm, Job ajob)
        {
            if (!(ajob is D2VIndexJob)) return null;
            D2VIndexJob job = (D2VIndexJob)ajob;

            StringBuilder logBuilder = new StringBuilder();
            List<string> arrFilesToDelete = new List<string>();
            Dictionary<int, string> audioFiles = VideoUtil.getAllDemuxedAudio(job.AudioTracks, new List<AudioTrackInfo>(), out arrFilesToDelete, job.Output, null);
            if (job.LoadSources)
            {
                if (job.DemuxMode != 0 && audioFiles.Count > 0)
                {
                    string[] files = new string[audioFiles.Values.Count];
                    audioFiles.Values.CopyTo(files, 0);
                    Util.ThreadSafeRun(mainForm, new MethodInvoker(
                        delegate
                        {
                            mainForm.Audio.openAudioFile(files);
                        }));
                }
                // if the above needed delegation for openAudioFile this needs it for openVideoFile?
                // It seems to fix the problem of ASW dissapearing as soon as it appears on a system (Vista X64)
                Util.ThreadSafeRun(mainForm, new MethodInvoker(
                    delegate
                    {
                        AviSynthWindow asw = new AviSynthWindow(mainForm, job.Output);
                        asw.OpenScript += new OpenScriptCallback(mainForm.Video.openVideoFile);
                        asw.Show();
                    }));
            }

            return null;
        }
    }

    public class dgiIndexJobPostProcessor
    {
        public static JobPostProcessor PostProcessor = new JobPostProcessor(postprocess, "Dgi_postprocessor");
        private static LogItem postprocess(MainForm mainForm, Job ajob)
        {
            if (!(ajob is DGIIndexJob)) return null;
            DGIIndexJob job = (DGIIndexJob)ajob;

            StringBuilder logBuilder = new StringBuilder();
            List<string> arrFilesToDelete = new List<string>();
            Dictionary<int, string> audioFiles = VideoUtil.getAllDemuxedAudio(job.AudioTracks, new List<AudioTrackInfo>(), out arrFilesToDelete, job.Output, null);
            if (job.LoadSources)
            {
                if (job.DemuxMode != 0 && audioFiles.Count > 0)
                {
                    string[] files = new string[audioFiles.Values.Count];
                    audioFiles.Values.CopyTo(files, 0);
                    Util.ThreadSafeRun(mainForm, new MethodInvoker(
                        delegate
                        {
                            mainForm.Audio.openAudioFile(files);
                        }));
                }
                // if the above needed delegation for openAudioFile this needs it for openVideoFile?
                // It seems to fix the problem of ASW dissapearing as soon as it appears on a system (Vista X64)
                Util.ThreadSafeRun(mainForm, new MethodInvoker(
                    delegate
                    {
                        AviSynthWindow asw = new AviSynthWindow(mainForm, job.Output);
                        asw.OpenScript += new OpenScriptCallback(mainForm.Video.openVideoFile);
                        asw.Show();
                    }));
            }

            return null;
        }
    }

    public class dgmIndexJobPostProcessor
    {
        public static JobPostProcessor PostProcessor = new JobPostProcessor(postprocess, "Dgm_postprocessor");
        private static LogItem postprocess(MainForm mainForm, Job ajob)
        {
            if (!(ajob is DGMIndexJob))
                return null;
            DGMIndexJob job = (DGMIndexJob)ajob;

            StringBuilder logBuilder = new StringBuilder();
            List<string> arrFilesToDelete = new List<string>();
            Dictionary<int, string> audioFiles = VideoUtil.getAllDemuxedAudio(job.AudioTracks, new List<AudioTrackInfo>(), out arrFilesToDelete, job.Output, null);
            if (job.LoadSources)
            {
                if (job.DemuxMode != 0 && audioFiles.Count > 0)
                {
                    string[] files = new string[audioFiles.Values.Count];
                    audioFiles.Values.CopyTo(files, 0);
                    Util.ThreadSafeRun(mainForm, new MethodInvoker(
                        delegate
                        {
                            mainForm.Audio.openAudioFile(files);
                        }));
                }
                // if the above needed delegation for openAudioFile this needs it for openVideoFile?
                // It seems to fix the problem of ASW dissapearing as soon as it appears on a system (Vista X64)
                Util.ThreadSafeRun(mainForm, new MethodInvoker(
                    delegate
                    {
                        AviSynthWindow asw = new AviSynthWindow(mainForm, job.Output);
                        asw.OpenScript += new OpenScriptCallback(mainForm.Video.openVideoFile);
                        asw.Show();
                    }));
            }

            return null;
        }
    }

    public class ffmsIndexJobPostProcessor
    {
        public static JobPostProcessor PostProcessor = new JobPostProcessor(postprocess, "FFMS_postprocessor");
        private static LogItem postprocess(MainForm mainForm, Job ajob)
        {
            if (!(ajob is FFMSIndexJob)) return null;
            FFMSIndexJob job = (FFMSIndexJob)ajob;

            StringBuilder logBuilder = new StringBuilder();
            List<string> arrFilesToDelete = new List<string>();
            Dictionary<int, string> audioFiles = VideoUtil.getAllDemuxedAudio(job.AudioTracks, job.AudioTracksDemux, out arrFilesToDelete, job.Output, null);
            if (job.LoadSources)
            {
                if (job.DemuxMode != 0)
                {
                    string[] files = new string[audioFiles.Values.Count];
                    audioFiles.Values.CopyTo(files, 0);
                    Util.ThreadSafeRun(mainForm, new MethodInvoker(
                        delegate
                        {
                            mainForm.Audio.openAudioFile(files);
                        }));
                }
                // if the above needed delegation for openAudioFile this needs it for openVideoFile?
                // It seems to fix the problem of ASW dissapearing as soon as it appears on a system (Vista X64)
                Util.ThreadSafeRun(mainForm, new MethodInvoker(
                    delegate
                    {
                        AviSynthWindow asw = new AviSynthWindow(mainForm, job.Input, job.Output);
                        asw.OpenScript += new OpenScriptCallback(mainForm.Video.openVideoFile);
                        asw.Show();
                    }));
            }

            return null;
        }
    }

    public class lsmashIndexJobPostProcessor
    {
        public static JobPostProcessor PostProcessor = new JobPostProcessor(postprocess, "LSMASH_postprocessor");
        private static LogItem postprocess(MainForm mainForm, Job ajob)
        {
            if (!(ajob is LSMASHIndexJob)) 
                return null;
            LSMASHIndexJob job = (LSMASHIndexJob)ajob;

            StringBuilder logBuilder = new StringBuilder();
            List<string> arrFilesToDelete = new List<string>();
            Dictionary<int, string> audioFiles = VideoUtil.getAllDemuxedAudio(job.AudioTracks, job.AudioTracksDemux, out arrFilesToDelete, job.Output, null);
            if (job.LoadSources)
            {
                if (job.DemuxMode != 0)
                {
                    string[] files = new string[audioFiles.Values.Count];
                    audioFiles.Values.CopyTo(files, 0);
                    Util.ThreadSafeRun(mainForm, new MethodInvoker(
                        delegate
                        {
                            mainForm.Audio.openAudioFile(files);
                        }));
                }
                // if the above needed delegation for openAudioFile this needs it for openVideoFile?
                // It seems to fix the problem of ASW dissapearing as soon as it appears on a system (Vista X64)
                Util.ThreadSafeRun(mainForm, new MethodInvoker(
                    delegate
                    {
                        AviSynthWindow asw = new AviSynthWindow(mainForm, job.Input, job.Output);
                        asw.OpenScript += new OpenScriptCallback(mainForm.Video.openVideoFile);
                        asw.Show();
                    }));
            }

            return null;
        }
    }

    public delegate void ProjectCreationComplete(); // this event is fired when the dgindex thread finishes
}
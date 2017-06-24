﻿// ****************************************************************************
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
using System.IO;
using System.Text;

using MeGUI.core.util;

namespace MeGUI
{
    public class LSMASHIndexer : CommandlineJobProcessor<LSMASHIndexJob>
    {
        public static readonly JobProcessorFactory Factory =
                    new JobProcessorFactory(new ProcessorFactory(init), "LSMASHIndexer");

        private static IJobProcessor init(MainForm mf, Job j)
        {
            if (j is LSMASHIndexJob) 
                return new LSMASHIndexer(mf.Settings.LSMASH.Path);
            return null;
        }

        private bool indexer;
        public LSMASHIndexer(string executableName)
        {
            UpdateCacher.CheckPackage("lsmash");
            executable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "ping.exe");
        }

        protected override string Commandline
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("127.0.0.1 -t");
                return sb.ToString();
            }
        }

        protected override void checkJobIO()
        {
            try
            {
                if (!String.IsNullOrEmpty(job.Output))
                {
                    FileUtil.ensureDirectoryExists(Path.GetDirectoryName(job.Output));
                    FileUtil.DeleteFile(job.Input + ".lwi", base.log);
                }
            }
            finally
            {
                base.checkJobIO();
            }
            su.Status = "Creating LSMASH index...";
        }

        public override void ProcessLine(string line, StreamType stream, ImageType oType)
        {
            // make sure that this is only called once
            if (indexer)
                return;
            indexer = true;

            // generate the avs script
            StringBuilder strAVSScript = new StringBuilder();
            MediaInfoFile oInfo = null;
            strAVSScript.Append(VideoUtil.getLSMASHVideoInputLine(job.Input, job.Input + ".lwi", 0, ref oInfo));
            oInfo.Dispose();
            base.log.LogValue("AviSynth script", strAVSScript.ToString(), ImageType.Information);

            // check if the script has a video track, also this call will create the index file if there is one
            string strErrorText;
            if (!VideoUtil.AVSScriptHasVideo(strAVSScript.ToString(), out strErrorText))
            {
                // avs script has no video track or an error has been thrown
                base.log.LogEvent(strErrorText, ImageType.Error);
                su.HasError = true;
            }

            if (proc == null || proc.HasExited)
                return;

            try
            {
                bWaitForExit = true;
                mre.Set(); // if it's paused, then unpause
                stdoutDone.Set();
                stderrDone.Set();
                proc.Kill();
                while (bWaitForExit) // wait until the process has terminated without locking the GUI
                {
                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                }
                proc.WaitForExit();
                return;
            }
            catch (Exception e)
            {
                throw new JobRunException(e);
            }
        }

        protected override void doExitConfig()
        {
            // no further action if job failed or was aborted
            if (su.HasError || su.WasAborted)
            {
                job.FilesToDelete.Add(job.Input + ".lwi");
                base.doExitConfig();
                return;
            }

            bool bFound = File.Exists(job.Input + ".lwi");
            if (bFound)
            {
                // LWLibavVideoSource() and therefore an index file is used
                string inputIndex = job.Input + ".lwi";
                string outputIndex = job.Output;

                if (!String.IsNullOrEmpty(outputIndex) && !outputIndex.ToLowerInvariant().Equals(inputIndex.ToLowerInvariant()))
                {
                    // index in a location different from the standard one should be used
                    su.Status = "Copying LSMASH index...";
                    try
                    {
                        // try to delete the destination file
                        if (FileUtil.DeleteFile(outputIndex, stderrLog))
                        {
                            // destination file is deleted / not available ==> copy source to destination
                            File.Copy(inputIndex, outputIndex, true);
                            base.log.LogEvent(inputIndex + " moved to " + outputIndex);
                            job.FilesToDelete.Add(inputIndex);
                        }
                        else
                            su.HasError = true;
                    }
                    catch (Exception e)
                    {
                        base.log.LogEvent(inputIndex + " not moved to " + outputIndex + ". error: " + e.Message + " " + e.StackTrace, ImageType.Error);
                        su.HasError = true;
                    }
                }
            }

            if (job.DemuxMode == 0 || job.AudioTracks.Count == 0 || su.HasError)
            {
                // no audio processing
                base.doExitConfig();
                return;
            }

            int iTracksFound = 0;
            int iCurrentAudioTrack = -1;
            for (int iCurrentTrack = 0; iCurrentTrack <= 29; iCurrentTrack++) // hard limit to max. 30 tracks
            {
                StringBuilder strAVSScript = new StringBuilder();
                strAVSScript.Append(VideoUtil.getLSMASHAudioInputLine(job.Input, job.Output, iCurrentTrack));

                // is this an audio track?
                string strErrorText;
                if (AudioUtil.AVSScriptHasAudio(strAVSScript.ToString(), out strErrorText) == false)
                    continue;
                iCurrentAudioTrack++;

                foreach (AudioTrackInfo oAudioTrack in job.AudioTracks)
                {
                    if (oAudioTrack.TrackIndex != iCurrentAudioTrack)
                        continue;

                    // write avs file
                    string strAudioAVSFile;
                    strAudioAVSFile = Path.GetFileNameWithoutExtension(job.Output) + "_track_" + (oAudioTrack.TrackIndex + 1) + "_" + oAudioTrack.Language.ToLowerInvariant() + ".avs";
                    strAudioAVSFile = Path.Combine(Path.GetDirectoryName(job.Output), Path.GetFileName(strAudioAVSFile));
                    try
                    {
                        strAVSScript.AppendLine(@"# detected channels: " + oAudioTrack.NbChannels);
                        strAVSScript.Append(@"# detected channel positions: " + oAudioTrack.ChannelPositions);
                        StreamWriter oAVSWriter = new StreamWriter(strAudioAVSFile, false, Encoding.Default);
                        oAVSWriter.Write(strAVSScript);
                        oAVSWriter.Close();
                    }
                    catch (Exception ex)
                    {
                        base.log.LogEvent("Error creating audio AVS file: " + ex.Message, ImageType.Error);
                    }
                    break;
                }
                if (++iTracksFound == job.AudioTracks.Count)
                    break;
            }

            if (!bFound && File.Exists(job.Input + ".lwi"))
                job.FilesToDelete.Add(job.Input + ".lwi");

            base.doExitConfig();
        }

        protected override bool checkExitCode
        {
            get { return false; }
        }
    }
}
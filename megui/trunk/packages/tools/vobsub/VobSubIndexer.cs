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
using System.IO;
using System.Text;

using MeGUI.core.util;

namespace MeGUI
{
    public class VobSubIndexer : CommandlineJobProcessor<SubtitleIndexJob>
    {
        public static readonly JobProcessorFactory Factory = new JobProcessorFactory(new ProcessorFactory(init), "VobSubIndexer");

        private string configFile = null;

        private static IJobProcessor init(MainForm mf, Job j)
        {
            if (j is SubtitleIndexJob)
                return new VobSubIndexer();

            return null;
        }

        protected override string Commandline
        {
            get
            {
                return "\"" + MainForm.Instance.Settings.VobSub.Path + "\",Configure " + configFile;
            }
        }

        public VobSubIndexer()
        {
            UpdateCacher.CheckPackage("vobsub");
            executable = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\rundll32.exe";
        }

        #region IJobProcessor Members
        protected override void checkJobIO()
        {
            base.checkJobIO();
            generateScript();
            Util.ensureExists(configFile);
            su.Status = "Demuxing subtitles...";
        }

        private void generateScript()
        {
            // create the configuration script
            StringBuilder script = new StringBuilder();
            script.AppendLine(job.Input);
            script.AppendLine(FileUtil.GetPathWithoutExtension(job.Output));
            script.AppendLine(job.PGC.ToString());
            script.AppendLine("0"); // we presume angle processing has been done before
            script.AppendLine("ALL"); //always process everything and strip down later
            script.AppendLine("CLOSE");

            // write the script to a temp file
            configFile = Path.ChangeExtension(job.Output, ".vobsub");
            FileUtil.ensureDirectoryExists(Path.GetDirectoryName(configFile));
            using (StreamWriter output = new StreamWriter(configFile, false, Encoding.Default))
                output.Write(script.ToString());

            log.LogValue("VobSub configuration file", script);

            job.FilesToDelete.Add(configFile);
        }

        public override bool canPause
        {
            get { return false; }
        }

        #endregion

        protected override bool checkExitCode
        {
            get { return false; }
        }

        protected override void doExitConfig()
        {
            if (job.SingleFileExport || !File.Exists(job.Output))
                return;

            // multiple output files have to be generated based on the single input file
            su.Status = "Generating files...";

            string line;
            bool bHeader = true; // same header for all output files
            bool bWait = false;
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbIndex = new StringBuilder();
            StringBuilder sbIndexTemp = new StringBuilder();
            int index = 0;

            using (System.IO.StreamReader file = new System.IO.StreamReader(job.Output))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (bHeader)
                    {
                        if (line.StartsWith("langidx:"))
                        {
                            // first subtitle track detected
                            bHeader = false;
                            bWait = true;
                        }
                        else
                            sbHeader.AppendLine(line);
                    }
                    else if (bWait)
                    {
                        sbIndexTemp.AppendLine(line);
                        if (line.StartsWith("id: "))
                        {
                            // new track detected
                            index = Int32.Parse(line.Substring(line.LastIndexOf(' ')));

                            // create full output text
                            sbIndex.Clear();
                            sbIndex.Append(sbHeader.ToString());
                            sbIndex.AppendLine("langidx: " + index);
                            sbIndex.Append(sbIndexTemp.ToString());
                            bWait = false;
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(line))
                        {
                            bWait = true;

                            // check if the track found in the input file is selected to be demuxed
                            bool bFound = false;
                            foreach (int id in job.TrackIDs)
                            {
                                if (index == id)
                                    bFound = true;
                            }

                            // export if found or if all tracks should be demuxed
                            if (bFound || job.IndexAllTracks)
                            {
                                // create output file
                                string outputFile = Path.Combine(Path.GetDirectoryName(job.Output), Path.GetFileNameWithoutExtension(job.Output)) + "_" + index + ".idx";
                                using (System.IO.StreamWriter output = new System.IO.StreamWriter(outputFile))
                                    output.WriteLine(sbIndex.ToString());

                                outputFile = Path.ChangeExtension(outputFile, ".sub");
                                File.Copy(Path.ChangeExtension(job.Output, ".sub"), outputFile, true);

                                log.LogEvent("Subtitle file created: " + Path.GetFileName(outputFile));
                            }

                            sbIndexTemp.Clear();
                            sbIndexTemp.AppendLine(line);
                        }
                        else
                            sbIndex.AppendLine(line);
                    }
                }
            }
            File.Delete(job.Output);
            File.Delete(Path.ChangeExtension(job.Output, ".sub"));
        }
    }
}
// ****************************************************************************
// 
// Copyright (C) 2005-2015 Doom9 & al
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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Update
{
    public class FileData
    {
        public string Source = string.Empty; // temp file
        public string Destination = string.Empty; // destination file
    }

    public class UpgradeData
    {
        public string Package = string.Empty;
        public List<FileData> Files = new List<FileData>();
        public Version Version = new Version();
    }

    public class Version
    {
        public Version() { }

        public Version(string version, string url)
        {
            this.fileVersion = version;
            this.url = url;
        }

        private string fileVersion = string.Empty;
        public string FileVersion
        {
            get { return fileVersion; }
            set { fileVersion = value; }
        }

        private string url = string.Empty;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private DateTime uploadDate = DateTime.MinValue;
        public DateTime UploadDate
        {
            get { return uploadDate; }
            set { uploadDate = value; }
        }

        private string web = string.Empty;
        public string Web
        {
            get { return web; }
            set { web = value; }
        }
    }

    class Program
    {
        static List<string> errorsEncountered = new List<string>();

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // detect if MeGUI should be restarted
                bool bRestart = false;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--restart")
                        bRestart = true;
                }

                // wait till MeGUI is closed
                int iMax = 0;
                bool bFound = false;
                do
                {
                    bFound = false;
                    foreach (Process oProc in Process.GetProcessesByName("MeGUI"))
                    {
                        if (Path.GetDirectoryName(Application.ExecutablePath).Equals(Path.GetDirectoryName(oProc.MainModule.FileName)))
                            bFound = true;
                    }
                    if (bFound)
                        System.Threading.Thread.Sleep(200);
                } while (bFound && iMax++ < 200);

                if (iMax > 200)
                {
                    errorsEncountered.Add("MeGUI is still running. Update aborted.");
                    exit(false);
                }

                // checks if the main application is available
                string file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "megui.exe");

#if !DEBUG
                if (!File.Exists(file))
                {
                    errorsEncountered.Add(file + " not found");
                    exit(false);
                }
#endif
                file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "update.arg");
                if (!File.Exists(file))
                {
                    errorsEncountered.Add(file + " not found");
                    exit(bRestart);
                }

                List<UpgradeData> oData = new List<UpgradeData>();
                using (Stream fs = new FileStream(file, FileMode.Open))
                {
                    XmlReader reader = new XmlTextReader(fs);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<UpgradeData>));
                    if (serializer.CanDeserialize(reader))
                        oData = serializer.Deserialize(reader) as List<UpgradeData>;
                }

                if (oData.Count == 0)
                {
                    errorsEncountered.Add("No valid update data provided");
                    exit(bRestart);
                }

                foreach (UpgradeData oPackage in oData)
                {
                    bool bOK = true;
                    foreach (FileData oFile in oPackage.Files)
                    {
                        try
                        {
                            if (!File.Exists(oFile.Source))
                            {
                                bOK = false;
                                errorsEncountered.Add(oPackage.Package + ": file not found - " + oFile.Source);
                                continue;
                            }

                            File.Delete(oFile.Destination);
                            File.Move(oFile.Source, oFile.Destination);
                        }
                        catch (Exception e)
                        {
                            bOK = false;
                            errorsEncountered.Add(oPackage.Package + ": " + e.Message);
                        }
                    }
                    oPackage.Files = new List<FileData>();
                    if (!bOK)
                        oPackage.Version = new Version();
                }

                // create update file
                using (Stream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "megui.arg"), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<UpgradeData>));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    XmlWriter writer = XmlTextWriter.Create(fs, settings);
                    serializer.Serialize(writer, oData);
                }

                deleteTempFiles();

                exit(bRestart);
            }
            catch (Exception ex)
            {
                errorsEncountered.Add(ex.Message);
                exit(true);
            }
        }

        /// <summary>
        /// shows error message if available and exists
        /// </summary>
        private static void exit(bool bRestart)
        {
            if (errorsEncountered.Count > 0)
            {
                string error = String.Empty;
                foreach (string msg in errorsEncountered)
                    error += msg + "\r\n";
                MessageBox.Show(error, "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // delete update file
            string file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "update.arg");
            if (File.Exists(file))
                File.Delete(file);

            try
            {
                using (Process proc = new Process())
                {
                    ProcessStartInfo pstart = new ProcessStartInfo();
                    pstart.FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "megui.exe");
                    if (!bRestart)
                        pstart.Arguments = "--dont-start";
                    pstart.UseShellExecute = false;
                    proc.StartInfo = pstart;
                    if (!proc.Start())
                        MessageBox.Show("Failed to start MeGUI", "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start MeGUI: " + ex.Message, "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // exit program
            System.Environment.Exit(0);
        }

        /// <summary>
        /// deletes all temporary update files in case something is left behind
        /// </summary>
        private static void deleteTempFiles()
        {
            string meguiFolder = Path.GetDirectoryName(Application.ExecutablePath);
            if (Directory.Exists(meguiFolder))
            {
                try
                {
                    Array.ForEach(Directory.GetFiles(meguiFolder, "*.tempcopy", SearchOption.AllDirectories), delegate (string path) { File.Delete(path); });
                }
                catch (Exception ex)
                {
                    errorsEncountered.Add(ex.Message);
                }
            }
        }
    }
}
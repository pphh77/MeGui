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
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace UpdateCopier
{
    public class CommandlineUpgradeData
    {
        public List<string> filename = new List<string>();
        public List<string> tempFilename = new List<string>();
        public string newVersion;
    }

    class Program
    {
        static void showCommandlineErrorMessage(string[] args)
        {
            StringBuilder cmdline = new StringBuilder();
            foreach (string arg in args)
                cmdline.AppendLine(arg);
            MessageBox.Show("Error in commandline update arguments: there aren't enough. No program files will be updated. Commandline:\r\n"
                + cmdline.ToString(), "MeGUI Update Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void Main(string[] args)
        {
            string appName = null;
            appName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "megui.exe");

#if !DEBUG
            if (!File.Exists(appName))
            {
                MessageBox.Show(appName + " not found. \nNo files will be updated.", "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif

            StringBuilder commandline = new StringBuilder();
            List<Exception> errorsEncountered = new List<Exception>();
            Dictionary<string, CommandlineUpgradeData> filesToCopy = new Dictionary<string,CommandlineUpgradeData>();
            List<string> filesToInstall = new List<string>();
            bool bRestart = false;
            string lastComponentName = null;
            for (int i = 0; i < args.Length; i += 1)
            {
                if (args[i] == "--restart")
                {
                    bRestart = true;
                    i++;
                }
                else if (args[i] == "--component")
                {
                    if (args.Length > i + 2)
                    {
                        CommandlineUpgradeData data = new CommandlineUpgradeData();
                        data.newVersion = args[i+2];
                        filesToCopy.Add(args[i+1], data);
                        lastComponentName = args[i+1];
                        i += 2;
                    }
                    else
                    {
                        showCommandlineErrorMessage(args);
                        return;
                    }
                }
                else
                {
                    if (args.Length > i + 1 && lastComponentName != null)
                    {
                        if (Path.GetExtension(args[i]).ToLower().Equals(".zip") ||
                            Path.GetExtension(args[i]).ToLower().Equals(".7z"))
                        {
                            if (filesToCopy.ContainsKey(lastComponentName))
                                filesToCopy.Remove(lastComponentName);
                            try
                            {
                                if (File.Exists(args[i]))
                                    File.Delete(args[i]);
                                if (File.Exists(args[i + 1]))
                                    File.Delete(args[i + 1]);
                            }
                            catch (Exception e)
                            {
                                errorsEncountered.Add(e);
                            }
                        }
                        else if (filesToCopy.ContainsKey(lastComponentName))
                        {
                            filesToCopy[lastComponentName].filename.Add(args[i]);
                            filesToCopy[lastComponentName].tempFilename.Add(args[i+1]);
                        }
                        i++;
                    }
                    else
                    {
                        showCommandlineErrorMessage(args);
                        return;
                    }
                }
            }

            Thread.Sleep(2000);
            foreach (string file in filesToCopy.Keys)
            {
                bool succeeded = true;
                for (int i = 0; i < filesToCopy[file].tempFilename.Count; i++)
                {
                    try
                    {
                        if (File.Exists(filesToCopy[file].tempFilename[i]))
                        {
                            File.Delete(filesToCopy[file].filename[i]);
                            File.Move(filesToCopy[file].tempFilename[i], filesToCopy[file].filename[i]);
                        }
                    }
                    catch (IOException)
                    {
                        succeeded = false;
                    }
                    catch (Exception e)
                    {
                        succeeded = false;
                        errorsEncountered.Add(e);
                    }
                }
                if (succeeded)
                    commandline.AppendFormat(@"--upgraded ""{0}"" ""{1}"" ", file, filesToCopy[file].newVersion);
                else
                    commandline.AppendFormat(@"--upgrade-failed ""{0}"" ", file);
            }
            if (!bRestart)
                commandline.Append("--dont-start");

            foreach (string file in filesToInstall)
                commandline.AppendFormat(@"--install ""{0}"" ", file);

            Process proc = new Process();
            ProcessStartInfo pstart = new ProcessStartInfo();
            pstart.FileName = appName;
            pstart.Arguments = commandline.ToString();
            pstart.UseShellExecute = false;
            proc.StartInfo = pstart;
            if (!proc.Start())
            {
                if (errorsEncountered.Count == 0)
                {
                    MessageBox.Show("Files updated but failed to restart MeGUI. You'll have to start it yourself.", "MeGUI Update Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string message = "The following errors were encountered when updating MeGUI:\r\n";
                    foreach (Exception e in errorsEncountered)
                        message += e.Message + "\r\n";
                    message += "Failed to restart MeGUI";
                    MessageBox.Show(message, "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (errorsEncountered.Count == 0)
                return;

            string message1 = "The following errors were encountered when updating MeGUI:\r\n";
            foreach (Exception e in errorsEncountered)
                message1 += e.Message + "\r\n";
            MessageBox.Show(message1, "MeGUI Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
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

// ****************************************************************************
// 
// Copyright (C) 2009  Jarrett Vance
// 
// code from http://jvance.com/pages/ChapterGrabber.xhtml
// 
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MeGUI
{
    [Serializable]
    public class ChapterInfo
    {
        public string Title { get; set; }
        public string SourceName { get; set; }
        public string SourcePath { get; set; }
        public string SourceType { get; set; }
        public double FramesPerSecond { get; set; }
        public int TitleNumber { get; set; }
        public int PGCNumber { get; set; }
        public int AngleNumber { get; set; }
        public List<Chapter> Chapters { get; set; }

        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        // XmlSerializer does not support TimeSpan, so use this property for serialization instead
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long DurationTicks
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }

        public ChapterInfo()
        {
            Chapters = new List<Chapter>();
            FramesPerSecond = 0;
        }

        public override string ToString()
        {
            string strResult = string.Empty;

            if (Chapters.Count != 1)
                strResult = string.Format("{0}  -  {1}  -  {2}  -  [{3} Chapters]", Title, SourceName, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", System.Math.Floor(Duration.TotalHours), Duration.Minutes, Duration.Seconds, Duration.Milliseconds), Chapters.Count);
            else
                strResult = string.Format("{0}  -  {1}  -  {2}  -  [{3} Chapter]", Title, SourceName, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", System.Math.Floor(Duration.TotalHours), Duration.Minutes, Duration.Seconds, Duration.Milliseconds), Chapters.Count);
            if (AngleNumber > 0)
                strResult += "  -  Angle " + AngleNumber;
            return strResult;
        }

        [XmlIgnore]
        public bool HasChapters
        {
            get { return (Chapters.Count > 0); }
        }

        public bool LoadFile(string strFileName)
        {
            if (LoadText(strFileName))
                return true;

            if (LoadXML(strFileName))
                return true;

            // now try mediainfo
            MediaInfoFile oInfo = new MediaInfoFile(strFileName);
            if (!oInfo.HasChapters)
                return false;

            Chapters = oInfo.ChapterInfo.Chapters;
            SourceName = oInfo.ChapterInfo.SourceName;
            FramesPerSecond = oInfo.ChapterInfo.FramesPerSecond;
            Title = oInfo.ChapterInfo.Title;
            Duration = oInfo.ChapterInfo.Duration;
            return true;
        }

        public void ChangeFps(double fps)
        {
            if (FramesPerSecond == 0 || FramesPerSecond == fps)
            {
                FramesPerSecond = fps;
                return;
            }

            for (int i = 0; i < Chapters.Count; i++)
            {
                Chapter c = Chapters[i];
                double frames = c.Time.TotalSeconds * FramesPerSecond;
                Chapters[i] = new Chapter() { Name = c.Name, Time = new TimeSpan((long)Math.Round(frames / fps * TimeSpan.TicksPerSecond)) };
            }

            double totalFrames = Duration.TotalSeconds * FramesPerSecond;
            Duration = new TimeSpan((long)Math.Round((totalFrames / fps) * TimeSpan.TicksPerSecond));
            FramesPerSecond = fps;
        }

        public bool SaveText(string strFileName)
        {
            try
            {
                List<string> lines = new List<string>();
                int i = 0;
                foreach (Chapter c in Chapters)
                {
                    i++;
                    if (c.Time.ToString().Length == 8)
                        lines.Add("CHAPTER" + i.ToString("00") + "=" + c.Time.ToString() + ".000"); // better formating
                    else if (c.Time.ToString().Length > 12)
                        lines.Add("CHAPTER" + i.ToString("00") + "=" + c.Time.ToString().Substring(0, 12)); // remove some duration length too long
                    else
                        lines.Add("CHAPTER" + i.ToString("00") + "=" + c.Time.ToString());
                    lines.Add("CHAPTER" + i.ToString("00") + "NAME=" + c.Name);
                }
                File.WriteAllLines(strFileName, lines.ToArray());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LoadText(string strFileName)
        {
            try
            {
                FileInfo oFileInfo = new FileInfo(strFileName);
                if (oFileInfo.Length > 1048576)
                    return false;

                int num = 0;
                TimeSpan ts = new TimeSpan(0);
                string time = String.Empty;
                string name = String.Empty;
                bool onTime = true;
                string[] lines = File.ReadAllLines(strFileName);
                foreach (string line in lines)
                {
                    if (onTime)
                    {
                        num++;
                        //read time
                        time = line.Replace("CHAPTER" + num.ToString("00") + "=", "");
                        ts = TimeSpan.Parse(time);
                    }
                    else
                    {
                        //read name
                        name = line.Replace("CHAPTER" + num.ToString("00") + "NAME=", "");
                        //add it to list
                        Chapters.Add(new Chapter() { Name = name, Time = ts });
                    }
                    onTime = !onTime;
                }

                SourceName = strFileName;
                Title = Path.GetFileNameWithoutExtension(strFileName);
                if (Chapters.Count > 0)
                    Duration = Chapters[Chapters.Count - 1].Time;
            }
            catch (Exception)
            {
                Chapters.Clear();
                return false;
            }

            return true;
        }

        public bool SaveQpfile(string strFileName)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Chapter c in Chapters)
                    lines.Add(string.Format("{0} K", (long)Math.Round(c.Time.TotalSeconds * FramesPerSecond)));
                File.WriteAllLines(strFileName, lines.ToArray());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveTsmuxerMeta(string filename)
        {
            string text = "--custom-" + Environment.NewLine + "chapters=";
            foreach (Chapter c in Chapters)
                text += c.Time.ToString() + ";";
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveXml(string filename)
        {
            Random rndb = new Random();
            XmlTextWriter xmlchap = new XmlTextWriter(filename, Encoding.UTF8);
            xmlchap.Formatting = Formatting.Indented;
            xmlchap.WriteStartDocument();
            xmlchap.WriteComment("<!DOCTYPE Tags SYSTEM " + "\"" + "matroskatags.dtd" + "\"" + ">");
            xmlchap.WriteStartElement("Chapters");
            xmlchap.WriteStartElement("EditionEntry");
            xmlchap.WriteElementString("EditionFlagHidden", "0");
            xmlchap.WriteElementString("EditionFlagDefault", "0");
            xmlchap.WriteElementString("EditionUID", Convert.ToString(rndb.Next(1, Int32.MaxValue)));
            foreach (Chapter c in Chapters)
            {
                xmlchap.WriteStartElement("ChapterAtom");
                xmlchap.WriteStartElement("ChapterDisplay");
                xmlchap.WriteElementString("ChapterString", c.Name);
                xmlchap.WriteElementString("ChapterLanguage", "und");
                xmlchap.WriteEndElement();
                xmlchap.WriteElementString("ChapterUID", Convert.ToString(rndb.Next(1, Int32.MaxValue)));
                if (c.Time.ToString().Length == 8)
                    xmlchap.WriteElementString("ChapterTimeStart", c.Time.ToString() + ".0000000");
                else
                    xmlchap.WriteElementString("ChapterTimeStart", c.Time.ToString());
                xmlchap.WriteElementString("ChapterFlagHidden", "0");
                xmlchap.WriteElementString("ChapterFlagEnabled", "1");
                xmlchap.WriteEndElement();
            }
            xmlchap.WriteEndElement();
            xmlchap.WriteEndElement();
            xmlchap.Flush();
            xmlchap.Close();
        }

        private bool LoadXML(string strFileName)
        {
            try
            {
                FileInfo oFileInfo = new FileInfo(strFileName);
                if (oFileInfo.Length > 1048576)
                    return false;

                XmlDocument oChap = new XmlDocument();
                oChap.Load(strFileName);

                foreach (XmlNode oFirstNode in oChap.ChildNodes)
                {
                    if (!oFirstNode.Name.ToLowerInvariant().Equals("chapters"))
                        continue;

                    foreach (XmlNode oSecondNode in oFirstNode.ChildNodes)
                    {
                        if (!oSecondNode.Name.ToLowerInvariant().Equals("editionentry"))
                            continue;

                        foreach (XmlNode oThirdNode in oSecondNode.ChildNodes)
                        {
                            if (!oThirdNode.Name.ToLowerInvariant().Equals("chapteratom"))
                                continue;

                            Chapter oChapter = new Chapter();
                            foreach (XmlNode oChapterNode in oThirdNode.ChildNodes)
                            {
                                if (oChapterNode.Name.ToLowerInvariant().Equals("chaptertimestart"))
                                {
                                    oChapter.SetTimeBasedOnString(oChapterNode.InnerText);
                                }
                                else if (oChapterNode.Name.ToLowerInvariant().Equals("chapterdisplay"))
                                {
                                    foreach (XmlNode oChapterString in oChapterNode.ChildNodes)
                                    {
                                        if (oChapterString.Name.ToLowerInvariant().Equals("chapterstring"))
                                        {
                                            oChapter.Name = oChapterString.InnerText;
                                        }
                                    }
                                    break; // avoid multiple chapter entries
                                }
                            }
                            Chapters.Add(oChapter);
                        }
                        break; // avoid multiple editions
                    }
                }

                SourceName = strFileName;
                Title = Path.GetFileNameWithoutExtension(strFileName);
                if (Chapters.Count > 0)
                    Duration = Chapters[Chapters.Count - 1].Time;
            }
            catch (Exception)
            {
                Chapters.Clear();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create Chapters XML File from OGG Chapters File
        /// </summary>
        /// <param name="inFile">input</inFile>
        public void SaveAppleXML(string inFile)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                sb.AppendLine("<!-- GPAC 3GPP Text Stream -->");
                sb.AppendLine("<TextStream version=\"1.1\">");
                sb.AppendLine("<TextStreamHeader>");
                sb.AppendLine("<TextSampleDescription>");
                sb.AppendLine("</TextSampleDescription>");
                sb.AppendLine("</TextStreamHeader>");
                string lastTime = null;

                using (StreamReader sr = new StreamReader(inFile))
                {
                    string line = null;
                    string chapTitle = null;
                    int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        i++;
                        if (i % 2 == 1)
                        {
                            lastTime = line.Substring(line.IndexOf("=") + 1);
                            sb.Append("<TextSample sampleTime=\"" + lastTime + "\"");
                        }
                        else
                        {
                            chapTitle = System.Text.RegularExpressions.Regex.Replace(line.Substring(line.IndexOf("=") + 1), "\"", "&quot;");
                            sb.Append(" text=\"" + chapTitle + "\" />" + Environment.NewLine);
                        }
                    }
                }
                sb.AppendLine("<TextSample sampleTime=\"" + (TimeSpan.Parse(lastTime) + new TimeSpan(0, 0, 0, 0, 500)).ToString(@"hh\:mm\:ss\.fff") + "\" xml:space=\"preserve\" />");
                sb.AppendLine("</TextStream>");

                using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(inFile), Path.GetFileNameWithoutExtension(inFile) + ".xml")))
                {
                    sw.Write(sb.ToString());
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

        /// <summary>
        /// gets Timeline for tsMuxeR
        /// </summary>
        /// <returns>chapters Timeline as string</returns>
        public string GetChapterTimeLine()
        {
            string strTimeLine = string.Empty;

            foreach (Chapter oChapter in Chapters)
                strTimeLine += oChapter.Time.ToString().Substring(0,12) + ";";

            if (strTimeLine.EndsWith(";"))
                strTimeLine = strTimeLine.Substring(0, strTimeLine.Length - 1);

            return strTimeLine;
        }
    }
}
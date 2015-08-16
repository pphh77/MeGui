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
using MeGUI.core.util;

namespace eac3to
{
    /// <summary>A Stream</summary>
    public abstract class Stream
    {
        virtual public int Number { get; set; }
        virtual public string Name { get; set; }
        virtual public StreamType Type { get; set; }
        virtual public string Description { get; set; }
        virtual public string Language { get; set; }
        abstract public object[] ExtractTypes { get; }

        protected Stream() { }

        protected Stream(StreamType type, string s, LogItem _log)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s", "The string 's' cannot be null or empty.");

            this.Type = type;

            Description = s.Substring(s.IndexOf(":") + 1);
            Number = int.Parse(s.Substring(0, s.IndexOf(":")));

            if (s.Contains("Joined EVO"))
                Name = "Joined EVO";
            else if (s.Contains("Joined VOB"))
                Name = "Joined VOB";
            else if (type == StreamType.Subtitle && s.IndexOf("\"") > 0)
                Name = s.Substring(s.IndexOf("\"") + 1, s.LastIndexOf("\"") - s.IndexOf("\"") - 1);
            else
                Name = "";

            if (type == StreamType.Audio || type == StreamType.Subtitle || type == StreamType.Video)
                setLanguage(s, _log);
        }

        private void setLanguage(string s, LogItem _log)
        {
            char[] separator = { ',' };
            string[] split = s.Split(separator);
            string language = string.Empty;
            if (split.Length > 1)
            {
                language = split[1].Substring(1, split[1].Length - 1).Trim();
                switch (language)
                {
                    case "Modern Greek":    language = "Greek"; break;
                    case "LowGerman":       language = "Low German"; break;
                    case "North Ndebele":   language = "Ndebele, North"; break;
                    case "South Ndebele":   language = "Ndebele, South"; break;
                    case "Bokmål":          language = "Norwegian Bokmål"; break;
                    case "Walamo":          language = "Wolaitta"; break;
                    case "Undetermined":    language = string.Empty; break;
                }
                if (!String.IsNullOrEmpty(language) && (Char.IsNumber(language[0]) || language[0].Equals('"')))
                    language = string.Empty;
            }

            if (!MeGUI.LanguageSelectionContainer.IsLanguageAvailable(language))
            {
                if (this.Type != StreamType.Video)
                {
                    if (string.IsNullOrEmpty(language))
                        _log.LogEvent("The language information is not available for this track. The default MeGUI language has been selected.", ImageType.Information);
                    else
                        _log.LogEvent("The language information \"" + language + "\" is unknown. The default MeGUI language has been selected instead.", ImageType.Warning);
                    this.Language = MeGUI.MainForm.Instance.Settings.DefaultLanguage1;
                }
                else
                    this.Language = "";
            }
            else
                this.Language = language; 
        }

        public static Stream Parse(string s, LogItem _log)
        {
            //EVO, 1 video track, 1 audio track, 3 subtitle tracks, 1:43:54
            //"director"

            /////////////////////////////////////////////////////////////////
            //////// input file
            /*
            M2TS, 1 video track, 1 audio track, 0:00:11, 60i /1.001
            1: h264/AVC, 1080i60 /1.001 (16:9)
            2: AC3, 5.1 channels, 640kbps, 48khz             
              */

            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s", "The string 's' cannot be null or empty.");

            Stream stream = null;

            if (s.Contains("AVC") || s.Contains("MVC") || s.Contains("VC-1") || s.Contains("MPEG") || s.Contains("DIRAC") || s.Contains("THEORA"))
                stream = VideoStream.Parse(s, _log);
            else if (s.Contains("AC3") || s.Contains("TrueHD") || s.Contains("DTS") || 
                     s.Contains("RAW") || s.Contains("PCM") || s.Contains("MP") || s.Contains("AAC") ||
                     s.Contains("FLAC") || s.Contains("WAVPACK") || s.Contains("TTA") || s.Contains("VORBIS"))
                stream = AudioStream.Parse(s, _log);
            else if (s.Contains("Subtitle"))
                stream = SubtitleStream.Parse(s, _log);
            else if (s.Contains("Chapters"))
                stream = ChapterStream.Parse(s, _log);
            else if (s.Contains("Joined"))
                stream = JoinStream.Parse(s, _log);

            return stream;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Number, Description);
        }
    }
}
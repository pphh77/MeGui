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

namespace MediaInfoWrapper
{
    ///<summary>Contains properties for a TextTrack </summary>
    public class TextTrack
    {
        private string _Count;
        private string _StreamCount;
        private string _StreamKind;
        private string _StreamKindID;
        private string _StreamOrder;
        private string _Inform;
        private string _ID;
        private string _UniqueID;
        private string _Title;
        private string _CodecID;
        private string _CodecIDString;
        private string _CodecIDInfo;
        private string _Delay;
        private string _Video0Delay;
        private string _PlayTime;
        private string _PlayTimeString;
        private string _PlayTimeString1;
        private string _PlayTimeString2;
        private string _PlayTimeString3;
        private string _Language;
        private string _LanguageString;
        private string _LanguageMore;
        private string _Default;
        private string _DefaultString;
        private string _Forced;
        private string _ForcedString;

        ///<summary> Count of objects available in this stream </summary>
        public string Count
        {
            get
            {
                if (String.IsNullOrEmpty(this._Count))
                    this._Count="";
                return _Count;
            }
            set
            {
                this._Count=value;
            }
        }

        ///<summary> Count of streams of that kind available </summary>
        public string StreamCount
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamCount))
                    this._StreamCount="";
                return _StreamCount;
            }
            set
            {
                this._StreamCount=value;
            }
        }

        ///<summary> Stream name </summary>
        public string StreamKind
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamKind))
                    this._StreamKind="";
                return _StreamKind;
            }
            set
            {
                this._StreamKind=value;
            }
        }

        ///<summary> When multiple streams, number of the stream </summary>
        public string StreamKindID
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamKindID))
                    this._StreamKindID="";
                return _StreamKindID;
            }
            set
            {
                this._StreamKindID=value;
            }
        }

        ///<summary>Stream order in the file, whatever is the kind of stream (base=0)</summary>
        public string StreamOrder
        {
            get
            {
                if (String.IsNullOrEmpty(this._StreamOrder))
                    this._StreamOrder = "";
                return _StreamOrder;
            }
            set
            {
                this._StreamOrder = value;
            }
        }

        ///<summary> Last   Inform   call </summary>
        public string Inform
        {
            get
            {
                if (String.IsNullOrEmpty(this._Inform))
                    this._Inform="";
                return _Inform;
            }
            set
            {
                this._Inform=value;
            }
        }

        ///<summary> A ID for this stream in this file </summary>
        public string ID
        {
            get
            {
                if (String.IsNullOrEmpty(this._ID))
                    this._ID="";
                return _ID;
            }
            set
            {
                this._ID=value;
            }
        }

        ///<summary> A unique ID for this stream, should be copied with stream copy </summary>
        public string UniqueID
        {
            get
            {
                if (String.IsNullOrEmpty(this._UniqueID))
                    this._UniqueID="";
                return _UniqueID;
            }
            set
            {
                this._UniqueID=value;
            }
        }

        ///<summary> Name of the track </summary>
        public string Title
        {
            get
            {
                if (String.IsNullOrEmpty(this._Title))
                    this._Title="";
                return _Title;
            }
            set
            {
                this._Title=value;
            }
        }

        ///<summary> Codec used </summary>
        public string CodecID
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecID))
                    this._CodecID="";
                return _CodecID;
            }
            set
            {
                this._CodecID=value;
            }
        }

        ///<summary> Codec used (test) </summary>
        public string CodecIDString
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDString))
                    this._CodecIDString = "";
                return _CodecIDString;
            }
            set
            {
                this._CodecIDString = value;
            }
        }

        ///<summary> Codec used (info) </summary>
        public string CodecIDInfo
        {
            get
            {
                if (String.IsNullOrEmpty(this._CodecIDInfo))
                    this._CodecIDInfo = "";
                return _CodecIDInfo;
            }
            set
            {
                this._CodecIDInfo = value;
            }
        }

        ///<summary> Delay fixed in the stream (relative) </summary>
        public string Delay
        {
            get
            {
                if (String.IsNullOrEmpty(this._Delay))
                    this._Delay="";
                return _Delay;
            }
            set
            {
                this._Delay=value;
            }
        }

        ///<summary> Delay fixed in the stream (absolute _ video0) </summary>
        public string Video0Delay
        {
            get
            {
                if (String.IsNullOrEmpty(this._Video0Delay))
                    this._Video0Delay="";
                return _Video0Delay;
            }
            set
            {
                this._Video0Delay=value;
            }
        }

        ///<summary> Play time of the stream </summary>
        public string PlayTime
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTime))
                    this._PlayTime="";
                return _PlayTime;
            }
            set
            {
                this._PlayTime=value;
            }
        }

        ///<summary> Play time (formated) </summary>
        public string PlayTimeString
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTimeString))
                this._PlayTimeString="";
                return _PlayTimeString;
            }
            set
            {
                this._PlayTimeString=value;
            }
        }

        ///<summary> Play time in format : HHh MMmn SSs MMMms, XX omited if zero </summary>
        public string PlayTimeString1
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTimeString1))
                    this._PlayTimeString1="";
                return _PlayTimeString1;
            }
            set
            {
                this._PlayTimeString1=value;
            }
        }

        ///<summary> Play time in format : XXx YYy only, YYy omited if zero </summary>
        public string PlayTimeString2
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTimeString2))
                    this._PlayTimeString2="";
                return _PlayTimeString2;
            }
            set
            {
                this._PlayTimeString2=value;
            }
        }

        ///<summary> Play time in format : HH:MM:SS.MMM </summary>
        public string PlayTimeString3
        {
            get
            {
                if (String.IsNullOrEmpty(this._PlayTimeString3))
                    this._PlayTimeString3="";
                return _PlayTimeString3;
            }
            set
            {
                this._PlayTimeString3=value;
            }
        }

        ///<summary> Language (2 letters) </summary>
        public string Language
        {
            get
            {
                if (String.IsNullOrEmpty(this._Language))
                this._Language="";
                return _Language;
            }
            set
            {
                this._Language=value;
            }
        }

        ///<summary> Language (full) </summary>
        public string LanguageString
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageString))
                    this._LanguageString="";
                return _LanguageString;
            }
            set
            {
                this._LanguageString=value;
            }
        }

        ///<summary> More info about Language (director s comment...) </summary>
        public string LanguageMore
        {
            get
            {
                if (String.IsNullOrEmpty(this._LanguageMore))
                    this._LanguageMore="";
                return _LanguageMore;
            }
            set
            {
                this._LanguageMore=value;
            }
        }

        ///<summary> Default Info </summary>
        public string Default
        {
            get
            {
                if (String.IsNullOrEmpty(this._Default))
                    this._Default = "";
                return _Default;
            }
            set
            {
                this._Default = value;
            }
        }

        ///<summary> Default Info (string format)</summary>
        public string DefaultString
        {
            get
            {
                if (String.IsNullOrEmpty(this._DefaultString))
                    this._DefaultString = "";
                return _DefaultString;
            }
            set
            {
                this._DefaultString = value;
            }
        }

        ///<summary> Forced Info </summary>
        public string Forced
        {
            get
            {
                if (String.IsNullOrEmpty(this._Forced))
                    this._Forced = "";
                return _Forced;
            }
            set
            {
                this._Forced = value;
            }
        }

        ///<summary> Forced Info (string format)</summary>
        public string ForcedString
        {
            get
            {
                if (String.IsNullOrEmpty(this._ForcedString))
                    this._ForcedString = "";
                return _ForcedString;
            }
            set
            {
                this._ForcedString = value;
            }
        }
    }
}
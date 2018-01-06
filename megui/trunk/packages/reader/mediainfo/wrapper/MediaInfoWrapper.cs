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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using MeGUI;

namespace MediaInfoWrapper
{
    //disables the xml comment warning on compilation
#pragma warning disable 1591
    public enum StreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Other,
        Image,
        Menu,
    }

    public enum InfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public enum InfoOptions
    {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }

    public enum InfoFileOptions
    {
        FileOption_Nothing = 0x00,
        FileOption_NoRecursive = 0x01,
        FileOption_CloseAll = 0x02,
        FileOption_Max = 0x04
    };

    public enum Status
    {
        None = 0x00,
        Accepted = 0x01,
        Filled = 0x02,
        Updated = 0x04,
        Finalized = 0x08,
    }
#pragma warning restore 1591

    /// <summary>
    /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
    /// every information MediaInfo.dll can collect.
    /// Tracks are accessibles as properties.
    /// </summary>
    public class MediaInfo : IDisposable
    {

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, Int64 File_Size, Int64 File_Offset);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, Int64 File_Size, Int64 File_Offset);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, Int64 File_Size, byte[] Buffer, IntPtr Buffer_Size);
        [DllImport("MediaInfo.dll")]
        private static extern Int64 MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern Int64 MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);

        private IntPtr Handle;
        private bool MustUseAnsi;
        private bool bSuccess;

        #region Handle DLL
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool LoadLibraryA(string hModule);

        private static object _locker = new object();
        private int _random;
        private static object _lockerDLL = new object();
        private static int _countDLL = 0;
        #endregion

        public bool OpenSuccess { get => bSuccess; }

        //MediaInfo class
        public MediaInfo()
        {
            Random rnd = new Random();
            _random = rnd.Next(1, 1000000);
            bSuccess = false;

            try
            {
                Handle = MediaInfo_New();
            }
            catch
            {
                Handle = (IntPtr)0;
            }
            if (Environment.OSVersion.ToString().IndexOf("Windows") == -1)
                MustUseAnsi = true;
            else
                MustUseAnsi = false;
        }

        /// <summary>
        /// When called with a proper file target, returns a MediaInfo object filled with list of media tracks containing
        /// information MediaInfo.dll can collect. Tracks are accessible as properties.
        /// </summary>
        /// <param name="path"></param>
        public MediaInfo(string path) : this ()
        {
            if (Handle == (IntPtr)0)
                return;

            _FileName = path;
            if (MainForm.Instance.Settings.ShowDebugInformation)
                HandleMediaInfoWrapperDLL(false);

            if (!File.Exists(path))
            {
                Close();
                return;
            }
            
            Open(path);
            getStreamCount();
            getAllInfos();
            Close();
            bSuccess = true;
        }

        private void HandleMediaInfoWrapperDLL(bool bUnload)
        {
            lock (_lockerDLL)
            {
                if (MainForm.Instance.MediaInfoWrapperLog == null)
                    MainForm.Instance.MediaInfoWrapperLog = MainForm.Instance.Log.Info("MediaInfoWrapper");

                bool bDebug = false;
#if DEBUG
                bDebug = true;
#endif

                if (bUnload)
                {
                    _countDLL--;
                    if (_countDLL > 0)
                    {
                        MainForm.Instance.MediaInfoWrapperLog.LogValue("sessions open: " + _countDLL + ", id: " + _random, "File: " + _FileName + (bDebug ? Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.StackTrace : String.Empty));
                        return;
                    }

                    bool bResult = false;
                    string strFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "mediainfo.dll");
                    foreach (System.Diagnostics.ProcessModule mod in System.Diagnostics.Process.GetCurrentProcess().Modules)
                    {
                        if (mod.FileName.ToLowerInvariant().Equals(strFile.ToLowerInvariant()))
                            bResult = FreeLibrary(mod.BaseAddress);
                    }
                    MainForm.Instance.MediaInfoWrapperLog.LogValue("sessions open: " + _countDLL + ", id: " + _random + ", close: " + bResult, "File: " + _FileName + (bDebug ? Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.StackTrace : String.Empty));
                }
                else
                {
                    if (_countDLL == 0)
                        LoadLibraryA("mediainfo.dll");
                    _countDLL++;
                    MainForm.Instance.MediaInfoWrapperLog.LogValue("sessions open: " + _countDLL + ", id: " + _random, "File: " + "File: " + _FileName + (bDebug ? Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.StackTrace : String.Empty));
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                if (Handle == (IntPtr)0)
                    return;
                MediaInfo_Delete(Handle);
                disposedValue = true;

                if (MainForm.Instance.Settings.ShowDebugInformation)
                    HandleMediaInfoWrapperDLL(true);
            }
        }

         ~MediaInfo()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public int Open(String FileName)
        {
            if (Handle == (IntPtr)0)
                return 0;
            if (MustUseAnsi)
            {
                IntPtr FileName_Ptr = Marshal.StringToHGlobalAnsi(FileName);
                int ToReturn = (int)MediaInfoA_Open(Handle, FileName_Ptr);
                Marshal.FreeHGlobal(FileName_Ptr);
                return ToReturn;
            }
            else
                return (int)MediaInfo_Open(Handle, FileName);
        }

        public int Open_Buffer_Init(Int64 File_Size, Int64 File_Offset)
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (int)MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
        }

        public int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (int)MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
        }

        public Int64 Open_Buffer_Continue_GoTo_Get()
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (Int64)MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
        }

        public int Open_Buffer_Finalize()
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (int)MediaInfo_Open_Buffer_Finalize(Handle);
        }

        public void Close()
        {
            if (Handle == (IntPtr)0)
                return;
            MediaInfo_Close(Handle);
        }

        public String Inform()
        {
            if (Handle == (IntPtr)0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
                return Marshal.PtrToStringAnsi(MediaInfoA_Inform(Handle, (IntPtr)0));
            else
                return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, (IntPtr)0));
        }

        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
        {
            if (Handle == (IntPtr)0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
            {
                IntPtr Parameter_Ptr = Marshal.StringToHGlobalAnsi(Parameter);
                String ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter_Ptr, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
                Marshal.FreeHGlobal(Parameter_Ptr);
                return ToReturn;
            }
            else
                return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
        }

        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {
            if (Handle == (IntPtr)0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
                return Marshal.PtrToStringAnsi(MediaInfoA_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
            else
                return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
        }

        public String Option(String Option, String Value)
        {
            if (Handle == (IntPtr)0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
            {
                IntPtr Option_Ptr = Marshal.StringToHGlobalAnsi(Option);
                IntPtr Value_Ptr = Marshal.StringToHGlobalAnsi(Value);
                String ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Option(Handle, Option_Ptr, Value_Ptr));
                Marshal.FreeHGlobal(Option_Ptr);
                Marshal.FreeHGlobal(Value_Ptr);
                return ToReturn;
            }
            else
                return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value));
        }

        public int State_Get()
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (int)MediaInfo_State_Get(Handle);
        }

        public int Count_Get(StreamKind StreamKind, int StreamNumber)
        {
            if (Handle == (IntPtr)0)
                return 0;
            return (int)MediaInfo_Count_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber);
        }

        public int Count_Get(StreamKind StreamKind)
        {
            return Count_Get(StreamKind, -1);
        }

        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo)
        {
            return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
        }

        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
        }

        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);
        }

        public String Option(String Option_)
        {
            return Option(Option_, "");
        }


        private List<VideoTrack> _Video;
        private List<GeneralTrack> _General;
        private List<AudioTrack> _Audio;
        private List<TextTrack> _Text;
        private List<ChaptersTrack> _Chapters;
        private Int32 _VideoCount;
        private Int32 _GeneralCount;
        private Int32 _AudioCount;
        private Int32 _TextCount;
        private Int32 _ChaptersCount;
        private string _FileName;


        private string GetSpecificMediaInfo(StreamKind KindOfStream, int trackindex, string NameOfParameter)
        {
            return Get(KindOfStream, trackindex, NameOfParameter);
        }

        private void getStreamCount()
        {
            _AudioCount = Count_Get(StreamKind.Audio);
            _VideoCount = Count_Get(StreamKind.Video);
            _GeneralCount = Count_Get(StreamKind.General);
            _TextCount = Count_Get(StreamKind.Text);
            _ChaptersCount = Count_Get(StreamKind.Menu);
        }

        private void getAllInfos()
        {
            getVideoInfo();
            getAudioInfo();
            getChaptersInfo();
            getTextInfo();
            getGeneralInfo();
        }

        ///<summary> List of all the General streams available in the file, type GeneralTrack[trackindex] to access a specific track</summary>
        public List<GeneralTrack> General
        {
            get
            {
                if (this._General == null)
                {
                   getGeneralInfo();
                }
                return this._General;
            }
        }

        private void getGeneralInfo()
        {
            if (this._General == null)
            {
                this._General = new List<GeneralTrack>();
                int num1 = Count_Get(StreamKind.General);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        GeneralTrack _tracktemp_ = new GeneralTrack();              
                        _tracktemp_.Count= GetSpecificMediaInfo(StreamKind.General,num2,"Count");
                        _tracktemp_.StreamCount= GetSpecificMediaInfo(StreamKind.General,num2,"StreamCount");
                        _tracktemp_.StreamKind= GetSpecificMediaInfo(StreamKind.General,num2,"StreamKind");
                        _tracktemp_.StreamKindID= GetSpecificMediaInfo(StreamKind.General,num2,"StreamKindID");
                        _tracktemp_.StreamOrder = GetSpecificMediaInfo(StreamKind.General, num2, "StreamOrder");
                        _tracktemp_.Inform= GetSpecificMediaInfo(StreamKind.General,num2,"Inform");
                        _tracktemp_.ID= GetSpecificMediaInfo(StreamKind.General,num2,"ID");
                        _tracktemp_.UniqueID= GetSpecificMediaInfo(StreamKind.General,num2,"UniqueID");
                        _tracktemp_.GeneralCount= GetSpecificMediaInfo(StreamKind.General,num2,"GeneralCount");
                        _tracktemp_.VideoCount= GetSpecificMediaInfo(StreamKind.General,num2,"VideoCount");
                        _tracktemp_.AudioCount= GetSpecificMediaInfo(StreamKind.General,num2,"AudioCount");
                        _tracktemp_.TextCount= GetSpecificMediaInfo(StreamKind.General,num2,"TextCount");
                        _tracktemp_.ChaptersCount= GetSpecificMediaInfo(StreamKind.General,num2,"ChaptersCount");
                        _tracktemp_.ImageCount= GetSpecificMediaInfo(StreamKind.General,num2,"ImageCount");
                        _tracktemp_.CompleteName= GetSpecificMediaInfo(StreamKind.General,num2,"CompleteName");
                        _tracktemp_.FolderName= GetSpecificMediaInfo(StreamKind.General,num2,"FolderName");
                        _tracktemp_.FileName= GetSpecificMediaInfo(StreamKind.General,num2,"FileName");
                        _tracktemp_.FileExtension= GetSpecificMediaInfo(StreamKind.General,num2,"FileExtension");
                        _tracktemp_.FileSize= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize");
                        _tracktemp_.FileSizeString= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize/String");
                        _tracktemp_.FileSizeString1= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize/String1");
                        _tracktemp_.FileSizeString2= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize/String2");
                        _tracktemp_.FileSizeString3= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize/String3");
                        _tracktemp_.FileSizeString4= GetSpecificMediaInfo(StreamKind.General,num2,"FileSize/String4");
                        _tracktemp_.Format= GetSpecificMediaInfo(StreamKind.General,num2,"Format");
                        _tracktemp_.FormatString= GetSpecificMediaInfo(StreamKind.General,num2,"Format/String");
                        _tracktemp_.FormatInfo= GetSpecificMediaInfo(StreamKind.General,num2,"Format/Info");
                        _tracktemp_.FormatUrl= GetSpecificMediaInfo(StreamKind.General,num2,"Format/Url");
                        _tracktemp_.FormatExtensions= GetSpecificMediaInfo(StreamKind.General,num2,"Format/Extensions");
                        _tracktemp_.OveralBitRate= GetSpecificMediaInfo(StreamKind.General,num2,"OveralBitRate");
                        _tracktemp_.OveralBitRateString= GetSpecificMediaInfo(StreamKind.General,num2,"OveralBitRate/String");
                        _tracktemp_.PlayTime= GetSpecificMediaInfo(StreamKind.General,num2,"PlayTime");
                        _tracktemp_.PlayTimeString= GetSpecificMediaInfo(StreamKind.General,num2,"PlayTime/String");
                        _tracktemp_.PlayTimeString1= GetSpecificMediaInfo(StreamKind.General,num2,"PlayTime/String1");
                        _tracktemp_.PlayTimeString2= GetSpecificMediaInfo(StreamKind.General,num2,"PlayTime/String2");
                        _tracktemp_.PlayTimeString3= GetSpecificMediaInfo(StreamKind.General,num2,"PlayTime/String3");
                        _tracktemp_.Title= GetSpecificMediaInfo(StreamKind.General,num2,"Title");
                        _tracktemp_.TitleMore= GetSpecificMediaInfo(StreamKind.General,num2,"Title/More");
                        _tracktemp_.Domain= GetSpecificMediaInfo(StreamKind.General,num2,"Domain");
                        _tracktemp_.Collection= GetSpecificMediaInfo(StreamKind.General,num2,"Collection");
                        _tracktemp_.CollectionTotalParts= GetSpecificMediaInfo(StreamKind.General,num2,"Collection/Total_Parts");
                        _tracktemp_.Season= GetSpecificMediaInfo(StreamKind.General,num2,"Season");
                        _tracktemp_.Movie= GetSpecificMediaInfo(StreamKind.General,num2,"Movie");
                        _tracktemp_.MovieMore= GetSpecificMediaInfo(StreamKind.General,num2,"Movie/More");
                        _tracktemp_.Album= GetSpecificMediaInfo(StreamKind.General,num2,"Album");
                        _tracktemp_.AlbumTotalParts= GetSpecificMediaInfo(StreamKind.General,num2,"Album/Total_Parts");
                        _tracktemp_.AlbumSort= GetSpecificMediaInfo(StreamKind.General,num2,"Album/Sort");
                        _tracktemp_.Comic= GetSpecificMediaInfo(StreamKind.General,num2,"Comic");
                        _tracktemp_.ComicTotalParts= GetSpecificMediaInfo(StreamKind.General,num2,"Comic/Total_Parts");
                        _tracktemp_.Part= GetSpecificMediaInfo(StreamKind.General,num2,"Part");
                        _tracktemp_.PartTotalParts= GetSpecificMediaInfo(StreamKind.General,num2,"Part/Total_Parts");
                        _tracktemp_.PartPosition= GetSpecificMediaInfo(StreamKind.General,num2,"Part/Position");
                        _tracktemp_.Track= GetSpecificMediaInfo(StreamKind.General,num2,"Track");
                        _tracktemp_.TrackPosition= GetSpecificMediaInfo(StreamKind.General,num2,"Track/Position");
                        _tracktemp_.TrackMore= GetSpecificMediaInfo(StreamKind.General,num2,"Track/More");
                        _tracktemp_.TrackSort= GetSpecificMediaInfo(StreamKind.General,num2,"Track/Sort");
                        _tracktemp_.Chapter= GetSpecificMediaInfo(StreamKind.General,num2,"Chapter");
                        _tracktemp_.SubTrack= GetSpecificMediaInfo(StreamKind.General,num2,"SubTrack");
                        _tracktemp_.OriginalAlbum= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Album");
                        _tracktemp_.OriginalMovie= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Movie");
                        _tracktemp_.OriginalPart= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Part");
                        _tracktemp_.OriginalTrack= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Track");
                        _tracktemp_.Author= GetSpecificMediaInfo(StreamKind.General,num2,"Author");
                        _tracktemp_.Artist= GetSpecificMediaInfo(StreamKind.General,num2,"Artist");
                        _tracktemp_.PerformerSort= GetSpecificMediaInfo(StreamKind.General,num2,"Performer/Sort");
                        _tracktemp_.OriginalPerformer= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Performer");
                        _tracktemp_.Accompaniment= GetSpecificMediaInfo(StreamKind.General,num2,"Accompaniment");
                        _tracktemp_.MusicianInstrument= GetSpecificMediaInfo(StreamKind.General,num2,"Musician_Instrument");
                        _tracktemp_.Composer= GetSpecificMediaInfo(StreamKind.General,num2,"Composer");
                        _tracktemp_.ComposerNationality= GetSpecificMediaInfo(StreamKind.General,num2,"Composer/Nationality");
                        _tracktemp_.Arranger= GetSpecificMediaInfo(StreamKind.General,num2,"Arranger");
                        _tracktemp_.Lyricist= GetSpecificMediaInfo(StreamKind.General,num2,"Lyricist");
                        _tracktemp_.OriginalLyricist= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Lyricist");
                        _tracktemp_.Conductor= GetSpecificMediaInfo(StreamKind.General,num2,"Conductor");
                        _tracktemp_.Actor= GetSpecificMediaInfo(StreamKind.General,num2,"Actor");
                        _tracktemp_.ActorCharacter= GetSpecificMediaInfo(StreamKind.General,num2,"Actor_Character");
                        _tracktemp_.WrittenBy= GetSpecificMediaInfo(StreamKind.General,num2,"WrittenBy");
                        _tracktemp_.ScreenplayBy= GetSpecificMediaInfo(StreamKind.General,num2,"ScreenplayBy");
                        _tracktemp_.Director= GetSpecificMediaInfo(StreamKind.General,num2,"Director");
                        _tracktemp_.AssistantDirector= GetSpecificMediaInfo(StreamKind.General,num2,"AssistantDirector");
                        _tracktemp_.DirectorOfPhotography= GetSpecificMediaInfo(StreamKind.General,num2,"DirectorOfPhotography");
                        _tracktemp_.ArtDirector= GetSpecificMediaInfo(StreamKind.General,num2,"ArtDirector");
                        _tracktemp_.EditedBy= GetSpecificMediaInfo(StreamKind.General,num2,"EditedBy");
                        _tracktemp_.Producer= GetSpecificMediaInfo(StreamKind.General,num2,"Producer");
                        _tracktemp_.CoProducer= GetSpecificMediaInfo(StreamKind.General,num2,"CoProducer");
                        _tracktemp_.ExecutiveProducer= GetSpecificMediaInfo(StreamKind.General,num2,"ExecutiveProducer");
                        _tracktemp_.ProductionDesigner= GetSpecificMediaInfo(StreamKind.General,num2,"ProductionDesigner");
                        _tracktemp_.CostumeDesigner= GetSpecificMediaInfo(StreamKind.General,num2,"CostumeDesigner");
                        _tracktemp_.Choregrapher= GetSpecificMediaInfo(StreamKind.General,num2,"Choregrapher");
                        _tracktemp_.SoundEngineer= GetSpecificMediaInfo(StreamKind.General,num2,"SoundEngineer");
                        _tracktemp_.MasteredBy= GetSpecificMediaInfo(StreamKind.General,num2,"MasteredBy");
                        _tracktemp_.RemixedBy= GetSpecificMediaInfo(StreamKind.General,num2,"RemixedBy");
                        _tracktemp_.ProductionStudio= GetSpecificMediaInfo(StreamKind.General,num2,"ProductionStudio");
                        _tracktemp_.Publisher= GetSpecificMediaInfo(StreamKind.General,num2,"Publisher");
                        _tracktemp_.PublisherURL= GetSpecificMediaInfo(StreamKind.General,num2,"Publisher/URL");
                        _tracktemp_.DistributedBy= GetSpecificMediaInfo(StreamKind.General,num2,"DistributedBy");
                        _tracktemp_.EncodedBy= GetSpecificMediaInfo(StreamKind.General,num2,"EncodedBy");
                        _tracktemp_.ThanksTo= GetSpecificMediaInfo(StreamKind.General,num2,"ThanksTo");
                        _tracktemp_.Technician= GetSpecificMediaInfo(StreamKind.General,num2,"Technician");
                        _tracktemp_.CommissionedBy= GetSpecificMediaInfo(StreamKind.General,num2,"CommissionedBy");
                        _tracktemp_.EncodedOriginalDistributedBy= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Original/DistributedBy");
                        _tracktemp_.RadioStation= GetSpecificMediaInfo(StreamKind.General,num2,"RadioStation");
                        _tracktemp_.RadioStationOwner= GetSpecificMediaInfo(StreamKind.General,num2,"RadioStation/Owner");
                        _tracktemp_.RadioStationURL= GetSpecificMediaInfo(StreamKind.General,num2,"RadioStation/URL");
                        _tracktemp_.ContentType= GetSpecificMediaInfo(StreamKind.General,num2,"ContentType");
                        _tracktemp_.Subject= GetSpecificMediaInfo(StreamKind.General,num2,"Subject");
                        _tracktemp_.Synopsys= GetSpecificMediaInfo(StreamKind.General,num2,"Synopsys");
                        _tracktemp_.Summary= GetSpecificMediaInfo(StreamKind.General,num2,"Summary");
                        _tracktemp_.Description= GetSpecificMediaInfo(StreamKind.General,num2,"Description");
                        _tracktemp_.Keywords= GetSpecificMediaInfo(StreamKind.General,num2,"Keywords");
                        _tracktemp_.Period= GetSpecificMediaInfo(StreamKind.General,num2,"Period");
                        _tracktemp_.LawRating= GetSpecificMediaInfo(StreamKind.General,num2,"LawRating");
                        _tracktemp_.IRCA= GetSpecificMediaInfo(StreamKind.General,num2,"IRCA");
                        _tracktemp_.Language= GetSpecificMediaInfo(StreamKind.General,num2,"Language");
                        _tracktemp_.Medium= GetSpecificMediaInfo(StreamKind.General,num2,"Medium");
                        _tracktemp_.Product= GetSpecificMediaInfo(StreamKind.General,num2,"Product");
                        _tracktemp_.Country= GetSpecificMediaInfo(StreamKind.General,num2,"Country");
                        _tracktemp_.WrittenDate= GetSpecificMediaInfo(StreamKind.General,num2,"Written_Date");
                        _tracktemp_.RecordedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Recorded_Date");
                        _tracktemp_.ReleasedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Released_Date");
                        _tracktemp_.MasteredDate= GetSpecificMediaInfo(StreamKind.General,num2,"Mastered_Date");
                        _tracktemp_.EncodedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Date");
                        _tracktemp_.TaggedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Tagged_Date");
                        _tracktemp_.OriginalReleasedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Released_Date");
                        _tracktemp_.OriginalRecordedDate= GetSpecificMediaInfo(StreamKind.General,num2,"Original/Recorded_Date");
                        _tracktemp_.WrittenLocation= GetSpecificMediaInfo(StreamKind.General,num2,"Written_Location");
                        _tracktemp_.RecordedLocation= GetSpecificMediaInfo(StreamKind.General,num2,"Recorded_Location");
                        _tracktemp_.ArchivalLocation= GetSpecificMediaInfo(StreamKind.General,num2,"Archival_Location");
                        _tracktemp_.Genre= GetSpecificMediaInfo(StreamKind.General,num2,"Genre");
                        _tracktemp_.Mood= GetSpecificMediaInfo(StreamKind.General,num2,"Mood");
                        _tracktemp_.Comment= GetSpecificMediaInfo(StreamKind.General,num2,"Comment");
                        _tracktemp_.Rating= GetSpecificMediaInfo(StreamKind.General,num2,"Rating ");
                        _tracktemp_.EncodedApplication= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Application");
                        _tracktemp_.EncodedLibrary= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibrarySettings= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Library_Settings");
                        _tracktemp_.EncodedOriginal= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Original");
                        _tracktemp_.EncodedOriginalUrl= GetSpecificMediaInfo(StreamKind.General,num2,"Encoded_Original/Url");
                        _tracktemp_.Copyright= GetSpecificMediaInfo(StreamKind.General,num2,"Copyright");
                        _tracktemp_.ProducerCopyright= GetSpecificMediaInfo(StreamKind.General,num2,"Producer_Copyright");
                        _tracktemp_.TermsOfUse= GetSpecificMediaInfo(StreamKind.General,num2,"TermsOfUse");
                        _tracktemp_.CopyrightUrl= GetSpecificMediaInfo(StreamKind.General,num2,"Copyright/Url");
                        _tracktemp_.ISRC= GetSpecificMediaInfo(StreamKind.General,num2,"ISRC");
                        _tracktemp_.MSDI= GetSpecificMediaInfo(StreamKind.General,num2,"MSDI");
                        _tracktemp_.ISBN= GetSpecificMediaInfo(StreamKind.General,num2,"ISBN");
                        _tracktemp_.BarCode= GetSpecificMediaInfo(StreamKind.General,num2,"BarCode");
                        _tracktemp_.LCCN= GetSpecificMediaInfo(StreamKind.General,num2,"LCCN");
                        _tracktemp_.CatalogNumber= GetSpecificMediaInfo(StreamKind.General,num2,"CatalogNumber");
                        _tracktemp_.LabelCode= GetSpecificMediaInfo(StreamKind.General,num2,"LabelCode");
                        _tracktemp_.Cover= GetSpecificMediaInfo(StreamKind.General,num2,"Cover");
                        _tracktemp_.CoverDatas= GetSpecificMediaInfo(StreamKind.General,num2,"Cover_Datas");
                        _tracktemp_.BPM= GetSpecificMediaInfo(StreamKind.General,num2,"BPM");
                        _tracktemp_.VideoCodecList= GetSpecificMediaInfo(StreamKind.General,num2,"Video_Codec_List");
                        _tracktemp_.VideoLanguageList= GetSpecificMediaInfo(StreamKind.General,num2,"Video_Language_List");
                        _tracktemp_.AudioCodecList= GetSpecificMediaInfo(StreamKind.General,num2,"Audio_Codec_List");
                        _tracktemp_.AudioLanguageList= GetSpecificMediaInfo(StreamKind.General,num2,"Audio_Language_List");
                        _tracktemp_.TextCodecList= GetSpecificMediaInfo(StreamKind.General,num2,"Text_Codec_List");
                        _tracktemp_.TextLanguageList= GetSpecificMediaInfo(StreamKind.General,num2,"Text_Language_List");
                        _tracktemp_.ChaptersCodecList= GetSpecificMediaInfo(StreamKind.General,num2,"Chapters_Codec_List");
                        _tracktemp_.ChaptersLanguageList= GetSpecificMediaInfo(StreamKind.General,num2,"Chapters_Language_List");
                        _tracktemp_.ImageCodecList= GetSpecificMediaInfo(StreamKind.General,num2,"Image_Codec_List");
                        _tracktemp_.ImageLanguageList= GetSpecificMediaInfo(StreamKind.General,num2,"Image_Language_List");
                        _tracktemp_.Other= GetSpecificMediaInfo(StreamKind.General,num2,"Other");
                        _tracktemp_.Attachments = GetSpecificMediaInfo(StreamKind.General, num2, "Attachments");
                        this._General.Add(_tracktemp_);
                    }
                }  
            }
        }

        ///<summary> List of all the Video streams available in the file, type VideoTrack[trackindex] to access a specific track</summary>
        public List<VideoTrack> Video
        {
            get
            {
                if (this._Video == null)
                {
                   getVideoInfo();
                }
                return this._Video;
            }
        }

        private void getVideoInfo()
        {
            if (this._Video == null)
            {
                this._Video = new List<VideoTrack>();
                int num1 = Count_Get(StreamKind.Video);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        VideoTrack _tracktemp_ = new VideoTrack();                            
                        _tracktemp_.Count = GetSpecificMediaInfo(StreamKind.Video,num2,"Count");
                        _tracktemp_.StreamCount = GetSpecificMediaInfo(StreamKind.Video,num2,"StreamCount");
                        _tracktemp_.StreamKind = GetSpecificMediaInfo(StreamKind.Video,num2,"StreamKind");
                        _tracktemp_.StreamKindID = GetSpecificMediaInfo(StreamKind.Video,num2,"StreamKindID");
                        _tracktemp_.StreamOrder = GetSpecificMediaInfo(StreamKind.Video, num2, "StreamOrder");
                        _tracktemp_.Inform = GetSpecificMediaInfo(StreamKind.Video,num2,"Inform");
                        _tracktemp_.ID = GetSpecificMediaInfo(StreamKind.Video,num2,"ID");
                        _tracktemp_.UniqueID = GetSpecificMediaInfo(StreamKind.Video,num2,"UniqueID");
                        _tracktemp_.Title = GetSpecificMediaInfo(StreamKind.Video,num2,"Title");
                        _tracktemp_.Codec = GetSpecificMediaInfo(StreamKind.Video,num2,"Codec");
                        _tracktemp_.CodecString = GetSpecificMediaInfo(StreamKind.Video,num2,"Codec/String");
                        _tracktemp_.CodecInfo = GetSpecificMediaInfo(StreamKind.Video,num2,"Codec/Info");
                        _tracktemp_.CodecUrl = GetSpecificMediaInfo(StreamKind.Video,num2,"Codec/Url");
                        _tracktemp_.CodecID = GetSpecificMediaInfo(StreamKind.Video, num2, "CodecID");
                        _tracktemp_.CodecIDInfo = GetSpecificMediaInfo(StreamKind.Video, num2, "CodecID/Info");
                        _tracktemp_.BitRate = GetSpecificMediaInfo(StreamKind.Video,num2,"BitRate");
                        _tracktemp_.BitRateString = GetSpecificMediaInfo(StreamKind.Video,num2,"BitRate/String");
                        _tracktemp_.BitRateMode = GetSpecificMediaInfo(StreamKind.Video,num2,"BitRate_Mode");
                        _tracktemp_.EncodedLibrary = GetSpecificMediaInfo(StreamKind.Video,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibrarySettings = GetSpecificMediaInfo(StreamKind.Video,num2,"Encoded_Library_Settings");
                        _tracktemp_.Width = GetSpecificMediaInfo(StreamKind.Video,num2,"Width");
                        _tracktemp_.Height = GetSpecificMediaInfo(StreamKind.Video,num2,"Height");
                        _tracktemp_.AspectRatio = GetSpecificMediaInfo(StreamKind.Video,num2,"AspectRatio");
                        _tracktemp_.AspectRatioString = GetSpecificMediaInfo(StreamKind.Video, num2, "AspectRatio/String");
                        _tracktemp_.PixelAspectRatio = GetSpecificMediaInfo(StreamKind.Video, num2, "PixelAspectRatio");
                        _tracktemp_.PixelAspectRatioString = GetSpecificMediaInfo(StreamKind.Video, num2, "PixelAspectRatio/String");
                        _tracktemp_.FrameRate = GetSpecificMediaInfo(StreamKind.Video,num2,"FrameRate");
                        _tracktemp_.FrameRateString = GetSpecificMediaInfo(StreamKind.Video,num2,"FrameRate/String");
                        _tracktemp_.FrameRateOriginal = GetSpecificMediaInfo(StreamKind.Video, num2, "FrameRate_Original");
                        _tracktemp_.FrameRateOriginalString = GetSpecificMediaInfo(StreamKind.Video, num2, "FrameRate_Original/String");
                        _tracktemp_.FrameRateMode = GetSpecificMediaInfo(StreamKind.Video, num2, "FrameRate_Mode");
                        _tracktemp_.FrameRateModeString = GetSpecificMediaInfo(StreamKind.Video, num2, "FrameRate_Mode/String");
                        _tracktemp_.FrameCount = GetSpecificMediaInfo(StreamKind.Video,num2,"FrameCount");
                        _tracktemp_.BitDepth = GetSpecificMediaInfo(StreamKind.Video,num2,"BitDepth");
                        _tracktemp_.BitsPixelFrame = GetSpecificMediaInfo(StreamKind.Video,num2,"Bits/(Pixel*Frame)");
                        _tracktemp_.Delay = GetSpecificMediaInfo(StreamKind.Video,num2,"Delay");
                        _tracktemp_.Duration = GetSpecificMediaInfo(StreamKind.Video,num2,"Duration");
                        _tracktemp_.DurationString = GetSpecificMediaInfo(StreamKind.Video,num2,"Duration/String");
                        _tracktemp_.DurationString1 = GetSpecificMediaInfo(StreamKind.Video,num2,"Duration/String1");
                        _tracktemp_.DurationString2 = GetSpecificMediaInfo(StreamKind.Video,num2,"Duration/String2");
                        _tracktemp_.DurationString3 = GetSpecificMediaInfo(StreamKind.Video,num2,"Duration/String3");
                        _tracktemp_.Language = GetSpecificMediaInfo(StreamKind.Video,num2,"Language");
                        _tracktemp_.LanguageString = GetSpecificMediaInfo(StreamKind.Video,num2,"Language/String");
                        _tracktemp_.LanguageMore = GetSpecificMediaInfo(StreamKind.Video,num2,"Language_More");
                        _tracktemp_.Format = GetSpecificMediaInfo(StreamKind.Video, num2, "Format");
                        _tracktemp_.FormatInfo = GetSpecificMediaInfo(StreamKind.Video, num2, "Format/Info");
                        _tracktemp_.FormatProfile = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Profile");
                        _tracktemp_.FormatSettings = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings");
                        _tracktemp_.FormatSettingsBVOP = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_BVOP");
                        _tracktemp_.FormatSettingsBVOPString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_BVOP/String");
                        _tracktemp_.FormatSettingsCABAC = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_CABAC");
                        _tracktemp_.FormatSettingsCABACString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_CABAC/String");
                        _tracktemp_.FormatSettingsGMC = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_GMC");
                        _tracktemp_.FormatSettingsGMCString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_GMAC/String");
                        _tracktemp_.FormatSettingsMatrix = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_Matrix");
                        _tracktemp_.FormatSettingsMatrixData = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_Matrix_Data");
                        _tracktemp_.FormatSettingsMatrixString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_Matrix/String");
                        _tracktemp_.FormatSettingsPulldown = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_Pulldown");
                        _tracktemp_.FormatSettingsQPel = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_QPel");
                        _tracktemp_.FormatSettingsQPelString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_QPel/String");
                        _tracktemp_.FormatSettingsRefFrames = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_RefFrames");
                        _tracktemp_.FormatSettingsRefFramesString = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Settings_RefFrames/String");
                        _tracktemp_.ScanType = GetSpecificMediaInfo(StreamKind.Video, num2, "ScanType");
                        _tracktemp_.ScanTypeString = GetSpecificMediaInfo(StreamKind.Video, num2, "ScanType/String");
                        _tracktemp_.FormatUrl = GetSpecificMediaInfo(StreamKind.Video, num2, "Format/Url");
                        _tracktemp_.FormatVersion = GetSpecificMediaInfo(StreamKind.Video, num2, "Format_Version");
                        _tracktemp_.Default = GetSpecificMediaInfo(StreamKind.Video, num2, "Default");
                        _tracktemp_.DefaultString = GetSpecificMediaInfo(StreamKind.Video, num2, "Default/String");
                        _tracktemp_.Forced = GetSpecificMediaInfo(StreamKind.Video, num2, "Forced");
                        _tracktemp_.ForcedString = GetSpecificMediaInfo(StreamKind.Video, num2, "Forced/String");
                         this._Video.Add(_tracktemp_);
                    }
                }
            }
        }

        ///<summary> List of all the Audio streams available in the file, type AudioTrack[trackindex] to access a specific track</summary>
        public List<AudioTrack> Audio
        {
            get
            {
                if (this._Audio == null)
                {
                   getAudioInfo();
                }
                return this._Audio;
            }
        }

        private void getAudioInfo()
        {
            if (this._Audio == null)
            {
                this._Audio = new List<AudioTrack>();
                int num1 = Count_Get(StreamKind.Audio);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        AudioTrack _tracktemp_ = new AudioTrack();                                              
                        _tracktemp_.Count= GetSpecificMediaInfo(StreamKind.Audio,num2,"Count");
                        _tracktemp_.StreamCount= GetSpecificMediaInfo(StreamKind.Audio,num2,"StreamCount");
                        _tracktemp_.StreamKind= GetSpecificMediaInfo(StreamKind.Audio,num2,"StreamKind");
                        _tracktemp_.StreamKindString = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamKind/String");
                        _tracktemp_.StreamKindID= GetSpecificMediaInfo(StreamKind.Audio,num2,"StreamKindID");
                        _tracktemp_.StreamKindPos = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamKindPos");
                        _tracktemp_.StreamOrder = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamOrder");
                        _tracktemp_.Inform= GetSpecificMediaInfo(StreamKind.Audio,num2,"Inform");
                        _tracktemp_.ID= GetSpecificMediaInfo(StreamKind.Audio,num2,"ID");
                        _tracktemp_.IDString = GetSpecificMediaInfo(StreamKind.Audio, num2, "ID/String");
                        _tracktemp_.UniqueID= GetSpecificMediaInfo(StreamKind.Audio,num2,"UniqueID");
                        _tracktemp_.MenuID = GetSpecificMediaInfo(StreamKind.Audio, num2, "MenuID");
                        _tracktemp_.MenuIDString = GetSpecificMediaInfo(StreamKind.Audio, num2, "MenuID/String");
                        _tracktemp_.Format = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format");
                        _tracktemp_.FormatInfo = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format/Info");
                        _tracktemp_.FormatUrl = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format/Url");
                        _tracktemp_.FormatVersion = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Version");
                        _tracktemp_.FormatProfile = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Profile");
                        _tracktemp_.FormatSettings = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings");
                        _tracktemp_.FormatSettingsSBR = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_SBR");
                        _tracktemp_.FormatSettingsSBRString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_SBR/String");
                        _tracktemp_.FormatSettingsPS = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_PS");
                        _tracktemp_.FormatSettingsPSString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_PS/String");
                        _tracktemp_.FormatSettingsFloor = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_Floor");
                        _tracktemp_.FormatSettingsFirm = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_Firm");
                        _tracktemp_.FormatSettingsEndianness = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_Endianness");
                        _tracktemp_.FormatSettingsSign = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_Sign");
                        _tracktemp_.FormatSettingsLaw = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_Law");
                        _tracktemp_.FormatSettingsITU = GetSpecificMediaInfo(StreamKind.Audio, num2, "Format_Settings_ITU");
                        _tracktemp_.MuxingMode = GetSpecificMediaInfo(StreamKind.Audio, num2, "MuxingMode");
                        _tracktemp_.CodecID = GetSpecificMediaInfo(StreamKind.Audio, num2, "CodecID");
                        _tracktemp_.CodecIDInfo = GetSpecificMediaInfo(StreamKind.Audio, num2, "CodecID/Info");
                        _tracktemp_.CodecIDUrl = GetSpecificMediaInfo(StreamKind.Audio, num2, "CodecID/Url");
                        _tracktemp_.CodecIDHint = GetSpecificMediaInfo(StreamKind.Audio, num2, "CodecID/Hint");
                        _tracktemp_.CodecIDDescription = GetSpecificMediaInfo(StreamKind.Audio, num2, "CodecID_Description");
                        _tracktemp_.Duration = GetSpecificMediaInfo(StreamKind.Audio, num2, "Duration");
                        _tracktemp_.DurationString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Duration/String");
                        _tracktemp_.DurationString1 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Duration/String1");
                        _tracktemp_.DurationString2 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Duration/String2");
                        _tracktemp_.DurationString3 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Duration/String3");
                        _tracktemp_.BitRateMode = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Mode");
                        _tracktemp_.BitRateModeString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Mode/String");
                        _tracktemp_.BitRate = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate");
                        _tracktemp_.BitRateString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate/String");
                        _tracktemp_.BitRateMinimum = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Minimum");
                        _tracktemp_.BitRateMinimumString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Minimum/String");
                        _tracktemp_.BitRateNominal = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Nominal");
                        _tracktemp_.BitRateNominalString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Nominal/String");
                        _tracktemp_.BitRateMaximum = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Maximum");
                        _tracktemp_.BitRateMaximumString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitRate_Maximum/String");
                        _tracktemp_.Channels = GetSpecificMediaInfo(StreamKind.Audio, num2, "Channel(s)");
                        _tracktemp_.ChannelsString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Channel(s)/String");
                        _tracktemp_.ChannelMode = GetSpecificMediaInfo(StreamKind.Audio, num2, "ChannelMode");
                        _tracktemp_.ChannelPositions = GetSpecificMediaInfo(StreamKind.Audio, num2, "ChannelPositions");
                        _tracktemp_.ChannelPositionsString2 = GetSpecificMediaInfo(StreamKind.Audio, num2, "ChannelPositions/String2");
                        _tracktemp_.SamplingRate = GetSpecificMediaInfo(StreamKind.Audio, num2, "SamplingRate");
                        _tracktemp_.SamplingRateString = GetSpecificMediaInfo(StreamKind.Audio, num2, "SamplingRate/String");
                        _tracktemp_.SamplingCount = GetSpecificMediaInfo(StreamKind.Audio, num2, "SamplingCount");
                        _tracktemp_.BitDepth = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitDepth");
                        _tracktemp_.BitDepthString = GetSpecificMediaInfo(StreamKind.Audio, num2, "BitDepth/String");
                        _tracktemp_.CompressionRatio = GetSpecificMediaInfo(StreamKind.Audio, num2, "CompressionRatio");
                        _tracktemp_.Delay = GetSpecificMediaInfo(StreamKind.Audio, num2, "Delay");
                        _tracktemp_.DelayString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Delay/String");
                        _tracktemp_.DelayString1 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Delay/String1");
                        _tracktemp_.DelayString2 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Delay/String2");
                        _tracktemp_.DelayString3 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Delay/String3");
                        _tracktemp_.VideoDelay = GetSpecificMediaInfo(StreamKind.Audio, num2, "Video_Delay");
                        _tracktemp_.VideoDelayString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Video_Delay/String");
                        _tracktemp_.VideoDelayString1 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Video_Delay/String1");
                        _tracktemp_.VideoDelayString2 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Video_Delay/String2");
                        _tracktemp_.VideoDelayString3 = GetSpecificMediaInfo(StreamKind.Audio, num2, "Video_Delay/String3");
                        _tracktemp_.ReplayGainGain = GetSpecificMediaInfo(StreamKind.Audio, num2, "ReplayGain_Gain");
                        _tracktemp_.ReplayGainGainString = GetSpecificMediaInfo(StreamKind.Audio, num2, "ReplayGain_Gain/String");
                        _tracktemp_.ReplayGainPeak = GetSpecificMediaInfo(StreamKind.Audio, num2, "ReplayGain_Peak");
                        _tracktemp_.StreamSize = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize");
                        _tracktemp_.StreamSizeString = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String");
                        _tracktemp_.StreamSizeString1 = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String1");
                        _tracktemp_.StreamSizeString2  = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String2");
                        _tracktemp_.StreamSizeString3 = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String3");
                        _tracktemp_.StreamSizeString4 = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String4");
                        _tracktemp_.StreamSizeString5 = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize/String5");
                        _tracktemp_.StreamSizeProportion = GetSpecificMediaInfo(StreamKind.Audio, num2, "StreamSize_Proportion");
                        _tracktemp_.Alignment = GetSpecificMediaInfo(StreamKind.Audio, num2, "Alignment");
                        _tracktemp_.AlignmentString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Alignment/String");
                        _tracktemp_.InterleaveVideoFrames = GetSpecificMediaInfo(StreamKind.Audio, num2, "Interleave_VideoFrames");
                        _tracktemp_.InterleaveDuration = GetSpecificMediaInfo(StreamKind.Audio, num2, "Interleave_Duration");
                        _tracktemp_.InterleaveDurationString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Interleave_Duration/String");
                        _tracktemp_.InterleavePreload = GetSpecificMediaInfo(StreamKind.Audio, num2, "Interleave_Preload");
                        _tracktemp_.InterleavePreloadString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Interleave_Preload/String");
                        _tracktemp_.Title= GetSpecificMediaInfo(StreamKind.Audio,num2,"Title");
                        _tracktemp_.EncodedLibrary= GetSpecificMediaInfo(StreamKind.Audio,num2,"Encoded_Library");
                        _tracktemp_.EncodedLibraryString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encoded_Library/String");
                        _tracktemp_.EncodedLibraryName = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encoded_Library/Name");
                        _tracktemp_.EncodedLibraryVersion = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encoded_Library/Version");
                        _tracktemp_.EncodedLibraryDate = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encoded_Library/Date");
                        _tracktemp_.EncodedLibrarySettings= GetSpecificMediaInfo(StreamKind.Audio,num2,"Encoded_Library_Settings");
                        _tracktemp_.Language= GetSpecificMediaInfo(StreamKind.Audio,num2,"Language");
                        _tracktemp_.LanguageString= GetSpecificMediaInfo(StreamKind.Audio,num2,"Language/String");
                        _tracktemp_.LanguageMore= GetSpecificMediaInfo(StreamKind.Audio,num2,"Language_More");
                        _tracktemp_.EncodedDate = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encoded_Date");
                        _tracktemp_.TaggedDate = GetSpecificMediaInfo(StreamKind.Audio, num2, "Tagged_Date");
                        _tracktemp_.Encryption = GetSpecificMediaInfo(StreamKind.Audio, num2, "Encryption");
                        _tracktemp_.Default = GetSpecificMediaInfo(StreamKind.Audio, num2, "Default");
                        _tracktemp_.DefaultString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Default/String");
                        _tracktemp_.Forced = GetSpecificMediaInfo(StreamKind.Audio, num2, "Forced");
                        _tracktemp_.ForcedString = GetSpecificMediaInfo(StreamKind.Audio, num2, "Forced/String");                        
                        this._Audio.Add(_tracktemp_);
                    }
                }
            }
        }

        ///<summary> List of all the Text streams available in the file, type TextTrack[trackindex] to access a specific track</summary>
        public List<TextTrack> Text
        {
            get
            {
                if (this._Text == null)
                {
                   getTextInfo();
                }
                return this._Text;
            }
        }

        private void getTextInfo()
        {
            if (this._Text == null)
            {
                this._Text = new List<TextTrack>();
                int num1 = Count_Get(StreamKind.Text);
                if (num1 > 0)
                {
                    int num3 = num1 - 1;
                    for (int num2 = 0; num2 <= num3; num2++)
                    {
                        TextTrack _tracktemp_ = new TextTrack();                              
                        _tracktemp_.Count= GetSpecificMediaInfo(StreamKind.Text,num2,"Count");
                        _tracktemp_.StreamCount= GetSpecificMediaInfo(StreamKind.Text,num2,"StreamCount");
                        _tracktemp_.StreamKind= GetSpecificMediaInfo(StreamKind.Text,num2,"StreamKind");
                        _tracktemp_.StreamKindID= GetSpecificMediaInfo(StreamKind.Text,num2,"StreamKindID");
                        _tracktemp_.StreamOrder = GetSpecificMediaInfo(StreamKind.Text, num2, "StreamOrder");
                        _tracktemp_.Inform= GetSpecificMediaInfo(StreamKind.Text,num2,"Inform");
                        _tracktemp_.ID= GetSpecificMediaInfo(StreamKind.Text,num2,"ID");
                        _tracktemp_.UniqueID= GetSpecificMediaInfo(StreamKind.Text,num2,"UniqueID");
                        _tracktemp_.Title= GetSpecificMediaInfo(StreamKind.Text,num2,"Title");
                        _tracktemp_.Codec= GetSpecificMediaInfo(StreamKind.Text,num2,"Codec");
                        _tracktemp_.CodecString= GetSpecificMediaInfo(StreamKind.Text,num2,"Codec/String");
                        _tracktemp_.CodecUrl= GetSpecificMediaInfo(StreamKind.Text,num2,"Codec/Url");
                        _tracktemp_.Delay= GetSpecificMediaInfo(StreamKind.Text,num2,"Delay");
                        _tracktemp_.Video0Delay= GetSpecificMediaInfo(StreamKind.Text,num2,"Video0_Delay");
                        _tracktemp_.PlayTime= GetSpecificMediaInfo(StreamKind.Text,num2,"PlayTime");
                        _tracktemp_.PlayTimeString= GetSpecificMediaInfo(StreamKind.Text,num2,"PlayTime/String");
                        _tracktemp_.PlayTimeString1= GetSpecificMediaInfo(StreamKind.Text,num2,"PlayTime/String1");
                        _tracktemp_.PlayTimeString2= GetSpecificMediaInfo(StreamKind.Text,num2,"PlayTime/String2");
                        _tracktemp_.PlayTimeString3= GetSpecificMediaInfo(StreamKind.Text,num2,"PlayTime/String3");
                        _tracktemp_.Language= GetSpecificMediaInfo(StreamKind.Text,num2,"Language");
                        _tracktemp_.LanguageString= GetSpecificMediaInfo(StreamKind.Text,num2,"Language/String");
                        _tracktemp_.LanguageMore= GetSpecificMediaInfo(StreamKind.Text,num2,"Language_More");
                        _tracktemp_.Default = GetSpecificMediaInfo(StreamKind.Text, num2, "Default");
                        _tracktemp_.DefaultString = GetSpecificMediaInfo(StreamKind.Text, num2, "Default/String");
                        _tracktemp_.Forced = GetSpecificMediaInfo(StreamKind.Text, num2, "Forced");
                        _tracktemp_.ForcedString = GetSpecificMediaInfo(StreamKind.Text, num2, "Forced/String");
                         this._Text.Add(_tracktemp_);
                    }
                }
            }
        }

        ///<summary> List of all the Chapters streams available in the file, type ChaptersTrack[trackindex] to access a specific track</summary>
        public List<ChaptersTrack> Chapters
        {
            get
            {
                if (this._Chapters == null)
                {
                   getChaptersInfo();
                }
                return this._Chapters;
            }
        }

        private void getChaptersInfo()
        {
            if (this._Chapters != null)
                return;

            this._Chapters = new List<ChaptersTrack>();
            int num1 = Count_Get(StreamKind.Menu);
            if (num1 <= 0)
                return;

            int num3 = num1 - 1;
            for (int num2 = 0; num2 <= num3; num2++)
            {
                int iStart = 0;
                int iEnd = 0;
                Int32.TryParse(GetSpecificMediaInfo(StreamKind.Menu, num2, "Chapters_Pos_Begin"), out iStart);
                Int32.TryParse(GetSpecificMediaInfo(StreamKind.Menu, num2, "Chapters_Pos_End"), out iEnd);

                if (iStart == iEnd)
                    continue;

                ChaptersTrack _tracktemp_ = new ChaptersTrack();
                for (int i = iStart; i < iEnd; i++)
                    _tracktemp_.Chapters.Add(Get(StreamKind.Menu, num2, i, InfoKind.Name), Get(StreamKind.Menu, num2, i, InfoKind.Text));

                this._Chapters.Add(_tracktemp_);
            }
        }
    }
}
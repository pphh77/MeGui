using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace MeGUI
{
    #region video and audio codecs
    /// <summary>
    /// Dummy interface to avoid some runtime type errors. This should be implemented by VideoCodec and AudioCodec
    /// </summary>
    public interface ICodec { }
    
    public class VideoCodec : ICodec, IIDable
    {
        private string id;
        public string ID
        {
            get { return id; }
        }
        public VideoCodec(string id)
        {
            this.id = id;
        }
        public static readonly VideoCodec ASP = new VideoCodec("ASP");
        public static readonly VideoCodec AVC = new VideoCodec("AVC");
        public static readonly VideoCodec SNOW = new VideoCodec("SNOW");
        public static readonly VideoCodec HFYU = new VideoCodec("HFYU");
    }
    public class AudioCodec : ICodec, IIDable
    {
        private string id;
        public string ID
        {
            get { return id; }
        }
        public AudioCodec(string id)
        {
            this.id = id;
        }
        public static readonly AudioCodec MP3 = new AudioCodec("MP3");
        public static readonly AudioCodec AAC = new AudioCodec("AAC");
        public static readonly AudioCodec VORBIS = new AudioCodec("VORBIS");
        public static readonly AudioCodec DTS = new AudioCodec("DTS");
        public static readonly AudioCodec AC3 = new AudioCodec("AC3");
        public static readonly AudioCodec MP2 = new AudioCodec("MP2");
        public static readonly AudioCodec WAV = new AudioCodec("WAV");
        public static readonly AudioCodec PCM = new AudioCodec("PCM");
    }
    public class SubtitleCodec : ICodec, IIDable
    {
        private string id;
        public string ID
        {
            get { return id; }
        }
        public SubtitleCodec(string id)
        {
            this.id = id;
        }
        public static readonly SubtitleCodec TEXT = new SubtitleCodec("TEXT");
        public static readonly SubtitleCodec IMAGE = new SubtitleCodec("IMAGE");
    }
    #endregion
    #region video and audio encoder types
    /// <summary>
    /// Dummy interface so runtime typing problems don't arise, and we can avoid ugly (object) casts
    /// </summary>
    public interface IEncoderType
    {
        ICodec Codec
        {
            get;
        }
    }

    public class VideoEncoderType : IEncoderType, IIDable
    {
        private string id;
        private VideoCodec codec;
        public VideoCodec VCodec
        {
            get { return codec; }
        }
        public ICodec Codec
        {
            get { return VCodec; }
        }

        public string ID
        {
            get { return id; }
        }
        public VideoEncoderType(string id, VideoCodec codec)
        {
            this.id = id;
            this.codec = codec;
        }
        public static readonly VideoEncoderType XVID = new VideoEncoderType("XVID", VideoCodec.ASP);
        public static readonly VideoEncoderType LMP4 = new VideoEncoderType("LMP4", VideoCodec.ASP);
        public static readonly VideoEncoderType X264 = new VideoEncoderType("X264", VideoCodec.AVC);
        public static readonly VideoEncoderType X264FARM = new VideoEncoderType("X264FARM", VideoCodec.AVC);
        public static readonly VideoEncoderType SNOW = new VideoEncoderType("SNOW", VideoCodec.SNOW);
        public static readonly VideoEncoderType HFYU = new VideoEncoderType("HFYU", VideoCodec.HFYU);
    }
    public class AudioEncoderType : IEncoderType, IIDable
    {
        private string id;
        private AudioCodec codec;
        public ICodec Codec
        {
            get { return ACodec; }
        }
        public AudioCodec ACodec
        {
            get { return codec; }
        }
        public string ID
        {
            get { return id; }
        }
        public AudioEncoderType(string id, AudioCodec codec)
        {
            this.id = id;
            this.codec = codec;
        }
        public static readonly AudioEncoderType LAME = new AudioEncoderType("LAME", AudioCodec.MP3);
        public static readonly AudioEncoderType AUDX = new AudioEncoderType("AUDX", AudioCodec.MP3);
        public static readonly AudioEncoderType WAAC = new AudioEncoderType("WAAC", AudioCodec.AAC);
        public static readonly AudioEncoderType NAAC = new AudioEncoderType("NAAC", AudioCodec.AAC);
        public static readonly AudioEncoderType FAAC = new AudioEncoderType("FAAC", AudioCodec.AAC);
        public static readonly AudioEncoderType VORBIS = new AudioEncoderType("VORBIS", AudioCodec.VORBIS);
        public static readonly AudioEncoderType FFAC3 = new AudioEncoderType("FFAC3", AudioCodec.AC3);
        public static readonly AudioEncoderType FFMP2 = new AudioEncoderType("FFMP2", AudioCodec.MP2);
        public static readonly AudioEncoderType AFTEN = new AudioEncoderType("AFTEN", AudioCodec.AC3);
    }
    #endregion
    
    class CodecManager
    {
        public static GenericRegisterer<VideoCodec> VideoCodecs = new GenericRegisterer<VideoCodec>();
        public static GenericRegisterer<AudioCodec> AudioCodecs = new GenericRegisterer<AudioCodec>();
        public static GenericRegisterer<VideoEncoderType> VideoEncoderTypes = new GenericRegisterer<VideoEncoderType>();
        public static GenericRegisterer<AudioEncoderType> AudioEncoderTypes = new GenericRegisterer<AudioEncoderType>();
        
        // Audio Codecs
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> NAAC = new NeroAACSettingsProvider();
/*        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> WAAC = new WinAmpAACSettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> Lame = new LameMP3SettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> FAAC = new FaacSettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> Vorbis = new OggVorbisSettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> AudX = new AudXSettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> AC3 = new AC3SettingsProvider();
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType> MP2 = new MP2SettingsProvider();*/

        // Video Codecs
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType> X264 = new X264SettingsProvider();
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType> X264FARM = new X264FARMSettingsProvider();
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType> Snow = new SnowSettingsProvider();
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType> Lavc = new LavcSettingsProvider();
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType> XviD = new XviDSettingsProvider();

        // All Audio Codecs
#warning this must be renamed
        public static readonly ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType>[] ListOfAudioCodecs = new ISettingsProvider<AudioCodecSettings, string[], AudioCodec, AudioEncoderType>[] { NAAC/*, WAAC, Lame, AudX, Vorbis, AC3, MP2, FAAC*/};
        // All Audio Codecs
#warning this must be renamed
        public static readonly ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType>[] ListOfVideoCodecs = new ISettingsProvider<VideoCodecSettings, VideoInfo, VideoCodec, VideoEncoderType>[] { Lavc, X264, Snow, XviD, X264FARM };
        
        static CodecManager()
        {
            if (!(
                VideoCodecs.Register(VideoCodec.ASP) &&
                VideoCodecs.Register(VideoCodec.AVC) &&
                VideoCodecs.Register(VideoCodec.HFYU) &&
                VideoCodecs.Register(VideoCodec.SNOW)))
                throw new Exception("Failed to register a standard video codec");
            if (!(
                AudioCodecs.Register(AudioCodec.AAC) &&
                AudioCodecs.Register(AudioCodec.AC3) &&
                AudioCodecs.Register(AudioCodec.DTS) &&
                AudioCodecs.Register(AudioCodec.MP2) &&
                AudioCodecs.Register(AudioCodec.MP3) &&
                AudioCodecs.Register(AudioCodec.VORBIS)))
                throw new Exception("Failed to register a standard audio codec");
            if (!(
                VideoEncoderTypes.Register(VideoEncoderType.HFYU) &&
                VideoEncoderTypes.Register(VideoEncoderType.LMP4) &&
                VideoEncoderTypes.Register(VideoEncoderType.SNOW) &&
                VideoEncoderTypes.Register(VideoEncoderType.X264) &&
                VideoEncoderTypes.Register(VideoEncoderType.X264FARM) &&
                VideoEncoderTypes.Register(VideoEncoderType.XVID)))
                throw new Exception("Failed to register a standard video encoder type");
            if (!(
                AudioEncoderTypes.Register(AudioEncoderType.AUDX) &&
                AudioEncoderTypes.Register(AudioEncoderType.FAAC) &&
                AudioEncoderTypes.Register(AudioEncoderType.FFAC3) &&
                AudioEncoderTypes.Register(AudioEncoderType.FFMP2) &&
                AudioEncoderTypes.Register(AudioEncoderType.LAME) &&
                AudioEncoderTypes.Register(AudioEncoderType.NAAC) &&
                AudioEncoderTypes.Register(AudioEncoderType.VORBIS) &&
                AudioEncoderTypes.Register(AudioEncoderType.WAAC) &&
                AudioEncoderTypes.Register(AudioEncoderType.AFTEN)))
                throw new Exception("Failed to register a standard audio encoder type");
                
        }
    }
    #region Video/Audio/Subtitle Types
    public class VideoType : OutputType
    {
        private VideoCodec[] supportedCodecs;

        public VideoCodec[] SupportedCodecs
        {
            get { return supportedCodecs; }
        }

        public VideoType(string name, string filterName, string extension, ContainerType containerType, VideoCodec supportedCodec)
            : this(name, filterName, extension, containerType, new VideoCodec[] { supportedCodec }) { }

        public VideoType(string name, string filterName, string extension, ContainerType containerType, VideoCodec[] supportedCodecs)
            : base(name, filterName, extension, containerType) {
                this.supportedCodecs = supportedCodecs;
        }
        public static readonly VideoType MP4 = new VideoType("MP4", "MP4 Files", "mp4", ContainerType.MP4, new VideoCodec[] { VideoCodec.ASP, VideoCodec.AVC });
        public static readonly VideoType RAWASP = new VideoType("RAWASP", "RAW MPEG-4 ASP Files", "m4v", null, VideoCodec.ASP);
        public static readonly VideoType RAWAVC = new VideoType("RAWAVC", "RAW MPEG-4 AVC Files", "264", null, VideoCodec.AVC);
        public static readonly VideoType MKV = new VideoType("MKV", "Matroska Files", "mkv", ContainerType.MKV, new VideoCodec[] { VideoCodec.ASP, VideoCodec.AVC, VideoCodec.SNOW, VideoCodec.HFYU});
        public static readonly VideoType AVI = new VideoType("AVI", "AVI Files", "avi", ContainerType.AVI, new VideoCodec[] { VideoCodec.ASP, VideoCodec.AVC, VideoCodec.HFYU, VideoCodec.SNOW });
    }
    public class AudioType : OutputType
    {
        private AudioCodec[] supportedCodecs;

        public AudioCodec[] SupportedCodecs
        {
            get { return supportedCodecs; }
        }

        public AudioType(string name, string filterName, string extension, ContainerType containerType, AudioCodec supportedCodec)
            : this(name, filterName, extension, containerType, new AudioCodec[] { supportedCodec }) { }

        public AudioType(string name, string filterName, string extension, ContainerType containerType, AudioCodec[] supportedCodecs)
            : base(name, filterName, extension, containerType) {
                this.supportedCodecs = supportedCodecs;
        }
        public static readonly AudioType MP4AAC = new AudioType("MP4-AAC", "MP4 AAC Files", "mp4", ContainerType.MP4, AudioCodec.AAC);
        public static readonly AudioType M4A = new AudioType("M4A", "MP4 Audio Files", "m4a", ContainerType.MP4, AudioCodec.AAC);
        public static readonly AudioType RAWAAC = new AudioType("Raw-AAC", "RAW AAC Files", "aac", null, AudioCodec.AAC);
        public static readonly AudioType MP3 = new AudioType("MP3", "MP3 Files", "mp3", null, AudioCodec.MP3);
        public static readonly AudioType VORBIS = new AudioType("Ogg", "Ogg Vorbis Files", "ogg", null, AudioCodec.VORBIS);
        public static readonly AudioType AC3 = new AudioType("AC3", "AC3 Files", "ac3", null, AudioCodec.AC3);
        public static readonly AudioType MP2 = new AudioType("MP2", "MP2 Files", "mp2", null, AudioCodec.MP2);
        public static readonly AudioType DTS = new AudioType("DTS", "DTS Files", "dts", null, AudioCodec.DTS);
        public static readonly AudioType WAV = new AudioType("WAV", "WAV Files", "wav", null, AudioCodec.WAV);
        public static readonly AudioType PCM = new AudioType("DTS", "DTS Files", "dts", null, AudioCodec.PCM);
        public static readonly AudioType CBRMP3 = new AudioType("CBR MP3", "CBR MP3 Files", "mp3", null, AudioCodec.MP3);
        public static readonly AudioType VBRMP3 = new AudioType("VBR MP3", "VBR MP3 Files", "mp3", null, AudioCodec.MP3);
    }
    public class SubtitleType : OutputType
    {
        public SubtitleType(string name, string filterName, string extension, ContainerType containerType)
            : base(name, filterName, extension, containerType) { }
        public static readonly SubtitleType SUBRIP = new SubtitleType("Subrip", "Subrip Subtitle Files", "srt", null);
        public static readonly SubtitleType VOBSUB = new SubtitleType("Vobsub", "Vobsub Subtitle Files", "idx", null);
    }
    public class ChapterType : OutputType
    {
        public ChapterType(string name, string filterName, string extension, ContainerType containerType)
            : base(name, filterName, extension, containerType) { }
        public static readonly ChapterType OGG_TXT = new ChapterType("Ogg chapter", "Ogg chapter files", "txt", null);
    }
    public class ContainerType : OutputFileType
    {
        public ContainerType(string name, string filterName, string extension)
            : base(name, filterName, extension) { }
        public static readonly ContainerType MP4 = new ContainerType("MP4", "MP4 Files", "mp4");
        public static readonly ContainerType MKV = new ContainerType("MKV", "Matroska Files", "mkv");
        public static readonly ContainerType AVI = new ContainerType("AVI", "AVI Files", "avi");
        public static readonly ContainerType[] Containers = new ContainerType[] { MP4, MKV, AVI };

        public static ContainerType ByName(string id)
        {
            foreach (ContainerType t in Containers)
                if (t.ID == id)
                    return t;
            return null;
        }
    }
    #endregion
    public class ContainerManager
    {
        public static GenericRegisterer<VideoType> VideoTypes = new GenericRegisterer<VideoType>();
        public static GenericRegisterer<AudioType> AudioTypes = new GenericRegisterer<AudioType>();
        public static GenericRegisterer<SubtitleType> SubtitleTypes = new GenericRegisterer<SubtitleType>();
        public static GenericRegisterer<ContainerType> ContainerTypes = new GenericRegisterer<ContainerType>();
	    public static GenericRegisterer<ChapterType> ChapterTypes = new GenericRegisterer<ChapterType>();

        static ContainerManager()
        {
            if (!(
                VideoTypes.Register(VideoType.AVI) &&
                VideoTypes.Register(VideoType.MKV) &&
                VideoTypes.Register(VideoType.MP4) &&
                VideoTypes.Register(VideoType.RAWASP) &&
                VideoTypes.Register(VideoType.RAWAVC)))
                throw new Exception("Failed to register a video type");
            if (!(
                AudioTypes.Register(AudioType.AC3) &&
                AudioTypes.Register(AudioType.MP3) &&
                AudioTypes.Register(AudioType.DTS) &&
                AudioTypes.Register(AudioType.MP2) &&
                AudioTypes.Register(AudioType.MP4AAC) &&
                AudioTypes.Register(AudioType.M4A) &&
                AudioTypes.Register(AudioType.RAWAAC) &&
                AudioTypes.Register(AudioType.VORBIS)))
                throw new Exception("Failed to register an audio type");
            if (!(
                SubtitleTypes.Register(SubtitleType.SUBRIP) &&
                SubtitleTypes.Register(SubtitleType.VOBSUB)))
                throw new Exception("Failed to register a subtitle type");
            if (!(
                ContainerTypes.Register(ContainerType.AVI) &&
                ContainerTypes.Register(ContainerType.MKV) &&
                ContainerTypes.Register(ContainerType.MP4)))
                throw new Exception("Failed to register a container type");
            if (!(
	            ChapterTypes.Register(ChapterType.OGG_TXT)))
		        throw new Exception("Failed to register a chapter type");
        }
    }

    public class OutputFileType : IIDable
    {
        public OutputFileType(string name, string filterName, string extension)
        {
            this.name = name;
            this.filterName = filterName;
            this.extension = extension;
        }

        public string ID
        {
            get { return name; }
        }

        private string name, filterName, extension;
        /// <summary>
        /// used to display the output type in dropdowns
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name;
        }
        public string OutputFilter
        {
            get { return "*." + extension; }
        }
        /// <summary>
        /// gets a valid filter string for file dialogs based on the known extension
        /// </summary>
        /// <returns></returns>
        public string OutputFilterString
        {
            get {return filterName + " (*." + extension + ")|*." + extension;}
        }
        /// <summary>
        /// gets the extension for this file type
        /// </summary>
        public string Extension
        {
            get {return this.extension;}
        }
    }

    public class OutputType : OutputFileType
    {
        public OutputType(string name, string filterName, string extension, ContainerType containerType)
            : base(name, filterName, extension)
        {
            this.containerType = containerType;
        }

        private ContainerType containerType;
        public ContainerType ContainerType
        {
            get { return this.containerType; }
        }
    }
}

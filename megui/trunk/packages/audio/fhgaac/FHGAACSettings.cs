using System;

namespace MeGUI
{
    public enum FhGAACProfile
    {
        [EnumTitle("AUTO")] AUTO,
        [EnumTitle("LC-AAC")] LC,
        [EnumTitle("HE-AAC")] HE,
        [EnumTitle("HE-AAC v2")] HE2
    }

    public enum FhGAAMode
    {
        [EnumTitle("CBR")] CBR,
        [EnumTitle("VBR")] VBR
    }

    public class FHGAACSettings : AudioCodecSettings
    {
        public static string ID = "Winamp-FhG AAC";

        public static readonly object[] SupportedBitrates =
            {32, 48, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 448, 512, 576, 640};

        public static readonly FhGAACProfile[] SupportedProfiles =
            {FhGAACProfile.AUTO, FhGAACProfile.LC, FhGAACProfile.HE, FhGAACProfile.HE2};

        public static readonly FhGAAMode[] SupportedModes =
            {FhGAAMode.CBR, FhGAAMode.VBR};

        public FHGAACSettings() : base(ID, AudioCodec.AAC, AudioEncoderType.FHGAAC, 128)
        {
            supportedBitrates = Array.ConvertAll(SupportedBitrates, o => (int) o);
            Mode = FhGAAMode.CBR;
            Profile = FhGAACProfile.AUTO;
            Quality = 3;
        }

        public FhGAAMode Mode { get; set; }

        public FhGAACProfile Profile { get; set; }

        public int Quality { get; set; }
    }
}
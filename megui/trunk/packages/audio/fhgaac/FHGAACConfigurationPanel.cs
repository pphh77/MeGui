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
using MeGUI.core.details.audio;

namespace MeGUI.packages.audio.fhgaac
{
    public partial class FHGAACConfigurationPanel : AudioConfigurationPanel, Editable<FHGAACSettings>
    {
        public FHGAACConfigurationPanel()
        {
            InitializeComponent();
            cbMode.Items.AddRange(EnumProxy.CreateArray(FHGAACSettings.SupportedModes));
            cbProfile.Items.AddRange(EnumProxy.CreateArray(FHGAACSettings.SupportedProfiles));
        }

        #region Editable<FHGAACSettings> Members

        FHGAACSettings Editable<FHGAACSettings>.Settings
        {
            get => (FHGAACSettings) Settings;
            set => Settings = value;
        }

        #endregion

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((FhGAAMode) (cbMode.SelectedItem as EnumProxy).RealValue)
            {
                case FhGAAMode.VBR:
                    trackBar.Visible = true;
                    label3.Visible = false;
                    cbProfile.Visible = false;
                    trackBar.Minimum = 1;
                    trackBar.Maximum = 6;
                    trackBar.TickFrequency = 1;
                    trackBar.Value = quality;
                    break;
                case FhGAAMode.CBR:
                    trackBar.Visible = true;
                    label3.Visible = true;
                    cbProfile.Visible = true;
                    trackBar.Minimum = 0;
                    trackBar.Maximum = 320;
                    trackBar.TickFrequency = 20;
                    trackBar.Value = bitrate;
                    break;
            }
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            switch ((FhGAAMode) (cbMode.SelectedItem as EnumProxy).RealValue)
            {
                case FhGAAMode.VBR:
                    encoderGroupBox.Text = string.Format(" Winamp-FhG AAC Options - Variable Bitrate @ Quality = {0} ",
                        trackBar.Value);
                    quality = trackBar.Value;
                    break;
                case FhGAAMode.CBR:
                    encoderGroupBox.Text = string.Format(" Winamp-FhG AAC Options - Constant Bitrate  @ {0} kbit/s ",
                        trackBar.Value);
                    bitrate = trackBar.Value;
                    break;
            }
        }

        #region properties

        private int bitrate;
        private int quality;

        /// <summary>
        ///     gets / sets the settings that are being shown in this configuration dialog
        /// </summary>
        protected override AudioCodecSettings CodecSettings
        {
            get
            {
                var nas = new FHGAACSettings();
                switch ((FhGAAMode) (cbMode.SelectedItem as EnumProxy).RealValue)
                {
                    case FhGAAMode.VBR:
                        nas.BitrateMode = BitrateManagementMode.VBR;
                        break;
                    case FhGAAMode.CBR:
                        nas.BitrateMode = BitrateManagementMode.CBR;
                        break;
                    default:
                        nas.BitrateMode = BitrateManagementMode.CBR;
                        break;
                }

                nas.Mode = (FhGAAMode) (cbMode.SelectedItem as EnumProxy).RealValue;
                nas.Profile = (FhGAACProfile) (cbProfile.SelectedItem as EnumProxy).RealValue;
                if (nas.Mode == FhGAAMode.CBR)
                    nas.Bitrate = trackBar.Value;
                else
                    nas.Quality = trackBar.Value;
                return nas;
            }
            set
            {
                var nas = value as FHGAACSettings;
                bitrate = nas.Bitrate;
                quality = nas.Quality;
                cbMode.SelectedItem = EnumProxy.Create(nas.Mode);
                cbProfile.SelectedItem = EnumProxy.Create(nas.Profile);
                if (nas.Mode == FhGAAMode.CBR)
                    trackBar.Value = Math.Max(Math.Min(nas.Bitrate, trackBar.Maximum), trackBar.Minimum);
                else
                    trackBar.Value = Math.Max(Math.Min(nas.Quality, trackBar.Maximum), trackBar.Minimum);
            }
        }

        #endregion
    }
}
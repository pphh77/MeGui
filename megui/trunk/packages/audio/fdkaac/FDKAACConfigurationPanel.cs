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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MeGUI.packages.audio.fdkaac
{
    public partial class FDKAACConfigurationPanel : MeGUI.core.details.audio.AudioConfigurationPanel, Editable<FDKAACSettings>
    {
        public FDKAACConfigurationPanel()
            : base()
        {
            InitializeComponent();
            cbMode.Items.AddRange(EnumProxy.CreateArray(FDKAACSettings.SupportedModes));
            cbProfile.Items.AddRange(EnumProxy.CreateArray(FDKAACSettings.SupportedProfiles));
        }

        #region properties
        /// <summary>
        /// gets / sets the settings that are being shown in this configuration dialog
        /// </summary>
        protected override AudioCodecSettings CodecSettings
        {
            get
            {
                FDKAACSettings nas = new FDKAACSettings();
                switch ((FdkAACMode)(cbMode.SelectedItem as EnumProxy).RealValue)
                {
                    case FdkAACMode.VBR: nas.BitrateMode = BitrateManagementMode.VBR; break;
                    case FdkAACMode.CBR: nas.BitrateMode = BitrateManagementMode.CBR; break;
                    default: nas.BitrateMode = BitrateManagementMode.CBR; break;
                }
                nas.Mode = (FdkAACMode)(cbMode.SelectedItem as EnumProxy).RealValue;
                nas.Profile = (FdkAACProfile)(cbProfile.SelectedItem as EnumProxy).RealValue;
                nas.Bitrate = (int)trackBar.Value;
                return nas;
            }
            set
            {
                FDKAACSettings nas = value as FDKAACSettings;
                cbMode.SelectedItem = EnumProxy.Create(nas.Mode);
                cbProfile.SelectedItem = EnumProxy.Create(nas.Profile);
                trackBar.Value = Math.Max(Math.Min(nas.Bitrate, trackBar.Maximum), trackBar.Minimum);
            }
        }
        #endregion

        #region Editable<FDKAACSettings> Members

        FDKAACSettings Editable<FDKAACSettings>.Settings
        {
            get
            {
                return (FDKAACSettings)Settings;
            }
            set
            {
                Settings = value;
            }
        }

        #endregion

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((FdkAACMode)(cbMode.SelectedItem as EnumProxy).RealValue)
            {
                case FdkAACMode.VBR:
                    trackBar.Visible = true;
                    label4.Visible = false;
                    trackBar.Minimum = 0;
                    trackBar.Maximum = 320;
                    trackBar.TickFrequency = 20;
                    break;
                case FdkAACMode.CBR:
                    trackBar.Visible = true;
                    label4.Visible = false;
                    trackBar.Minimum = 0;
                    trackBar.Maximum = 320;
                    trackBar.TickFrequency = 20;
                    break;
            }
            trackBar_ValueChanged(sender, e);
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            switch ((FdkAACMode)(cbMode.SelectedItem as EnumProxy).RealValue)
            {
                case FdkAACMode.VBR:
                    encoderGroupBox.Text = String.Format(" FDK-AAC Options - Variable Bitrate @ {0} kbit/s ", trackBar.Value);
                    break;
                case FdkAACMode.CBR:
                    encoderGroupBox.Text = String.Format(" FDK-AAC Options - Constant Bitrate  @ {0} kbit/s ", trackBar.Value);
                    break;
            }
        }
    }
}





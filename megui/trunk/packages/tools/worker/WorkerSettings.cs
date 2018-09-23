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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MeGUI
{
    public class WorkerSettings
    {
        private List<JobTyp> _arrJobTypes;
        private int _iMaxCount;

        public enum JobTyp
        {
            [EnumTitle("Audio Job")]
            Audio,
            [EnumTitle("DemuxerJob")]
            Demuxer,
            [EnumTitle("Indexer Job")]
            Indexer,
            [EnumTitle("Muxer Job")]
            Muxer,
            [EnumTitle("OneClick Job")]
            OneClick,
            [EnumTitle("Video Job")]
            Video
        };

        public WorkerSettings() : this (0, new List<JobTyp>())
        {

        }

        public WorkerSettings(int maxCount, List<JobTyp> arrJobTypes)
        {
            _iMaxCount = maxCount;
            _arrJobTypes = arrJobTypes;
        }

        public List<JobTyp> JobTypes
        {
            get { return _arrJobTypes; }
            set { _arrJobTypes = value;  }
        }

        public int MaxCount
        {
            get { return _iMaxCount; }
            set { _iMaxCount = value; }
        }

        public bool IsBlockedJob(Job oJob)
        {
            foreach (JobTyp oType in _arrJobTypes)
            {
                switch (oType)
                {
                    case JobTyp.Audio:
                    {
                        if (oJob is AudioJob)
                            return true;
                        break;
                    }
                    case JobTyp.Demuxer: 
                    {
                        if (oJob is SubtitleIndexJob || oJob is HDStreamsExJob || oJob is PgcDemuxJob
                            || oJob is MkvExtractJob || oJob is MeGUI.packages.tools.besplitter.AudioSplitJob)
                            return true;
                        break;
                    }
                    case JobTyp.Indexer:
                    {
                        if (oJob is IndexJob)
                            return true;
                        break;
                    }
                    case JobTyp.Muxer:
                    {
                        if (oJob is MuxJob || oJob is MeGUI.packages.tools.besplitter.AudioJoinJob  || oJob is MP4FpsModJob)
                            return true;
                        break;
                    }
                    case JobTyp.OneClick:
                        {
                            if (oJob is OneClickPostProcessingJob)
                                return true;
                            break;
                        }
                    case JobTyp.Video:
                    {
                        if (oJob is VideoJob || oJob is AviSynthJob)
                            return true;
                        break;
                    }
                }
            }

            return false;
        }

        public bool HasSameJobTypeList(WorkerSettings p)
        {
            var firstNotSecond = _arrJobTypes.Except(p.JobTypes).ToList();
            var secondNotFirst = p.JobTypes.Except(_arrJobTypes).ToList();
            return firstNotSecond.Count == 0 && secondNotFirst.Count == 0;
        }

#region equals override
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            WorkerSettings p = obj as WorkerSettings;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_iMaxCount == p.MaxCount) && HasSameJobTypeList(p);
        }

        public bool Equals(WorkerSettings p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_iMaxCount == p.MaxCount) && HasSameJobTypeList(p);
        }

        public override int GetHashCode()
        {
            return _iMaxCount ^ _arrJobTypes.GetHashCode();
        }

        public static bool operator ==(WorkerSettings a, WorkerSettings b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (a.MaxCount == b.MaxCount) && a.HasSameJobTypeList(b);
        }

        public static bool operator !=(WorkerSettings a, WorkerSettings b)
        {
            return !(a == b);
        }
    }
    #endregion

}
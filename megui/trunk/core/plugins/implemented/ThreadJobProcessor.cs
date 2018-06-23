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
using System.Diagnostics;
using System.IO;
using System.Threading;

using MeGUI.core.util;

namespace MeGUI
{
    public abstract class ThreadJobProcessor<TJob> : IJobProcessor
        where TJob : Job
    {
        #region variables
        protected TJob job;
        protected StatusUpdate su;
        protected LogItem log;
        protected Thread _processingThread;
        #endregion

        protected virtual void checkJobIO()
        {
            // only check if the input file exist if it is not a cleanup job
            if (!(this is MeGUI.core.details.CleanupJobRunner))
                Util.ensureExists(job.Input);
        }

        protected virtual void doExitConfig()
        {
            if (su.HasError || su.WasAborted)
                return;

            if (String.IsNullOrEmpty(job.Output))
                return;

            if (File.Exists(job.Output))
            {
                MediaInfoFile oInfo = new MediaInfoFile(job.Output, ref log);
            }
        }

        protected virtual void RunInThread()
        {
            
        }

        protected void ThreadFinished()
        {
            doExitConfig();
            su.IsComplete = true;
            StatusUpdate(su);
        }

        #region IVideoEncoder overridden Members

        public void setup(Job job2, StatusUpdate su, LogItem log)
        {
            Debug.Assert(job2 is TJob, "Job is the wrong type");

            this.log = log;
            TJob job = (TJob)job2;
            this.job = job;

            this.su = su;
            checkJobIO();
        }

        public void start()
        {
            try
            {
                _processingThread = new Thread(new ThreadStart(RunInThread));
                if (MainForm.Instance.Settings.ProcessingPriority == ProcessPriority.HIGH)
                    _processingThread.Priority = ThreadPriority.Highest;
                else if (MainForm.Instance.Settings.ProcessingPriority == ProcessPriority.ABOVE_NORMAL)
                    _processingThread.Priority = ThreadPriority.AboveNormal;
                else if (MainForm.Instance.Settings.ProcessingPriority == ProcessPriority.NORMAL)
                    _processingThread.Priority = ThreadPriority.Normal;
                else if (MainForm.Instance.Settings.ProcessingPriority == ProcessPriority.BELOW_NORMAL)
                    _processingThread.Priority = ThreadPriority.BelowNormal;
                else
                    _processingThread.Priority = ThreadPriority.Lowest;
                _processingThread.Start();
                new System.Windows.Forms.MethodInvoker(this.RunStatusCycle).BeginInvoke(null, null);
            }
            catch (Exception e)
            {
                throw new JobRunException(e);
            }
        }

        public void stop()
        {
            try
            {
                if (IsRunning())
                    _processingThread.Abort();
                doExitConfig();
                return;
            }
            catch (Exception e)
            {
                throw new JobRunException(e);
            }
        }

        public bool pause()
        {
            return false;
        }

        public bool resume()
        {
            return false;
        }

        public bool IsRunning()
        {
            return (this._processingThread != null && _processingThread.IsAlive);
        }

        public void changePriority(ProcessPriority priority)
        {
            if (!IsRunning())
                return;

            try
            {
                switch (priority)
                {
                    case ProcessPriority.IDLE:
                        _processingThread.Priority = ThreadPriority.Lowest;
                        break;
                    case ProcessPriority.BELOW_NORMAL:
                        _processingThread.Priority = ThreadPriority.BelowNormal;
                        break;
                    case ProcessPriority.NORMAL:
                        _processingThread.Priority = ThreadPriority.Normal;
                        break;
                    case ProcessPriority.ABOVE_NORMAL:
                        _processingThread.Priority = ThreadPriority.AboveNormal;
                        break;
                    case ProcessPriority.HIGH:
                        _processingThread.Priority = ThreadPriority.Highest;
                        break;
                }
                MainForm.Instance.Settings.ProcessingPriority = priority;
                return;
            }
            catch (Exception e) // process could not be running anymore
            {
                throw new JobRunException(e);
            }
        }

        #endregion
 
        #region status updates
        public event JobProcessingStatusUpdateCallback StatusUpdate;
        protected void RunStatusCycle()
        {
            while (IsRunning())
            {
                su.CurrentFileSize = FileSize.Of2(job.Output);
                su.FillValues();
                if (StatusUpdate != null && IsRunning())
                    StatusUpdate(su);
                MeGUI.core.util.Util.Wait(1000);
            }
            ThreadFinished();
        }
        #endregion
    }
}
﻿using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiPlayerNIIES.View.DSPlayer
{
    internal partial class DxPlay :IDisposable
    {
        public TimeSpan CurTime
        {
            get
            {
                if (m_mediaSeeking == null) return TimeSpan.Zero;
                long t = 0;
                int hr;
                hr = m_mediaSeeking.GetCurrentPosition(out t);
                DsError.ThrowExceptionForHR(hr);
                return TimeSpan.FromSeconds((double)t / 10_000_000);
            }
            set
            {
                if (m_mediaSeeking == null) return;
                long t = (long)(value.TotalSeconds * 10_000_000);
                long d = (long)(Duration.TotalSeconds * 10_000_000);
                int hr;
                hr = m_mediaSeeking.SetPositions(t, DirectShowLib.AMSeekingSeekingFlags.AbsolutePositioning, d, DirectShowLib.AMSeekingSeekingFlags.NoPositioning);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (m_mediaSeeking == null) return TimeSpan.Zero;
                long t = 0;
                int hr;
                hr = m_mediaSeeking.GetDuration(out t);
                DsError.ThrowExceptionForHR(hr);
                return TimeSpan.FromSeconds((double)t / 10_000_000);
            }
        }

        public double Position
        {
            get
            {
                if (m_mediaSeeking == null) return 0;
                return 1000 * CurTime.TotalMilliseconds / Duration.TotalMilliseconds;
            }
            set
            {
                if (m_mediaSeeking == null) return;
                CurTime = TimeSpan.FromSeconds(Duration.TotalSeconds * value / 1000);  
            }
        }

        public double Volume
        {
            get
            {
                if (m_basicAudio == null) return 0;
                int p,hr;
                hr = m_basicAudio.get_Volume(out p);
                double vol = Math.Pow(Math.E, ((double)p / 2325.5));
                DsError.ThrowExceptionForHR(hr);
                return vol;
            }
            set
            {
                if (m_basicAudio == null) return;
                    int hr;
                double db = 2325.5 * Math.Log(value); if (db <= -10000) db = -9999; if (db > 0) db = 0;
                hr = m_basicAudio.put_Volume((int)Math.Round(db));
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public double Rate
        {
            get
            {
                if (m_mediaSeeking == null) return 0;
                double r=0;
                int hr = m_mediaSeeking.GetRate(out r);
                DsError.ThrowExceptionForHR(hr);
                return r;
            }
            set
            {
                if (m_mediaSeeking == null) return;
                int hr = m_mediaSeeking.SetRate(value);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return m_State == GraphState.Running;
            }
        }

        public bool IsPaused
        {
            get
            {
                return m_State == GraphState.Paused;
            }
        }

        public bool IsStopped
        {
            get
            {
                return m_State == GraphState.Stopped;
            }
        }

        
        public  Control videoPanel
        {
            get; private set;
        }

        

    }
}
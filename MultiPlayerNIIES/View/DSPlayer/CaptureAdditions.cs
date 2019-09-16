using DirectShowLib;
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
                int res;
                res = m_mediaSeeking.GetCurrentPosition(out t);
                return TimeSpan.FromSeconds((double)t / 10_000_000);
            }
            set
            {
                if (m_mediaSeeking == null) return;
                long t = (long)(value.TotalSeconds * 10_000_000);
                long d = (long)(Duration.TotalSeconds * 10_000_000);
                m_mediaSeeking.SetPositions(t, DirectShowLib.AMSeekingSeekingFlags.AbsolutePositioning, d, DirectShowLib.AMSeekingSeekingFlags.NoPositioning);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (m_mediaSeeking == null) return TimeSpan.Zero;
                long t = 0;
                int res;
                res = m_mediaSeeking.GetDuration(out t);
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
                int p;
                m_basicAudio.get_Volume(out p);
                return p;
            }
            set
            { }
        }

        public bool IsPlaying
        {
            get
            {
                return m_State == GraphState.Running;
            }
        }

        public  Control videoPanel
        {
            get; private set;
        }

        internal void LoadMedia(Uri source)
        {
            this.LoadMedia(source.LocalPath);
        }

        public DxPlay(Control videoPanel)
        {
            this.videoPanel = videoPanel;
        }

        public void LoadMedia(string FileName)
        {
            try
            {
                int hr;
                IntPtr hEvent;

                // Save off the file name
                m_sFileName = FileName;

                // Set up the graph
                SetupGraph(videoPanel, FileName);

                // Get the event handle the graph will use to signal
                // when events occur
                hr = m_mediaEvent.GetEventHandle(out hEvent);
                DsError.ThrowExceptionForHR(hr);

                // Wrap the graph event with a ManualResetEvent
                m_mre = new ManualResetEvent(false);
#if USING_NET11
                m_mre.Handle = hEvent;
#else
                m_mre.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(hEvent, true);
#endif

                // Create a new thread to wait for events
                Thread t = new Thread(new ThreadStart(this.EventWait));
                t.Name = "Media Event Thread";
                t.Start();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

    }
}

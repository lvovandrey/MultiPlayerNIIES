using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                if (m_mediaSeeking == null) return TimeSpan.Zero;// && IsOnDeleting) return TimeSpan.FromSeconds(10);
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
                if (t < 0 || d < 0 || t > d) return;
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
                return CurTime.TotalMilliseconds / Duration.TotalMilliseconds;
            }
            set
            {
                if (m_mediaSeeking == null) return;
                CurTime = TimeSpan.FromSeconds(Duration.TotalSeconds * value);  
            }
        }

        public double Volume
        {
            get
            {
                if (m_basicAudio == null) return 0;
                int p,hr;
                hr = m_basicAudio.get_Volume(out p);
                double vol = Math.Pow(Math.E, ((double)(p+10709) / 2325.5)); 
                DsError.ThrowExceptionForHR(hr);
                return vol;
            }
            set
            {
                if (m_basicAudio == null) return;
                    int hr;
                double db = 2325.5 * Math.Log(value) - 10709;
                Debug.WriteLine("value=" + value.ToString() + "  db=" + db.ToString());
                if (db <= -10000) db = -9999; if (db > 0) db = 0;
                hr = m_basicAudio.put_Volume((int)Math.Round(db));
                DsError.ThrowExceptionForHR(hr);
            }
        }


        //public void SetVol(double value)
        //{
        
        //plVolume:= (65535 * value * 5) div 100;
        ////íîðìèðóåì õàðàêòåðèñòèêó óðîâíÿ ãðîìêîñòè
        //db:= trunc(33.22 * 100 * ln((plVolume + 1e-6) / 65535) / ln(10));
        //    if db < -10000 then db:= -9999;
        //    pBasicAudio.put_Volume(db);
        //}
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
              //  DsError.ThrowExceptionForHR(hr); 
              //TODO: снять отсюда комментарий
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


        public bool IsOnDeleting = false;


        public  Control videoPanel
        {
            get; private set;
        }

        internal bool TryRate()
        {
            int hr = m_mediaSeeking.SetRate(0.5);
            return hr == 0;
        }
    }
}

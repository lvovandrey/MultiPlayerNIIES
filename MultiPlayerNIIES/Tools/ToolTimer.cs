using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.Tools
{
    public delegate void VoidDelegate();

    public static class ToolsTimer
    {
        public static void Delay(VoidDelegate Complete, TimeSpan Interval)
        {
            System.Windows.Threading.DispatcherTimer ToolTimer = new System.Windows.Threading.DispatcherTimer();

            if (ToolTimer == null)
            {
                ToolTimer = new System.Windows.Threading.DispatcherTimer();
            }
            ToolTimer.Tick += (s, _) =>
            {
                Complete();
                ToolTimer.Stop();
                ToolTimer = null;
            };
            ToolTimer.Interval = Interval;
            ToolTimer.Start();
        }

        public static void Timer(VoidDelegate Tick, TimeSpan Interval)
        {
            System.Windows.Threading.DispatcherTimer ToolTimer = new System.Windows.Threading.DispatcherTimer();

            if (ToolTimer == null)
            {
                ToolTimer = new System.Windows.Threading.DispatcherTimer();
            }
            ToolTimer.Tick += (s, _) =>
            {
                Tick();
            };
            ToolTimer.Interval = Interval;
            ToolTimer.Start();
        }
    }
}

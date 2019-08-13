using MultiPlayerNIIES.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{

    public class PlayerPanelVM : INPCBase
    {
        private double position { get; set; }
        [Magic]
        public double Position { get { return position; } set { position = value; } }

        public int volume { get; set; }
        public int Volume { get; set; }

        public int curTime { get; set; }
        public int CurTime { get; set; }





    }
}

using DSPlayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DxPlay[] dxPlays;
        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            dxPlays = new DxPlay[6];
            //dxPlays[0]  = new DxPlay(panel1, @"C:\tmp\test1.avi", false);
            //dxPlays[1] = new DxPlay(panel2, @"C:\tmp\test1.avi", false);
            //dxPlays[2] = new DxPlay(panel3, @"C:\tmp\test1.avi", false);
            //dxPlays[3] = new DxPlay(panel4, @"C:\tmp\test1.avi", false);
            //dxPlays[4] = new DxPlay(panel5, @"C:\tmp\test4.avi", false);
            //dxPlays[5] = new DxPlay(panel6, @"C:\tmp\test6.avi", false);
        }

        private void ButtonPlay_Click(object sender, EventArgs e)
        {
            foreach (DxPlay p in dxPlays)
                p.Start();
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            foreach (DxPlay p in dxPlays)




                        


                p.Pause();
        }

        private void ButtonX2_Click(object sender, EventArgs e)
        {
            //foreach (DxPlay p in dxPlays)
            //p.Rate*=2;

            VideoContainer1.Size = new Size(200, 200);
            VideoContainer1.Location = new Point(0, 0);
            VideoContainer1.Show();
        }
    }
}

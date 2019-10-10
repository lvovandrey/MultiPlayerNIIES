using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsVideoControl
{
    public partial class VideoContainer1: UserControl
    {
        public VideoContainer1()
        {
            InitializeComponent();
            this.VideoPanel.MouseWheel += VideoPanel_MouseWheel;
        }

        private void VideoPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            double k = e.Delta > 0 ? 0.1 : -0.1;
            Zoom(k, this.VideoPanel, e.Location);
        }


        public void Zoom(double ZoomKoef, System.Windows.Forms.Control ZoomedElement, System.Drawing.Point ZoomCenterPositionInContainer)
        {
            
            Debug.WriteLine(" Loc =" + ZoomedElement.Location.ToString() + " Size =" + ZoomedElement.Size.ToString() + " Cursor=" + ZoomCenterPositionInContainer.ToString());

            double w = (double)ZoomedElement.Width;
            double h = (double)ZoomedElement.Height;


            double deltaX = ZoomKoef * w;
            double deltaY = ZoomKoef * h;

            double curX = (double)ZoomCenterPositionInContainer.X;
            double curY = (double)ZoomCenterPositionInContainer.Y;

            double ML = -(double)ZoomedElement.Location.X;//-ZoomedElement.Margin.Left;
            double MT = -(double)ZoomedElement.Location.Y;//-ZoomedElement.Margin.Top;

            double wnew = (double)w + (double)deltaX;
            double hnew = (double)h + (double)deltaY;

            double a = curX;
            double b = w - curX;
            double tau = a / b;
            double MLnew = -(tau / (1 + tau)) * deltaX - ML;

            double c = curY;
            double d = h - curY;
            double kappa = c / d;
            double MTnew = -(kappa / (1 + kappa)) * deltaY - MT;


            if (MLnew > 0) MLnew = 0;
            if (wnew < this.Width) wnew = this.Width;
            if (MTnew > 0) MTnew = 0;
            if (hnew < this.Height) hnew = this.Height;

            //   ZoomedElement.Dock = System.Windows.Forms.DockStyle.None;


            Debug.WriteLine(ZoomedElement.Location.ToString() + "  " + ZoomedElement.Size.ToString());

            ZoomedElement.Width = (int)wnew;
            ZoomedElement.Height = (int)hnew;
            //ZoomedElement.Margin = new System.Windows.Forms.Padding((int)MLnew, (int)MTnew, ZoomedElement.Margin.Right, ZoomedElement.Margin.Bottom);
            ZoomedElement.Location = new System.Drawing.Point((int)MLnew, (int)MTnew);

      

            //   Rect r = new Rect((int)MLnew, (int)MTnew, ZoomedElement.Width, ZoomedElement.Height); // TODO: преобразовать в пиксели - winforms пиксели понимает....
            //  dxPlay.SetWindowPosition(r);
            Debug.WriteLine(ZoomedElement.Location.ToString() + "  " + ZoomedElement.Size.ToString());

        }



    }
}

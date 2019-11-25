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
using System.Windows;
using System.Threading;

namespace WindowsFormsVideoControl
{
    public partial class VideoContainer1 : UserControl
    {
        public double CurZoomKoefX
        {
            get
            {
                return this.SelectablePictureBox1.Width / this.Width;
            }
        }
        public double CurZoomKoefY
        {
            get
            {
                return this.SelectablePictureBox1.Height / this.Height;
            }
        }


        public VideoContainer1()
        {
            InitializeComponent();

            DragDropSwitchOn(this, SelectablePictureBox1);
        }

        double oldW = 0;
        double oldH = 0;
        private void VideoContainer1_Load(object sender, EventArgs e)
        {
            this.SelectablePictureBox1.Width = this.Width;
            this.SelectablePictureBox1.Height = this.Height;
            this.SelectablePictureBox1.MouseWheel += MouseWheelHandler;
            oldW = Width;
            oldH = Height;
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            double k = e.Delta >= 0 ? 0.1 : -0.1;
            System.Drawing.Point curWinPos = this.PointToScreen(new System.Drawing.Point(0, 0));
            System.Drawing.Point Pos = new System.Drawing.Point(-curWinPos.X + Cursor.Position.X, -curWinPos.Y + Cursor.Position.Y);
            if (Pos.X < -2 || Pos.Y < -2 || Pos.X > this.Width + 2 || Pos.Y > this.Height + 2)
                return; // если зум за пределами окна - не делаем его
            Zoom(k, this.SelectablePictureBox1, e.Location);
        }


        #region Реализация Zoom
        public double OldZoomKoef = 1;
        public double CurZoomKoef = 1;

        public void Zoom(double ZoomKoef, System.Windows.Forms.Control ZoomedElement, System.Drawing.Point ZoomCenterPositionInContainer)
        {

            double kAspectRatio = (OriginalSize.Height / OriginalSize.Width);


            double w = (double)ZoomedElement.Width;
            double h = (double)ZoomedElement.Height;

            OldZoomKoef = (double)ZoomedElement.Width / (double)Width; Debug.WriteLine(OldZoomKoef.ToString());

            double deltaX = ZoomKoef * w;
            double deltaY = ZoomKoef * h;

            double curX = (double)ZoomCenterPositionInContainer.X;
            double curY = (double)ZoomCenterPositionInContainer.Y;

            double ML = -(double)ZoomedElement.Location.X;
            double MT = -(double)ZoomedElement.Location.Y;

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

            Думаю надо  тут учесть если оригинальное соотношение сторон, то...... над чет по другому делать (т.к. щас у нас мотаешь в минус - до ширины доходит минимальной а до высоты нет...)
            if (MLnew > 0) MLnew = 0;
            if (wnew < this.Width) wnew = this.Width;
            if (MTnew > 0) MTnew = 0;
            if (hnew < this.Height) hnew = this.Height;

            ZoomedElement.Width = (int)wnew;
            ZoomedElement.Height = (int)hnew;
            ZoomedElement.Location = new System.Drawing.Point((int)MLnew, (int)MTnew);

            if (AspectRatio == AspectRatio.Original)
                    ZoomedElement.Height = (int)(ZoomedElement.Width * kAspectRatio);
        }

        public void FitToFill()
        {
            Zoom(-1, SelectablePictureBox1, new System.Drawing.Point(0, 0));

            oldW = Width;
            oldH = Height;
        }

        #endregion

        #region Реализация Drag'n'Drop

        public Control Container;


        public bool IsDragDrop { get; private set; }

        System.Drawing.Point relativeMousePos;
        Control draggedObject;

        public void DragDropSwitchOn(Control container, Control DraggedElement)
        {
            if (IsDragDrop) return;
            Container = container;
            IsDragDrop = true;
            draggedObject = DraggedElement;
            draggedObject.MouseDown += StartDrag;
        }



        public void DragDropSwitchOff()
        {
            if (!IsDragDrop) return;
            Container = null;
            IsDragDrop = false;
            draggedObject.MouseDown -= StartDrag;
            draggedObject = null;
        }

        void StartDrag(object sender, MouseEventArgs e)
        {
            if ((Container == null) || !IsDragDrop) return;
            relativeMousePos = e.Location;
            draggedObject.MouseMove += OnDragMove;
            draggedObject.Leave += OnLostCapture;
            draggedObject.MouseUp += OnMouseUp;
        }

        void OnDragMove(object sender, MouseEventArgs e)
        {
            UpdatePosition(e);
        }
        int i;
        bool flag;
        void UpdatePosition(MouseEventArgs e)
        {
            var posFromForm = Cursor.Position;
            var point = this.PointToClient(posFromForm);
            var newPos = new System.Drawing.Point(point.X - relativeMousePos.X, point.Y - relativeMousePos.Y);
            draggedObject.Location = new System.Drawing.Point(newPos.X, newPos.Y);
        }



        void OnMouseUp(object sender, MouseEventArgs e)
        {
            FinishDrag(sender, e);

        }

        void OnLostCapture(object sender, EventArgs e)
        {
            FinishDrag(sender, null);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {

            draggedObject.MouseMove -= OnDragMove;
            draggedObject.Leave -= OnLostCapture;
            draggedObject.MouseUp -= OnMouseUp;

        }

        internal void OnResize()
        {
            //Тут ничего нет и не должно быть
        }






        #endregion

        private void VideoContainer1_Resize(object sender, EventArgs e)
        {
            if (oldH > 0 && oldW > 0)
            {
                if (AspectRatio == AspectRatio.BindToContainer)
                {
                    double kx = (double)Width / (double)oldW;
                    double ky = (double)Height / (double)oldH;

                    double wa = (double)SelectablePictureBox1.Width;
                    double wxa = (double)SelectablePictureBox1.Location.X;

                    double ha = (double)SelectablePictureBox1.Height;
                    double hya = (double)SelectablePictureBox1.Location.Y;

                    SelectablePictureBox1.Width = (int)Math.Round(wa * kx);
                    SelectablePictureBox1.Height = (int)Math.Round(ha * ky);
                    SelectablePictureBox1.Location = new System.Drawing.Point((int)Math.Round(wxa * kx), (int)Math.Round(hya * ky));
                }
                if (AspectRatio == AspectRatio.Original)
                {

                    double kx = (double)Width / (double)oldW;
                    double ky = (double)Height / (double)oldH;
                    if (Height > SelectablePictureBox1.Height) ky = kx;
                    else if (Width >= SelectablePictureBox1.Width) kx = ky;
                    else ky = kx;

                    double wa = (double)SelectablePictureBox1.Width;
                    double wxa = (double)SelectablePictureBox1.Location.X;

                    double ha = (double)SelectablePictureBox1.Height;
                    double hya = (double)SelectablePictureBox1.Location.Y;

                    SelectablePictureBox1.Width = (int)Math.Round(wa * kx);
                    SelectablePictureBox1.Height = (int)Math.Round(ha * ky);

                    double k = (OriginalSize.Height / OriginalSize.Width);
                    if (SelectablePictureBox1.Location.X == 0 && SelectablePictureBox1.Location.Y == 0)
                    {
                        if (!(Height >= SelectablePictureBox1.Height && Width >= SelectablePictureBox1.Width))
                        {
                            if (Width < SelectablePictureBox1.Width)
                            {
                                SelectablePictureBox1.Width = Width;
                                SelectablePictureBox1.Height = (int)(Width * k);
                            }
                            if (Height < SelectablePictureBox1.Height)
                            {
                                SelectablePictureBox1.Height = Height + 1;
                                SelectablePictureBox1.Width = (int)(Height / k);
                            }

                        }
                        if (Height >= SelectablePictureBox1.Height && Width >= SelectablePictureBox1.Width)
                        {
                            if (k > 1)
                            {
                                SelectablePictureBox1.Height = Height;
                                SelectablePictureBox1.Width = (int)(Height / k);
                            }
                            else
                            {
                                SelectablePictureBox1.Width = Width;
                                SelectablePictureBox1.Height = (int)(Width * k);
                            }

                        }

                    }
                    SelectablePictureBox1.Location = new System.Drawing.Point((int)Math.Round(wxa * kx), (int)Math.Round(hya * ky));
                }
            }
            oldW = Width;
            oldH = Height;
        }

        public void SetZoom(Rect zoomedArea)
        {
            SelectablePictureBox1.Width = (int)Math.Round(zoomedArea.Width);
            SelectablePictureBox1.Height = (int)Math.Round(zoomedArea.Height);
            SelectablePictureBox1.Location = new System.Drawing.Point((int)Math.Round(zoomedArea.Left), (int)Math.Round(zoomedArea.Top));
        }

        public Rect GetZoomedArea()
        {
            return new Rect(SelectablePictureBox1.Location.X, SelectablePictureBox1.Location.Y, SelectablePictureBox1.Width, SelectablePictureBox1.Height);
        }

        /// <summary>
        /// Соотношение сторон
        /// </summary>
        public AspectRatio AspectRatio = AspectRatio.BindToContainer;

        /// <summary>
        /// Исходный размер видео
        /// </summary>
        public System.Windows.Size OriginalSize = new System.Windows.Size(200, 100);

    }

}

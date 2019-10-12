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
    public partial class VideoContainer1: UserControl
    {
        public VideoContainer1()
        {
            InitializeComponent();

            DragDropSwitchOn(this, SelectablePictureBox1);
        }

        private void VideoContainer1_Load(object sender, EventArgs e)
        {
            this.SelectablePictureBox1.Width = this.Width;
            this.SelectablePictureBox1.Height = this.Height;
            this.SelectablePictureBox1.MouseWheel += MouseWheelHandler;
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            double k = e.Delta >= 0 ? 0.1 : -0.1;
            Zoom(k, this.SelectablePictureBox1, e.Location);
        }


        #region Реализация Zoom
        public void Zoom(double ZoomKoef, System.Windows.Forms.Control ZoomedElement, System.Drawing.Point ZoomCenterPositionInContainer)
        {
            
            double w = (double)ZoomedElement.Width;
            double h = (double)ZoomedElement.Height;


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


            if (MLnew > 0) MLnew = 0;
            if (wnew < this.Width) wnew = this.Width;
            if (MTnew > 0) MTnew = 0;
            if (hnew < this.Height) hnew = this.Height;

            ZoomedElement.Width = (int)wnew;
            ZoomedElement.Height = (int)hnew;
            ZoomedElement.Location = new System.Drawing.Point((int)MLnew, (int)MTnew);



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
            Debug.WriteLine("StartDrag");
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

            Debug.WriteLine("UpdatePosition"+ i++ + "  Loc= " + draggedObject.Location.ToString() + "   point" + point.ToString() + "   relativeMousePos" + relativeMousePos.ToString());
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


    }
}

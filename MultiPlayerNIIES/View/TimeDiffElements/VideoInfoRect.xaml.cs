using MultiPlayerNIIES.ViewModel;
using MultiPlayerNIIES.ViewModel.TimeDiffVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View.TimeDiffElements
{

    /// <summary>
    /// Логика взаимодействия для VideoInfoRect.xaml
    /// </summary>
    public partial class VideoInfoRect : UserControl
    {

        public VideoInfoRect(UIElement Container, double ColumnsWidth)
        {
            InitializeComponent();
            DragDropSwitchOn(Container, this);
            ColWidth = ColumnsWidth;
        }

        private double ColWidth = 300;

        private TimeDiffVideoInfoRectVM vm
        {
            get
            {
                return DataContext as TimeDiffVideoInfoRectVM;
            }
        }

        #region Дополнения к Drag'n'Drop
        public void AfterFinish(double MarginLeft)
        {
            if (vm == null) return;
            int pos = (int)Math.Floor(MarginLeft / ColWidth);
            vm.CurrentPosition = pos;
            Console.WriteLine("pos=" + pos + "CurPos=" +vm.CurrentPosition);
            Margin = new Thickness(vm.CurrentPosition * ColWidth + 20, Margin.Top, 0, 0);
        }

      

        internal void OnSizeContaierChanged()
        {
            if (vm == null) return;
            Margin = new Thickness(ColWidth*vm.CurrentPosition + 20, Margin.Top, 0, 0);
        }
        #endregion

        #region Реализация Drag'n'Drop

        public UIElement Container;

        public bool IsDragDrop { get; private set; }

        Vector relativeMousePos;
        FrameworkElement draggedObject;
        UIElement DraggerArea;



        public void DragDropSwitchOn(UIElement container, UIElement dragger)
        {
            if (IsDragDrop) return;
            Container = container;
            DraggerArea = dragger;
            IsDragDrop = true;
            MouseLeftButtonDown += StartDrag;
        }

        public void DragDropSwitchOff(UIElement container)
        {
            if (!IsDragDrop) return;
            Container = null;
            IsDragDrop = false;
            MouseLeftButtonDown -= StartDrag;



        }

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            if (!DraggerArea.IsMouseOver) return;
            if ((Container == null) || !IsDragDrop) return;
            draggedObject = (FrameworkElement)sender;
            relativeMousePos = e.GetPosition(draggedObject) - new Point();
            draggedObject.MouseMove += OnDragMove;
            draggedObject.LostMouseCapture += OnLostCapture;
            draggedObject.MouseUp += OnMouseUp;
            Mouse.Capture(draggedObject);

            Shadow.Visibility = Visibility.Visible;
        }



        void OnDragMove(object sender, MouseEventArgs e)
        {
            UpdatePosition(e);
        }

        void UpdatePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(Container);
            var newPos = point - relativeMousePos;
            //draggedObject.Margin = new Thickness(newPos.X, newPos.Y, 0, 0);
            //Перетаскиваем только по горизонтали
            draggedObject.Margin = new Thickness(newPos.X, draggedObject.Margin.Top, 0, 0);
        }

        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FinishDrag(sender, e);
            Mouse.Capture(null);
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            FinishDrag(sender, e);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {
            draggedObject.MouseMove -= OnDragMove;
            draggedObject.LostMouseCapture -= OnLostCapture;
            draggedObject.MouseUp -= OnMouseUp;
            UpdatePosition(e);

            AfterFinish(e.GetPosition(Container).X);
            Shadow.Visibility = Visibility.Hidden;
        }

        #endregion

        private void ImageSnapShot2_MouseEnter(object sender, MouseEventArgs e)
        {
            //ImageSnapShotBig.Source = ((Image)sender).Source;
            //ImageSnapShotBig.Visibility = Visibility.Visible;
        }

        private void ImageSnapShot2_MouseLeave(object sender, MouseEventArgs e)
        {
            //ImageSnapShotBig.Source = ((Image)sender).Source;
            //ImageSnapShotBig.Visibility = Visibility.Collapsed;
        }
    }
}

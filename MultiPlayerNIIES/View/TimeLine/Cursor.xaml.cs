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
using System.ComponentModel;
using System.Globalization;

namespace MultiPlayerNIIES.View.TimeLine
{
    /// <summary>
    /// Логика взаимодействия для Cursor.xaml
    /// </summary>
    public partial class Cursor : UserControl, INotifyPropertyChanged
    {
      
        public Cursor()
        {
            InitializeComponent();
            DataContext = this;
            MouseLeftButtonDown += StartDrag;

            CursorColor = new SolidColorBrush(Color.FromArgb(255, 7, 0, 71));
            OnCRPositionChanged += Cursor_OnCRPositionChanged;
        }

        private void Cursor_OnCRPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double newPos = Container.ActualWidth * CRPosition;
           
            if (newPos < 0) Margin = new Thickness(0, -5, 0, -5);
            else if (newPos > Container.ActualWidth) Margin = new Thickness(Container.ActualWidth, -5, 0, -5);
            else Margin = new Thickness(newPos, -5, 0, -5);

        }

        SolidColorBrush _CursorColor { get; set; }
        public SolidColorBrush CursorColor
        {
            get { return _CursorColor; }
            set
            {
                _CursorColor = value;
                OnPropertyChanged("CursorColor");
            }
        }




        #region DependencyProperties
        public static readonly DependencyProperty CRPositionProperty = DependencyProperty.Register("CRPosition",
         typeof(double), typeof(Cursor),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(CRPositionPropertyChangedCallback)));

        public double CRPosition
        {
            get { return (double)GetValue(CRPositionProperty); }
            set { SetValue(CRPositionProperty, value); }
        }


        public event PropertyChanged OnCRPositionChanged;
        public event Action OnCRPChanged;

        static void CRPositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Cursor)d).OnCRPositionChanged != null)
                ((Cursor)d).OnCRPositionChanged(d, e);
        }
        #endregion


        #region Реализация Drag'n'Drop

        public FrameworkElement Container;
        Vector relativeMousePos;
        FrameworkElement draggedObject;

        public event Action OnStartDrag;
        public event Action OnEndDrag;

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            OnStartDrag();
            draggedObject = (FrameworkElement)sender;
            relativeMousePos = e.GetPosition(draggedObject) - new Point();
            draggedObject.MouseMove += OnDragMove;
            draggedObject.LostMouseCapture += OnLostCapture;
            draggedObject.MouseUp += OnMouseUp;
            Mouse.Capture(draggedObject);
            CursorColor = new SolidColorBrush(Color.FromArgb(255, 83, 100, 255));
        }

        void OnDragMove(object sender, MouseEventArgs e)
        {
            UpdatePosition(e);
        }

        void UpdatePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(Container);
            var newPos = point - relativeMousePos;
            if (newPos.X < 0) draggedObject.Margin = new Thickness(0, -5, 0, -5);
            else if (newPos.X > Container.ActualWidth) draggedObject.Margin = new Thickness(Container.ActualWidth, -5, 0, -5);
            else draggedObject.Margin = new Thickness(newPos.X, -5, 0, -5);

           
         

            CRPosition = draggedObject.Margin.Left / Container.ActualWidth;
            OnCRPChanged();
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
            if (IsMouseOver) CursorColor = new SolidColorBrush(Color.FromArgb(255, 0, 50, 255));
            else CursorColor = new SolidColorBrush(Color.FromArgb(255, 7, 0, 71));
            OnEndDrag();
        }
        #endregion
        #region Реализация установки курсора в определенную точку
        public void SetPosition(double Position, MouseButtonEventArgs e)
        {
            OnStartDrag();
            draggedObject = this;
            var newPos = e.GetPosition(Container);
            if (newPos.X < 0) draggedObject.Margin = new Thickness(0, -5, 0, -5);
            else if (newPos.X > Container.ActualWidth) draggedObject.Margin = new Thickness(Container.ActualWidth, -5, 0, -5);
            else draggedObject.Margin = new Thickness(newPos.X, -5, 0, -5);


            Tools.ToolsTimer.Delay(() =>
            {
                CRPosition = draggedObject.Margin.Left / Container.ActualWidth;
                OnCRPChanged();
                OnEndDrag();
                StartDrag(this, e);
            }, TimeSpan.FromMilliseconds(10));

        }

        #endregion

        #region mvvm
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Cursor_MouseEnter(object sender, MouseEventArgs e)
        {
            CursorColor = new SolidColorBrush(Color.FromArgb(255, 0, 50, 255));
        }

        private void Cursor_MouseLeave(object sender, MouseEventArgs e)
        {
            CursorColor = new SolidColorBrush(Color.FromArgb(255, 7, 0, 71));
        }


    }

    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value + 12);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MultiPlayerNIIES.View.TimeLine
{
    /// <summary>
    /// Логика взаимодействия для Dash.xaml
    /// </summary>
    public partial class Dash : UserControl, INotifyPropertyChanged
    {

        public Dash()
        {
            InitializeComponent();

            DataContext = this;
        }





        public static readonly DependencyProperty TimeLabelVisibilityProperty = DependencyProperty.Register("TimeLabelVisibility",
        typeof(Visibility), typeof(Dash),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(TimeLabelVisibilityPropertyChangedCallback)));

        public Visibility TimeLabelVisibility
        {
            get
            {
                return (Visibility)GetValue(TimeLabelVisibilityProperty);
            }
            set
            {
                SetValue(TimeLabelVisibilityProperty, value);
                OnPropertyChanged("TimeLabelVisibility");
            }
        }

        public event PropertyChanged OnTimeLabelVisibilityChanged;

        static void TimeLabelVisibilityPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Dash)d).OnTimeLabelVisibilityChanged != null)
                ((Dash)d).OnTimeLabelVisibilityChanged(d, e);
        }


        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time",
        typeof(TimeSpan), typeof(Dash),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(TimePropertyChangedCallback)));

        public TimeSpan Time
        {
            get
            {
                return (TimeSpan)GetValue(TimeProperty);
            }
            set
            {
                SetValue(TimeProperty, value);
                OnPropertyChanged("Time");
            }
        }

        public event PropertyChanged OnTimeChanged;

        static void TimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Dash)d).OnTimeChanged != null)
                ((Dash)d).OnTimeChanged(d, e);
        }


        double lineWidth = 0.5;
        public double LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                if (value <= 0.001) return;
                lineWidth = value;
                OnPropertyChanged("LineWidth");
            }
        }

        double lineHeight = 5;
        public double LineHeight
        {
            get
            {
                return lineHeight;
            }
            set
            {
                if (value <= 0.001) return;
                lineHeight = value;
                OnPropertyChanged("LineHeight");
            }
        }

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}

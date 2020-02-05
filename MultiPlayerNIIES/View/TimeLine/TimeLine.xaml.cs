using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для TimeLine.xaml
    /// </summary>
    public partial class TimeLine : UserControl, INotifyPropertyChanged
    {
//        VideoPlayerVM VideoPlayerVM;
      
        public TimeLine()
        {
           

            InitializeComponent();
            //DataContext = this;

            FullTime = TimeSpan.FromSeconds(600);

            T1.T_full = FullTime;
            T1.T_el = TimeSpan.FromSeconds(60);
            T1.ChangeDashesHeight(12);
            T1.ChangeDashesWidth(1);
                        
            T_tenSec.T_full = FullTime;
            T_tenSec.T_el = TimeSpan.FromSeconds(10);
            T_tenSec.ChangeDashesHeight(6);

            Cursor1.Container = this;


            Binding binding = new Binding();

            binding.ElementName = "Cursor1"; // элемент-источник
            binding.Path = new PropertyPath("CRPosition"); // свойство элемента-источника
            binding.Mode = BindingMode.TwoWay;
            this.SetBinding(TimeLine.POSProperty, binding); // установка привязки для элемента-приемника

            OnPOSChanged += TimeLine_OnPOSChanged;
            Cursor1.OnCRPChanged += Cursor1_OnCRPChanged;
            Cursor1.OnStartDrag += Cursor1_OnStartDrag;
            Cursor1.OnEndDrag += Cursor1_OnEndDrag;

            SizeChanged += (d, e)=> { RefreshCusorPosition(); };

        }

        #region Перемещения курсора
        bool wasplayed = false;
        private void Cursor1_OnStartDrag()
        {
            VM vm = DataContext as VM; // да хуй с ним знаю что уродство
            if (vm == null) return; if (vm.SyncLeadPlayer == null) return;
            vm.SyncLeadPlayer.Body.VLC.TimeSlider_PreviewMouseDown(this, null);
        }
        private void Cursor1_OnEndDrag()
        {
            VM vm = DataContext as VM; // да хуй с ним знаю что уродство
            if (vm == null) return; if (vm.SyncLeadPlayer == null) return;
            vm.SyncLeadPlayer.Body.VLC.TimeSlider_PreviewMouseUp(this, null);
        }

        bool PosSelf = false;
        private void Cursor1_OnCRPChanged()
        {
            PosSelf = true;
            POS = Cursor1.CRPosition * 1000;
        }

        public void TimeLine_OnPOSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RefreshCusorPosition();
        }

        public void RefreshCusorPosition()
        {
            if (PosSelf) { PosSelf = false; return; }
            if (POS > -0.1) { Cursor1.CRPosition = POS / 1000;  Cursor1.RefreshPosition(); }
            
        }
        #endregion



        #region DependencyProperty POS - позиция на видео для биндинга к видеоплееру
        public static readonly DependencyProperty POSProperty = DependencyProperty.Register("POS",
         typeof(double), typeof(TimeLine),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(POSPropertyChangedCallback)));

        public double POS
        {
            get { return (double)GetValue(POSProperty); }
            set { SetValue(POSProperty, value); }
        }


        public event PropertyChanged OnPOSChanged;


        static void POSPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((TimeLine)d).OnPOSChanged != null)
                ((TimeLine)d).OnPOSChanged(d, e);
        }

        #endregion


        #region intervalsOperations

        //DependencyProperty FullTime  - чтобы можно было подписаться на него
        public TimeSpan FullTime
        {
            get { return (TimeSpan)GetValue(FullTimeProperty); }
            set
            {
                SetValue(FullTimeProperty, value);
                RefreshDashes();
                OnPropertyChanged("FullTime");
            }
        }

        public static readonly DependencyProperty FullTimeProperty =
            DependencyProperty.Register("FullTime", typeof(TimeSpan), typeof(TimeLine), new PropertyMetadata(TimeSpan.FromSeconds(10)));

        void RefreshDashes()
        {

            T1.T_full = FullTime;
            T_tenSec.T_full = FullTime;
            T_Sec.T_full = FullTime;
            T10.T_full = FullTime;

            T1.T_el = TimeSpan.FromSeconds(60);
            T_tenSec.T_el = TimeSpan.FromSeconds(10);
            T_Sec.T_el = TimeSpan.FromSeconds(1);
            T10.T_el = TimeSpan.FromSeconds(600);


            int N = (int)Math.Round((FullTime.TotalSeconds / T1.T_el.TotalSeconds)) + 2;
            T1.ClearDashes();
            T1.FillDashes(N);

            N = (int)Math.Round((FullTime.TotalSeconds / T_tenSec.T_el.TotalSeconds)) + 2;
            T_tenSec.ClearDashes();
            T_tenSec.FillDashes(N);

            if (FullTime.TotalSeconds < 60)
            {
                N = (int)Math.Round((FullTime.TotalSeconds / T_Sec.T_el.TotalSeconds)) + 2;
                T_Sec.ClearDashes();
                T_Sec.FillDashes(N);
            }

            N = (int)Math.Round((FullTime.TotalSeconds / T10.T_el.TotalSeconds)) + 2;
            T10.ClearDashes();
            T10.FillDashes(N);


            T1.ChangeDashesHeight(20);
            T1.ChangeDashesWidth(1);

            T_tenSec.ChangeDashesHeight(14);

            T_Sec.ChangeDashesHeight(10);

            T10.ChangeDashesHeight(26);
            T10.ChangeDashesWidth(2);

            T1.Visibility = Visibility.Visible;
            T_tenSec.Visibility = Visibility.Visible;
            T_Sec.Visibility = Visibility.Visible;
            T10.Visibility = Visibility.Visible;

            if (FullTime < TimeSpan.FromMinutes(0.5))
            {
                T1.TimeLabelVisibility = Visibility.Hidden;
                T_tenSec.TimeLabelVisibility = Visibility.Visible;
                T_Sec.TimeLabelVisibility = Visibility.Visible;
                T10.TimeLabelVisibility = Visibility.Hidden;
            }
            else if (FullTime < TimeSpan.FromMinutes(30))
            {
                T1.TimeLabelVisibility = Visibility.Visible;
                T_tenSec.TimeLabelVisibility = Visibility.Hidden;
                T_Sec.TimeLabelVisibility = Visibility.Hidden;
                T10.TimeLabelVisibility = Visibility.Visible;
                T_Sec.Visibility = Visibility.Hidden;
            }
            else if (FullTime >= TimeSpan.FromMinutes(30))
            {
                T1.TimeLabelVisibility = Visibility.Hidden;
                T_tenSec.TimeLabelVisibility = Visibility.Hidden;
                T_Sec.TimeLabelVisibility = Visibility.Hidden;
                T10.TimeLabelVisibility = Visibility.Visible;
                T_tenSec.Visibility = Visibility.Hidden;
                T_Sec.Visibility = Visibility.Hidden;
            }

            T_Sec.HideRepeatedDashes(T_tenSec.Dashes);
            T_tenSec.HideRepeatedDashes(T1.Dashes);
            T1.HideRepeatedDashes(T10.Dashes);

        }






        #endregion






        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Перемещаем курсор в точку клика на таймлайне
            Cursor1.SetPosition(0, e);

        }


        #region mvvm
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

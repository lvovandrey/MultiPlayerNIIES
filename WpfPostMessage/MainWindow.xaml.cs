using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfPostMessage
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();



        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  do stuff
            try
            {
                MessageProcessingAsync(hwnd, msg, wParam, lParam, handled);
            }
            catch(Exception e)
            {
                MessageBox.Show("Все такие серьезные, а у мен тут арифметическая ошиба вот такая: " + e.Message + "    StackTrace: " + e.StackTrace );
            }
            //        Thread.Sleep(50);
            return IntPtr.Zero;
        }

        void MessageProcessing(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool handled)
        {
            //  do stuff
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Txt.Text += msg.ToString() + "/" + lParam.ToInt64().ToString() + "/" + wParam.ToInt64().ToString() + " - ";
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Все такие серьезные, а у мен тут арифметическая ошиба вот такая: " + e.Message + "    StackTrace: " + e.StackTrace);
            }
            //        Thread.Sleep(50);
       //     return IntPtr.Zero;
        }
        async void MessageProcessingAsync(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool handled)
        {
            await Task.Run(()=>MessageProcessing(hwnd, msg, wParam, lParam, handled));
        }

        private void RunAsync()
        {
            string param = "Hi";
            Task.Run(() => MethodWithParameter(param));
        }

        private void MethodWithParameter(string param)
        {
            //Do stuff
        }

        Excel.Application ex = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            double w = SystemParameters.PrimaryScreenWidth;
            double h = SystemParameters.PrimaryScreenHeight;
            this.Txt.Text += w.ToString("#") +"х" + h.ToString("#")+"\n";
            //source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            //source.AddHook(new HwndSourceHook(WndProc));

            //ex = new Excel.Application();
            //ex.Visible = true;
            //Excel.Workbooks books = ex.Workbooks;
            //Excel._Workbook book = null;
            //book = books.Open(Environment.CurrentDirectory + @"\test.xlsm");

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ex.Run((object)"CurModeReceive", (object)"FuckMode");
        }
    }
}

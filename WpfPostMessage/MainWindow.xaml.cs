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
        HwndSource source;
        public MainWindow()
        {
            InitializeComponent();



        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  do stuff
            try
            {
                Txt.Text += msg.ToString() + "/" + lParam.ToInt32().ToString() + "/" + wParam.ToInt32().ToString() + " - ";
            }
            catch
            { }
            //        Thread.Sleep(50);
            return IntPtr.Zero;
        }
        Excel.Application ex;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));

            ex = new Excel.Application();
            ex.Visible = true;
            Excel.Workbooks books = ex.Workbooks;
            Excel._Workbook book = null;
            book = books.Open(Environment.CurrentDirectory + @"\test.xlsm");

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ex.Run((object)"CurModeReceive", (object)"FuckMode");
        }
    }
}

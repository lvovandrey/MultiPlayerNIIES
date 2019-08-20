using MultiPlayerNIIES.Tools;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для WaitProgressBar.xaml
    /// </summary>
    public partial class WaitProgressBar : UserControl
    {
        public WaitProgressBar()
        {
            InitializeComponent();
        }

        public void ShowMe(string text, TimeSpan interval)
        {
            TextBlockInfo.Text = text;
            Panel.SetZIndex(this, 1000);
            this.Visibility = Visibility.Visible;
            WaitProgressBar1.BeginAnimation(ProgressBar.ValueProperty, new DoubleAnimation(0, 100, interval));
            ToolsTimer.Delay(() => { this.Visibility = Visibility.Hidden; TextBlockInfo.Text = ""; WaitProgressBar1.Value = 0; }, interval);
        }
    }
}

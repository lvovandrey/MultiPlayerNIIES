using MultiPlayerNIIES.Tools;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для WaitProgressBarForTimeline.xaml
    /// </summary>
    public partial class WaitProgressBarForTimeline : UserControl
    {

        public List<WaitProgressBarForTimeline> AditionInstances = new List<WaitProgressBarForTimeline>();//костыль для дополнительных реализаций этого же элемента
        public WaitProgressBarForTimeline()
        {
            InitializeComponent();
        }

        public void ShowMe(string text, TimeSpan interval)
        {
            TextBlockInfo.Text = text;
            Panel.SetZIndex(this, 1000);
            this.Visibility = Visibility.Visible;
            ToolsTimer.Delay(() =>
            {
                DoubleAnimation animation = new DoubleAnimation(0, this.ActualWidth, interval);
                WaitProgressBar1.BeginAnimation(Rectangle.WidthProperty, animation);
                ToolsTimer.Delay(() => 
                {
                    WaitProgressBar1.BeginAnimation(Rectangle.WidthProperty, null);
                    TextBlockInfo.Text = "";
                    WaitProgressBar1.Width = 0;
                    this.Visibility = Visibility.Collapsed;
                }, interval+TimeSpan.FromSeconds(0.05));
            }, TimeSpan.FromSeconds(0.05));

            if (AditionInstances.Count>0) //вот так извращенно мы пробросим в другие такие же элементы это событие
                foreach (var item in AditionInstances)
                    item.ShowMe(text, interval);
        }
    }
}

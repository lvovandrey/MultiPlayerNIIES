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

namespace MultiPlayerNIIES.View.TimeStripes
{
    /// <summary>
    /// Логика взаимодействия для Stripe.xaml
    /// </summary>
    public partial class Stripe : UserControl
    {
        public Stripe()
        {
            InitializeComponent();
        }

        #region Реализация Focused
        public event Action UpFocus;

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpFocus();
        }
        #endregion
    }
}

using MultiPlayerNIIES.ViewModel.TimeStripeVM;
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
    /// Логика взаимодействия для StripesContainer.xaml
    /// </summary>
    public partial class StripesContainer : UserControl
    {
        public StripesContainer()
        {
            InitializeComponent();
        }

        public void FillStripes(IEnumerable<Stripe> stripes)
        {
            MainStack.Children.Clear();
            foreach (var s in stripes)
            {
                MainStack.Children.Add(s);
            }
        }

        private void MainStack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is StripeContainerVM)
            ((StripeContainerVM)this.DataContext).Refresh();
        }
    }
}

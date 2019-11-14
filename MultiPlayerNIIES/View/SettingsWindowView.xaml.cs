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
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindowView.xaml
    /// </summary>
    public partial class SettingsWindowView : Window
    {
        Dictionary<string, UserControl> SettingsElements;
        public SettingsWindowView()
        {
            InitializeComponent();
            SettingsElements = new Dictionary<string, UserControl>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ElementUsefulName = ((ListBoxItem)e.AddedItems[0]).Content.ToString();
            SettingsElementShow(ElementUsefulName);
        }

        private void SettingsElementShow(string ElementUsefulName)
        {
            foreach (var element in SettingsElements)
                element.Value.Visibility = Visibility.Collapsed;
            SettingsElements[ElementUsefulName].Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsElements.Add("Общее", CommonSettins);
            SettingsElements.Add("Вид", ViewSettings);
            SettingsElements.Add("Сохранение файлов", FilesOpenSettings);
            
        }
    }
}

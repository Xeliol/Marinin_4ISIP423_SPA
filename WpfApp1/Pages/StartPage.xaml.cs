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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();

            List<Users> masters = Core.Context.Users.Where(u => u.TypeID == 2).ToList();
            MasterListBox.ItemsSource = masters;

            List<string> SortItems = Core.Context.ServiceTypes.Select(t => t.Name).ToList();
            SortItems.Add("Пусто");
            SortBox.ItemsSource = SortItems;

            AccountButton.Content = NavigationData.CurrentData != null ? "Профиль" : "Войти";
        }

        List<String> serviceTypes = Core.Context.ServiceTypes.Select(st => st.Name).ToList();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {

                var srv = clickedButton.DataContext as Services;

                if (srv != null)
                {
                    NavigationService.Navigate(new AppointmentPage(srv));
                }
            }
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortBox.SelectedItem != null && SortBox.SelectedItem != "Пусто")
            {
                List<Users> newsource = Core.Context.Users.Where(u => u.TypeID == 2 && Core.Context.Services.Where(s => s.ServiceTypes.Name == SortBox.SelectedItem).Select(s => s.MasterID).Contains(u.UserID)).ToList();
                MasterListBox.ItemsSource = newsource;
            }else
            {
                MasterListBox.ItemsSource = Core.Context.Users.Where(u => u.TypeID == 2).ToList();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                MasterListBox.ItemsSource = Core.Context.Users.Where(u => u.TypeID == 2).ToList();
                return;
            }

            var filteredMasters = Core.Context.Users
                .Where(u => u.TypeID == 2 && u.Services.Any(s => s.Name.ToLower().Contains(searchText)))
                .ToList();

            MasterListBox.ItemsSource = filteredMasters;
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsPage());
        }

        private void MasterButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            ListBoxItem listBoxItem = (ListBoxItem)MasterListBox.ContainerFromElement(clickedButton);

            if (listBoxItem != null)
            {
                object dataItem = listBoxItem.Content;

                var mas = dataItem as Users;

                NavigationService.Navigate(new MasterPage(mas));
            }
        }
    }
}

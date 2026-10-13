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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            var currentUsers = Core.Context.Users.ToList();
            ListUser.ItemsSource = currentUsers;
        }

        private void clearFiltersButton_Click_1(object sender, RoutedEventArgs e)
        {
            fioFilterTextBox.Text = "";
            sortComboBox.SelectedIndex = 0;
            onlyAdminCheckBox.IsChecked = false;
        }

        private void fioFilterTextBox_TextChanged(object sender,TextChangedEventArgs e)
        {
            UpdateUsers();
        }

        private void sortComboBox_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            UpdateUsers();
        }
        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }
        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs
        e)
        {
            UpdateUsers();
        }
        private void UpdateUsers()
        {
            if (!IsInitialized)
            {
                return;
            }
            try
            {
                List<Users> currentUsers = Core.Context.Users.ToList();
                //Филтрация по фамилии
                if (!string.IsNullOrWhiteSpace(fioFilterTextBox.Text))
                {
                    currentUsers = currentUsers.Where(x =>
                    x.Name.ToLower().Contains(fioFilterTextBox.Text.ToLower())).ToList();
                }
                //Фильтрация по роли
                if (onlyAdminCheckBox.IsChecked.Value)
                {

                    currentUsers = currentUsers.Where(x => x.UserType.Name ==
                    "Админ").ToList();
                }
                //Фильтрация по убыванию/возрастанию
                ListUser.ItemsSource = (sortComboBox.SelectedIndex == 0) ?
                currentUsers.OrderBy(x => x.Name).ToList() : currentUsers.OrderByDescending(x =>
                x.Name).ToList();
            }
            catch (Exception)
            {
            }
        }
    }
}

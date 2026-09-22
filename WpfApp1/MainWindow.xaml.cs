using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfApp1.Pages;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavigationData.CurrentData = new CurData
            {
                ID = -1,
                Login = "Anon",
                Type = 0
            };

            //AccountButton.Content = "Log In";
            //AccountButton.Click += AccountButton_Click;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Page currentPage = MainFrame.Content as Page;
            if (MainFrame.NavigationService.CanGoBack) MainFrame.NavigationService.GoBack();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Page currentPage = MainFrame.Content as Page;
            if (AccountButton.Content == "Log In") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "My Account") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "My Profile") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "Manage") MainFrame.NavigationService.Navigate(new ManagerPage());
            else if (AccountButton.Content == "Admin") MainFrame.NavigationService.Navigate(new AdminPage());
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountButton.Content == "Log In") MainFrame.NavigationService.Navigate(new LoginPage());
            else if (AccountButton.Content == "My Account") MainFrame.NavigationService.Navigate(new AccountPage());
            else if (AccountButton.Content == "My Profile") MainFrame.NavigationService.Navigate(new MasterAccountPage());
            else if (AccountButton.Content == "Manage") MainFrame.NavigationService.Navigate(new ManagerPage());
            else if (AccountButton.Content == "Admin") MainFrame.NavigationService.Navigate(new AdminPage());
        }

        private void MasterAccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new MasterAccountPage());
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CurData user = NavigationData.CurrentData as CurData;

            switch (user.Type)
            {
                case 0:
                    AccountButton.Content = "Log In";
                    break;
                case 1:
                    AccountButton.Content = "My Account";
                    break;
                case 2:
                    AccountButton.Content = "My Profile";
                    break;
                case 3:
                    AccountButton.Content = "Manage";
                    break;
                case 4:
                    AccountButton.Content = "Admin";
                    break;
            }
        }
    }

    public static class NavigationData
    {
        public static object CurrentData
        {
            get; set;
        }
    }
}

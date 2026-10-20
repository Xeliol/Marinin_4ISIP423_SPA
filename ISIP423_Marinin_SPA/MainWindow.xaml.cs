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
using ISIP423_Marinin_SPA.Pages;

namespace ISIP423_Marinin_SPA
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
                Type = 0  };

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
            if (AccountButton.Content == "Войти") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "Аккаунт") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "Профиль") MainFrame.NavigationService.Navigate(new StartPage());
            else if (AccountButton.Content == "Управление") MainFrame.NavigationService.Navigate(new ManagerPage());
            else if (AccountButton.Content == "Админ") MainFrame.NavigationService.Navigate(new AdminPage());
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountButton.Content == "Войти") MainFrame.NavigationService.Navigate(new LoginPage());
            else if (AccountButton.Content == "Аккаунт") MainFrame.NavigationService.Navigate(new AccountPage());
            else if (AccountButton.Content == "Профиль") MainFrame.NavigationService.Navigate(new MasterAccountPage());
            else if (AccountButton.Content == "Управление") MainFrame.NavigationService.Navigate(new ManagerPage());
            else if (AccountButton.Content == "Админ") MainFrame.NavigationService.Navigate(new AdminPage());
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
                    AccountButton.Content = "Войти";
                    break;
                case 1:
                    AccountButton.Content = "Аккаунт";
                    break;
                case 2:
                    AccountButton.Content = "Профиль";
                    break;
                case 3:
                    AccountButton.Content = "Управление";
                    break;
                case 4:
                    AccountButton.Content = "Админ";
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => {
                DateTimeNow.Text = DateTime.Now.ToString();
            };
            timer.Start();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Message",
            MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
            else

                e.Cancel = false;
        }

        private void StyleButton_Click(object sender, RoutedEventArgs e)
        {
            if (StyleButton.Content == "☾")
            {
                StyleButton.Content = "☼";
                // определяем путь к файлу ресурсов
                var uri = new Uri("Dictionary.xaml", UriKind.Relative);
                // загружаем словарь ресурсов
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as
                ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            } else
            {
                StyleButton.Content = "☾";
                // определяем путь к файлу ресурсов
                var uri = new Uri("DictionaryDark.xaml", UriKind.Relative);
                // загружаем словарь ресурсов
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as
                ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
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

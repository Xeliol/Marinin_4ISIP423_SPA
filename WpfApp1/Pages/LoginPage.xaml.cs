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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool auth = Auth(LoginBox.Text, PassBox.Text);
            if (auth)
            {
                var log_user = Core.Context.Users.ToList().Where(p => p.Login == LoginBox.Text).Where(p => p.Password == PassBox.Text);
                var cur = NavigationData.CurrentData as CurData;

                cur.Login = log_user.First().Login;
                cur.Password = log_user.First().Password;
                cur.ID = log_user.First().UserID;
                cur.Type = log_user.First().TypeID;

                MessageBox.Show("Logged in!");
                if (cur.Type == 4) NavigationService.Navigate(new AdminPage());
                else if (cur.Type == 1 || cur.Type == 2) NavigationService.GoBack();
                else if (cur.Type == 3) NavigationService.Navigate(new ManagerPage());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        /// <summary>
        /// Этот метод проверяет можно ли авторизовать пользователя
        /// </summary>
        /// <param name="login">Введённый логин пользователя</param>
        /// <param name="password">Введённый пароль пользователя</param> 
        /// <returns>Можно ли авторизовать пользователя? (Ложь/Истина)</returns>
        public bool Auth(string login, string password)
        {
            var log_user = Core.Context.Users.ToList().Where(p => p.Login == login);
            if (log_user.Count() != 0)
            {
                log_user = Core.Context.Users.ToList().Where(p => p.Login == login).Where(p => p.Password == password);
                if (log_user.Count() != 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Wrong Password.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("No such user.");
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();

            foreach (var user in Core.Context.Users.ToList())
            {
                people.Add(user.Login);
            }
        }

        List<String> people = new List<String>();
        List<String> nums = new List<String> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

        private bool isPassWrong(String pass)
        {
            bool flag = true;
            if (PassBox1.Text.Length < 8) flag = false;
            else
            {
                flag = pass.Any(char.IsDigit);
                flag = !pass.All(char.IsDigit);
            }
            return !flag;
        }

        public static bool IsValidEmailRegex(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            try
            {
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            var signup = SignUp(LoginBox.Text, PassBox1.Text, PassBox2.Text, PhoneBox.Text, EmailBox.Text);
            if (signup)
            {
                Users new_user = new Users
                {
                    UserID = Core.Context.Users.ToList().Count() + 1,
                    Login = LoginBox.Text,
                    Password = PassBox1.Text,
                    Phone = PhoneBox.Text,
                    TypeID = 1,
                };
                if (IsValidEmailRegex(EmailBox.Text))
                {
                    new_user.Email = EmailBox.Text;
                }
                Core.Context.Users.Add(new_user);
                Core.Context.SaveChanges();

                var cur = NavigationData.CurrentData as CurData;

                cur.Login = new_user.Login;
                cur.Password = new_user.Password;
                cur.ID = new_user.UserID;
                cur.Type = new_user.TypeID;

                MessageBox.Show("Account created!");
                NavigationService.Navigate(new StartPage());
            }
        }

        /// <summary>
        /// Этот метод проверяет можно ли регистрировать пользователя
        /// </summary>
        /// <param name="login">Введённый логин пользователя</param>
        /// <param name="pass1">Введённый пароль пользователя</param> 
        /// <param name="pass2">Введённое подтверждение пароля пользователя</param> 
        /// <param name="phone">Введённый телефон пользователя</param> 
        /// <returns>Можно ли регистрировать пользователя? (Ложь/Истина)</returns>
        public bool SignUp(string login, string pass1, string pass2, string phone, string email = "")
        {
            if (pass1 != "" && pass2 != "" && login != "" && phone != "")
            {
                if (people.Contains(login))
                {
                    MessageBox.Show("Username is already in use.");
                    return false;
                }
                else
                {
                    if (pass1 != pass2)
                    {
                        MessageBox.Show("Passwords do not match.");
                        return false;
                    }
                    else
                    {
                        bool flag = true;
                        if (pass1.Length < 8) flag = false;
                        else
                        {
                            flag = pass1.Any(char.IsDigit);
                            flag = flag && !pass1.All(char.IsDigit);
                        }
                        if (!flag)
                        {
                            MessageBox.Show("Password does not follow the rules.");
                            return false;
                        }
                        else
                        {
                            if (Regex.IsMatch(phone, "[0-9]{7,}"))
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Phone number is invalid.");
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please fill every field without '*'.");
                return false;
            }
        }
    }
}
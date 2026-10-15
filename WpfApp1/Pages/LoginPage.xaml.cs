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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private int failedAttempts = 0;
        private Users currentUser;

        public LoginPage()
        {
            InitializeComponent();
        }

        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x =>
                x.ToString("X2")));
            }
        }

        public void CaptchaSwitch()
        {
            switch (captcha.Visibility)
            {
                case Visibility.Visible:
                    LoginBox.Clear();
                    PassBox.Clear();

                    captcha.Visibility = Visibility.Hidden;
                    captchaInput.Visibility = Visibility.Hidden;
                    labelCaptcha.Visibility = Visibility.Hidden;
                    submitCaptcha.Visibility = Visibility.Hidden;

                    labelLogin.Visibility = Visibility.Visible;
                    labelPass.Visibility = Visibility.Visible;
                    LoginBox.Visibility = Visibility.Visible;
                    PassBox.Visibility = Visibility.Visible;
                    noAccount.Visibility = Visibility.Visible;
                    forgotPass.Visibility = Visibility.Visible;

                    ButtonChangePassword.Visibility = Visibility.Visible;
                    LoginButton.Visibility = Visibility.Visible;
                    regButton.Visibility = Visibility.Visible;
                    return;
                case Visibility.Hidden:
                    captcha.Visibility = Visibility.Visible;
                    captchaInput.Visibility = Visibility.Visible;
                    labelCaptcha.Visibility = Visibility.Visible;
                    submitCaptcha.Visibility = Visibility.Visible;

                    labelLogin.Visibility = Visibility.Hidden;
                    labelPass.Visibility = Visibility.Hidden;
                    LoginBox.Visibility = Visibility.Hidden;
                    PassBox.Visibility = Visibility.Hidden;
                    noAccount.Visibility = Visibility.Hidden;
                    forgotPass.Visibility = Visibility.Hidden;

                    regButton.Visibility = Visibility.Hidden;
                    ButtonChangePassword.Visibility = Visibility.Hidden;
                    LoginButton.Visibility = Visibility.Hidden;
                    return;
            }
        }

        public void CaptchaChange()

        {

            String allowchar = " ";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            String[] ar = allowchar.Split(a);
            String pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            captcha.Text = pwd;
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool auth = Auth(LoginBox.Text, GetHash(PassBox.Password));
            if (auth)
            {
                var log_user = Core.Context.Users.ToList().Where(p => p.Login == LoginBox.Text).Where(p => p.Password == GetHash(PassBox.Password));
                var cur = NavigationData.CurrentData as CurData;

                cur.Login = log_user.First().Login;
                cur.Password = log_user.First().Password;
                cur.ID = log_user.First().UserID;
                cur.Type = log_user.First().TypeID;

                MessageBox.Show("Вход выполнен!");
                if (cur.Type == 4) NavigationService.Navigate(new AdminPage());
                else if (cur.Type == 1 || cur.Type == 2) NavigationService.GoBack();
                else if (cur.Type == 3) NavigationService.Navigate(new ManagerPage());
            }
            else
            {
                if (failedAttempts >= 3)
                {
                    if (captcha.Visibility != Visibility.Visible)
                    {
                        CaptchaSwitch();
                    }
                    CaptchaChange();
                }
                return;
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
                    MessageBox.Show("Неверный пароль.");
                    failedAttempts++;

                    return false;
                }
            }
            else
            {
                MessageBox.Show("Нет такого пользователя.");
                failedAttempts++;

                return false;
            }
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                CaptchaChange();

            }
            else
            {
                MessageBox.Show("Капча введена успешно, можете продолжить авторизацию", "Успех");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
            e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }
    }
}

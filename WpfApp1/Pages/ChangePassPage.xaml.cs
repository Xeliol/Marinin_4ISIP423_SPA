using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp1.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(TbLogin.Text))
            {
                MessageBox.Show("Все поля обязательны к заполнению!", "Предупреждение", MessageBoxButton.OK);
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новый пароль и его подтверждение не совпадают!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string hashedPass = LoginPage.GetHash(CurrentPasswordBox.Password);

            var user = Core.Context.Users
                .FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedPass);

            if (user == null)
            {
                MessageBox.Show("Текущий пароль/Логин неверный!", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string newPassword = NewPasswordBox.Password;
            bool number = false;
            bool letter = false;

            if (newPassword.Length >= 6)
            {
                foreach (char c in newPassword)
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) letter = true;
                    if (char.IsDigit(c)) number = true;
                }
            }
            if (letter && number)
            {
                user.Password = LoginPage.GetHash(NewPasswordBox.Password);
                Core.Context.SaveChanges();
                MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Новый пароль не соответствует требованиям безопасности!\n" +
                                "Пароль должен содержать минимум 6 символов: латинские буквы и цифры.",
                                "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}

using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AddUserPage : Page
    {
        private Users _currentUser = new Users();
        private bool _isNewUser = true;

        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();

            cmbRole.ItemsSource = Core.Context.UserType.ToList();

            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                _isNewUser = false;
                TBPass.Text = _currentUser.Password;
            }

            DataContext = _currentUser;
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация полей
            if (string.IsNullOrWhiteSpace(_currentUser.Login))
                errors.AppendLine("Укажите логин!");

            if (string.IsNullOrWhiteSpace(TBPass.Text))
                errors.AppendLine("Укажите пароль!");

            if (cmbRole.SelectedValue == null)
                errors.AppendLine("Выберите роль!");

            if (string.IsNullOrWhiteSpace(_currentUser.Name))
                errors.AppendLine("Укажите ФИО!");
            if (string.IsNullOrWhiteSpace(_currentUser.Phone))
            {
                errors.AppendLine("Укажите телефон!");
            }
            else if (_currentUser.Phone.Length > 20)
            {
                errors.AppendLine("Номер телефона слишком длинный (макс. 20 символов)!");
            }

            if (!string.IsNullOrWhiteSpace(_currentUser.Email) && _currentUser.Email.Length > 50)
            {
                errors.AppendLine("Email слишком длинный (макс. 50 символов)!");
            }


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK);
                return;
            }

            _currentUser.TypeID = (int)cmbRole.SelectedValue;
            _currentUser.ImagePath = string.IsNullOrWhiteSpace(TBPhoto.Text) ? "/images/default.jpg" : TBPhoto.Text;

            if (_isNewUser || TBPass.Text != _currentUser.Password)
            {
                _currentUser.Password = GetHash(TBPass.Text);
            }

            if (_isNewUser)
            {
                Core.Context.Users.Add(_currentUser);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentUser = new Users();
            DataContext = _currentUser;
            TBPass.Text = "";
            TBPhoto.Text = "";
            TBPhone.Text = "";
            TBEmail.Text = "";
            cmbRole.SelectedIndex = -1;
            _isNewUser = true;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddUserPage : Page
    {
        private Users _currentUser = new Users();
        private bool _isNewUser = true;
        private string _selectedImageSourcePath = null;

        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();
            cmbRole.ItemsSource = Core.Context.UserType.ToList();

            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                _isNewUser = false;
                TBPass.Text = _currentUser.Password;
                TbPhotoStatus.Text = string.IsNullOrWhiteSpace(_currentUser.ImagePath)
                    ? "Не выбрано" : System.IO.Path.GetFileName(_currentUser.ImagePath);
            }

            DataContext = _currentUser;
        }

        private void ButtonSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff",
                Title = "Выберите изображение"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                _selectedImageSourcePath = dialog.FileName;
                TbPhotoStatus.Text = System.IO.Path.GetFileName(_selectedImageSourcePath);
                PhotoPreview.Source = ImageHelper.CreateSquareBitmap(
                    ImageHelper.LoadBitmap(_selectedImageSourcePath), 250);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}");
                _selectedImageSourcePath = null;
                PhotoPreview.Source = null;
                TbPhotoStatus.Text = "Не выбрано";
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Text)) errors.AppendLine("Укажите пароль!");
            if (cmbRole.SelectedValue == null) errors.AppendLine("Выберите роль!");
            if (string.IsNullOrWhiteSpace(_currentUser.Name)) errors.AppendLine("Укажите ФИО!");
            if (string.IsNullOrWhiteSpace(_currentUser.Phone)) errors.AppendLine("Укажите телефон!");
            else if (_currentUser.Phone.Length > 20) errors.AppendLine("Телефон слишком длинный (макс. 20)!");
            if (!string.IsNullOrWhiteSpace(_currentUser.Email) && _currentUser.Email.Length > 50)
                errors.AppendLine("Email слишком длинный (макс. 50)!");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString(), "Ошибка"); return; }

            _currentUser.TypeID = (int)cmbRole.SelectedValue;

            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
            {
                try { _currentUser.ImagePath = ImageHelper.ProcessAndSaveImage(_selectedImageSourcePath, 250); }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}");
                    return;
                }
            }
            else if (_isNewUser && string.IsNullOrWhiteSpace(_currentUser.ImagePath))
            {
                _currentUser.ImagePath = "/images/default.jpg";
            }

            if (_isNewUser || TBPass.Text != _currentUser.Password)
                _currentUser.Password = GetHash(TBPass.Text);

            if (_isNewUser) Core.Context.Users.Add(_currentUser);

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД");
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentUser = new Users();
            DataContext = _currentUser;
            TBPass.Text = "";
            _selectedImageSourcePath = null;
            PhotoPreview.Source = null;
            TbPhotoStatus.Text = "Не выбрано";
            cmbRole.SelectedIndex = -1;
            _isNewUser = true;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                                        .Select(x => x.ToString("X2")));
        }
    }
}
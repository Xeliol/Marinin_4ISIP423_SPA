using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AccountPage : Page
    {
        private Users _currentUser;

        public AccountPage()
        {
            InitializeComponent();

            var curdata = NavigationData.CurrentData as CurData;
            _currentUser = Core.Context.Users.ToList()[curdata.ID - 1];

            this.DataContext = _currentUser;

            SessionListBox.ItemsSource = Core.Context.Appointments
                .Where(t => t.UserID == _currentUser.UserID).ToList();

            ProductListBox.ItemsSource = Core.Context.Orders
                .Where(o => o.UserID == _currentUser.UserID).ToList();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff",
                Title = "Выберите изображение"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                string newPath = ImageHelper.ProcessAndSaveImage(dialog.FileName, 250);
                _currentUser.ImagePath = newPath;
                Core.Context.SaveChanges();

                UserPhotoImage.Source = ImageHelper.LoadPreviewFromRelative(newPath);
                MessageBox.Show("Фото успешно обновлено!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении фото: {ex.Message}");
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationData.CurrentData = new CurData { ID = -1, Login = "Anon", Type = 0 };
            MessageBox.Show("Выход выполнен.");
            NavigationService.Navigate(new StartPage());
        }
    }
}
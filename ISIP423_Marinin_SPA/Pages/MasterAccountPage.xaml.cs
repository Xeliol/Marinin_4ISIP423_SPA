using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class MasterAccountPage : Page
    {
        private Users _currentUser;
        List<int> services = new List<int>();

        public MasterAccountPage()
        {
            InitializeComponent();

            var curdata = NavigationData.CurrentData as CurData;
            _currentUser = Core.Context.Users.ToList()[curdata.ID - 1];
            this.DataContext = _currentUser;

            services = _currentUser.Services.Select(s => s.ServiceID).ToList();
            SessionListBox.ItemsSource = Core.Context.Appointments
                .Where(a => services.Contains(a.Services.ServiceID) && a.UserID != null).ToList();
            ServiceListBox.ItemsSource = _currentUser.Services.ToList();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.jfif;*.png;*.bmp;*.gif;*.tif;*.tiff",
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

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Appointments app)
            {
                Core.Context.Appointments.Remove(app);
                Core.Context.SaveChanges();
                SessionListBox.ItemsSource = Core.Context.Appointments
                    .Where(a => services.Contains(a.Services.ServiceID) && a.UserID != null).ToList();
            }
        }

        private void ManageAppointmentsButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new AppointmentsTabPage());

        private void ManageServicesButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new ServicesTabPage());
    }
}
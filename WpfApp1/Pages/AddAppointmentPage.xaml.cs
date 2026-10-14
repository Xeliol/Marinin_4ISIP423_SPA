using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AddAppointmentPage : Page
    {
        private Appointments _currentAppointment = new Appointments();
        private bool _isNewAppointment = true;

        public AddAppointmentPage(Appointments selectedAppointment)
        {
            InitializeComponent();

            CurData user = NavigationData.CurrentData as CurData;

            if (selectedAppointment != null)
            {
                _currentAppointment = selectedAppointment;
                _isNewAppointment = false;

                DPDate.SelectedDate = _currentAppointment.Date;
                TBTime.Text = _currentAppointment.Date.ToString("HH:mm");
            }
            else
            {
                DPDate.SelectedDate = DateTime.Now;
                TBTime.Text = DateTime.Now.ToString("HH:mm");
            }

            if (user != null && user.Type == 2)
            {
                cmbUser.IsEnabled = false;

                if (_isNewAppointment)
                {
                    _currentAppointment.UserID = null;
                    _currentAppointment.Users = null;
                }

                cmbService.ItemsSource = Core.Context.Services.Where(x => x.MasterID == user.ID).ToList();
            }
            else
            {
                cmbUser.IsEnabled = true;

                var usersList = Core.Context.Users.ToList();

                var nullUser = new Users { Name = "(Без клиента / Свободное время)" };
                nullUser.UserID = -1;

                usersList.Insert(0, nullUser);
                cmbUser.ItemsSource = usersList;

                cmbService.ItemsSource = Core.Context.Services.ToList();
            }

            DataContext = _currentAppointment;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (cmbService.SelectedValue == null)
                errors.AppendLine("Выберите услугу!");

            if (DPDate.SelectedDate == null)
                errors.AppendLine("Укажите дату записи!");

            if (string.IsNullOrWhiteSpace(TBTime.Text))
            {
                errors.AppendLine("Укажите время записи!");
            }
            else if (!TimeSpan.TryParse(TBTime.Text, out TimeSpan parsedTime))
            {
                errors.AppendLine("Некорректный формат времени! Используйте ЧЧ:ММ (например, 15:30)");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                DateTime selectedDate = DPDate.SelectedDate.Value;
                TimeSpan selectedTime = TimeSpan.Parse(TBTime.Text);
                _currentAppointment.Date = selectedDate.Date + selectedTime;

                if (cmbUser.SelectedValue != null && (int)cmbUser.SelectedValue == -1)
                {
                    _currentAppointment.UserID = null;
                    _currentAppointment.Users = null;
                }

                if (_isNewAppointment)
                {
                    Core.Context.Appointments.Add(_currentAppointment);
                }

                Core.Context.SaveChanges();
                MessageBox.Show("Запись успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentAppointment = new Appointments();

            CurData user = NavigationData.CurrentData as CurData;
            if (user != null && user.Type == 2)
            {
                _currentAppointment.UserID = null;
                _currentAppointment.Users = null;
            }

            DataContext = null;
            DataContext = _currentAppointment;

            cmbUser.SelectedIndex = -1;
            cmbService.SelectedIndex = -1;
            DPDate.SelectedDate = DateTime.Now;
            TBTime.Text = DateTime.Now.ToString("HH:mm");
            _isNewAppointment = true;
        }
    }
}

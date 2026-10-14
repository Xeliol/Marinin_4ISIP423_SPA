using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AppointmentsTabPage : Page
    {
        public AppointmentsTabPage()
        {
            InitializeComponent();
            UpdateData();
        }

        private void UpdateData()
        {
            Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());

            CurData user = NavigationData.CurrentData as CurData;

            if (user != null && user.Type == 2)
            {
                DataGridAppointments.ItemsSource = Core.Context.Appointments
                    .Where(x => x.Services.MasterID == user.ID)
                    .ToList();
            }
            else
            {
                DataGridAppointments.ItemsSource = Core.Context.Appointments.ToList();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                UpdateData();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddAppointmentPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var appointmentsForRemoving = DataGridAppointments.SelectedItems.Cast<Appointments>().ToList();

            if (!appointmentsForRemoving.Any())
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {appointmentsForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Appointments.RemoveRange(appointmentsForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Appointments selectedAppointment)
            {
                NavigationService?.Navigate(new AddAppointmentPage(selectedAppointment));
            }
        }
    }
}

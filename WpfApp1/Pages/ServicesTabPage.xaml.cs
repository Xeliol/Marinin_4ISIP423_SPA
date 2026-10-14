using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class ServicesTabPage : Page
    {
        public ServicesTabPage()
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
                DataGridServices.ItemsSource = Core.Context.Services
                    .Where(x => x.MasterID == user.ID)
                    .ToList();
            }
            else // Админ (TypeID == 1) видит абсолютно всё
            {
                DataGridServices.ItemsSource = Core.Context.Services.ToList();
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
            NavigationService?.Navigate(new AddServicePage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var servicesForRemoving = DataGridServices.SelectedItems.Cast<Services>().ToList();

            if (!servicesForRemoving.Any())
            {
                MessageBox.Show("Выберите услуги для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить выбранные услуги ({servicesForRemoving.Count} шт.)?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Services.RemoveRange(servicesForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось удалить записи. Возможно, они используются в таблице Записей.\nОшибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Services selectedService)
            {
                NavigationService?.Navigate(new AddServicePage(selectedService));
            }
        }
    }
}

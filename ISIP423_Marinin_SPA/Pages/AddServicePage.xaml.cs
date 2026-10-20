using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddServicePage : Page
    {
        private Services _currentService = new Services();
        private bool _isNewService = true;
        CurData user = NavigationData.CurrentData as CurData;

        public AddServicePage(Services selectedService)
        {
            InitializeComponent();

            cmbType.ItemsSource = Core.Context.ServiceTypes.ToList();

            cmbMaster.ItemsSource = Core.Context.Users.Where(u => u.TypeID == 2).ToList();

            if (selectedService != null)
            {
                _currentService = selectedService;
                _isNewService = false;
            }

            DataContext = _currentService;

            if (user != null && user.Type == 2)
            {
                LBMaster.Visibility = Visibility.Collapsed;
                cmbMaster.Visibility = Visibility.Collapsed;

                if (_isNewService)
                {
                    _currentService.MasterID = user.ID; // Автопривязка
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Name))
                errors.AppendLine("Введите название услуги!");

            if (cmbType.SelectedValue == null)
                errors.AppendLine("Выберите категорию услуги!");

            // Админ обязан выбрать мастера, у мастера же поле скрыто и заполнено автоматически
            if (cmbMaster.Visibility == Visibility.Visible && cmbMaster.SelectedValue == null)
                errors.AppendLine("Выберите мастера для услуги!");

            if (_currentService.Price <= 0)
            {
                errors.AppendLine("Стоимость услуги должна быть больше нуля!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isNewService)
            {
                Core.Context.Services.Add(_currentService);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Услуга успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentService = new Services();

            if (user != null && user.Type == 2)
            {
                _currentService.MasterID = user.ID;
            }

            DataContext = null;
            DataContext = _currentService;

            cmbType.SelectedIndex = -1;
            cmbMaster.SelectedIndex = -1;
            _isNewService = true;
        }
    }
}

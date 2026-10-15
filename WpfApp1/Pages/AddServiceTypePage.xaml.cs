using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AddServiceTypePage : Page
    {
        private ServiceTypes _currentServiceType = new ServiceTypes();
        private bool _isNewType = true;

        public AddServiceTypePage(ServiceTypes selectedType)
        {
            InitializeComponent();

            if (selectedType != null)
            {
                _currentServiceType = selectedType;
                _isNewType = false;
            }

            DataContext = _currentServiceType;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentServiceType.Name))
            {
                errors.AppendLine("Укажите название типа услуги!");
            }
            else if (_currentServiceType.Name.Length > 100)
            {
                errors.AppendLine("Название слишком длинное (макс. 100 символов)!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isNewType)
            {
                Core.Context.ServiceTypes.Add(_currentServiceType);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentServiceType = new ServiceTypes();
            DataContext = _currentServiceType;
            _isNewType = true;
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddManufacturerPage : Page
    {
        private Manufacturers _currentManufacturer = new Manufacturers();
        private bool _isNewManufacturer = true;

        public AddManufacturerPage(Manufacturers selectedManufacturer)
        {
            InitializeComponent();

            if (selectedManufacturer != null)
            {
                _currentManufacturer = selectedManufacturer;
                _isNewManufacturer = false;
            }

            DataContext = _currentManufacturer;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentManufacturer.Name))
            {
                errors.AppendLine("Укажите наименование производителя!");
            }
            else if (_currentManufacturer.Name.Length > 100)
            {
                errors.AppendLine("Наименование слишком длинное (макс. 100 символов)!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isNewManufacturer)
            {
                Core.Context.Manufacturers.Add(_currentManufacturer);
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
            _currentManufacturer = new Manufacturers();
            DataContext = _currentManufacturer;
            _isNewManufacturer = true;
        }
    }
}

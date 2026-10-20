using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddProductTypePage : Page
    {
        private ProductTypes _currentProductType = new ProductTypes();
        private bool _isNewType = true;

        public AddProductTypePage(ProductTypes selectedType)
        {
            InitializeComponent();

            if (selectedType != null)
            {
                _currentProductType = selectedType;
                _isNewType = false;
            }

            DataContext = _currentProductType;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProductType.Name))
            {
                errors.AppendLine("Укажите название типа продукта!");
            }
            else if (_currentProductType.Name.Length > 100)
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
                Core.Context.ProductTypes.Add(_currentProductType);
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
            _currentProductType = new ProductTypes();
            DataContext = _currentProductType;
            _isNewType = true;
        }
    }
}

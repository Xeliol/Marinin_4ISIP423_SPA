using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class AddProductPage : Page
    {
        private Products _currentProduct = new Products();
        private bool _isNewProduct = true;

        public AddProductPage(Products selectedProduct)
        {
            InitializeComponent();

            cmbType.ItemsSource = Core.Context.ProductTypes.ToList();
            cmbManufacturer.ItemsSource = Core.Context.Manufacturers.ToList();

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                _isNewProduct = false;
                TBPhoto.Text = _currentProduct.ImagePath;
            }

            DataContext = _currentProduct;
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.Name))
                errors.AppendLine("Укажите название товара!");

            if (cmbType.SelectedValue == null)
                errors.AppendLine("Выберите тип товара!");

            if (cmbManufacturer.SelectedValue == null)
                errors.AppendLine("Выберите производителя!");

            if (!int.TryParse(TBPrice.Text, out int price) || price < 0)
            {
                errors.AppendLine("Цена должна быть положительным целым числом!");
            }

            if (!double.TryParse(TBSale.Text, out double sale) || sale < 0 || sale > 100)
            {
                errors.AppendLine("Скидка должна быть числом от 0 до 100!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentProduct.TypeID = (int)cmbType.SelectedValue;
            _currentProduct.ManufacturerID = (int)cmbManufacturer.SelectedValue;
            _currentProduct.ImagePath = string.IsNullOrWhiteSpace(TBPhoto.Text) ? "/images/ProductPlaceholder.png" : TBPhoto.Text;

            if (_isNewProduct)
            {
                Core.Context.Products.Add(_currentProduct);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные товара успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentProduct = new Products();
            DataContext = _currentProduct;

            TBPrice.Text = "0";
            TBSale.Text = "0";
            TBDescription.Text = "";
            TBPhoto.Text = "";
            cmbType.SelectedIndex = -1;
            cmbManufacturer.SelectedIndex = -1;
            _isNewProduct = true;
        }
    }
}

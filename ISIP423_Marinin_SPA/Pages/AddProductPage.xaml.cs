using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddProductPage : Page
    {
        private Products _currentProduct = new Products();
        private bool _isNewProduct = true;
        private string _selectedImageSourcePath = null;

        public AddProductPage(Products selectedProduct)
        {
            InitializeComponent();

            cmbType.ItemsSource = Core.Context.ProductTypes.ToList();
            cmbManufacturer.ItemsSource = Core.Context.Manufacturers.ToList();

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                _isNewProduct = false;
                TbPhotoStatus.Text = string.IsNullOrWhiteSpace(_currentProduct.ImagePath)
                    ? "Не выбрано" : System.IO.Path.GetFileName(_currentProduct.ImagePath);
            }

            DataContext = _currentProduct;
        }

        private void ButtonSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.jfif;*.png;*.bmp;*.gif;*.tif;*.tiff",
                Title = "Выберите изображение"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                _selectedImageSourcePath = dialog.FileName;
                TbPhotoStatus.Text = System.IO.Path.GetFileName(_selectedImageSourcePath);
                PhotoPreview.Source = ImageHelper.CreateSquareBitmap(
                    ImageHelper.LoadBitmap(_selectedImageSourcePath), 250);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}");
                _selectedImageSourcePath = null;
                PhotoPreview.Source = null;
                TbPhotoStatus.Text = "Не выбрано";
            }
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

            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
            {
                try
                {
                    _currentProduct.ImagePath = ImageHelper.ProcessAndSaveImage(_selectedImageSourcePath, 250);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}");
                    return;
                }
            }
            else if (_isNewProduct && string.IsNullOrWhiteSpace(_currentProduct.ImagePath))
            {
                _currentProduct.ImagePath = "images/ProductPlaceholder.png";
            }

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
            _selectedImageSourcePath = null;
            PhotoPreview.Source = null;
            TbPhotoStatus.Text = "Не выбрано";
            cmbType.SelectedIndex = -1;
            cmbManufacturer.SelectedIndex = -1;
            _isNewProduct = true;
        }
    }
}

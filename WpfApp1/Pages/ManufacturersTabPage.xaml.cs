using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class ManufacturersTabPage : Page
    {
        public ManufacturersTabPage()
        {
            InitializeComponent();
            DataGridManufacturers.ItemsSource = Core.Context.Manufacturers.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridManufacturers.ItemsSource = Core.Context.Manufacturers.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddManufacturerPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var manufacturersForRemoving = DataGridManufacturers.SelectedItems.Cast<Manufacturers>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {manufacturersForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Manufacturers.RemoveRange(manufacturersForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DataGridManufacturers.ItemsSource = Core.Context.Manufacturers.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении данных. Возможно, у этого производителя есть связанные товары.\n\nДетали: " + ex.Message);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturer = (sender as Button).DataContext as Manufacturers;
            NavigationService?.Navigate(new Pages.AddManufacturerPage(selectedManufacturer));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class ProductTypesTabPage : Page
    {
        public ProductTypesTabPage()
        {
            InitializeComponent();
            DataGridProductTypes.ItemsSource = Core.Context.ProductTypes.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridProductTypes.ItemsSource = Core.Context.ProductTypes.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddProductTypePage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var typesForRemoving = DataGridProductTypes.SelectedItems.Cast<ProductTypes>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {typesForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.ProductTypes.RemoveRange(typesForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    DataGridProductTypes.ItemsSource = Core.Context.ProductTypes.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении. Возможно, к этому типу уже привязаны существующие товары.\n\nДетали: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = (sender as Button).DataContext as ProductTypes;
            NavigationService?.Navigate(new AddProductTypePage(selectedType));
        }
    }
}

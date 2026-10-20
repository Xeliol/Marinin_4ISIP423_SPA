using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class OrdersTabPage : Page
    {
        public OrdersTabPage()
        {
            InitializeComponent();
            DataGridOrders.ItemsSource = Core.Context.Orders.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridOrders.ItemsSource = Core.Context.Orders.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddOrderPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var ordersForRemoving = DataGridOrders.SelectedItems.Cast<Orders>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить выбранные заказы в количестве {ordersForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Orders.RemoveRange(ordersForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Заказы успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    DataGridOrders.ItemsSource = Core.Context.Orders.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении. Возможно, к заказу привязаны товары в корзине (ProductsOrder).\n\nДетали: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = (sender as Button).DataContext as Orders;
            NavigationService?.Navigate(new AddOrderPage(selectedOrder));
        }
    }
}

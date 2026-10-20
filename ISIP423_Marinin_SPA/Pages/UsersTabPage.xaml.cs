using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = Core.Context.Users.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x =>
                x.Reload());
                DataGridUser.ItemsSource = Core.Context.Users.ToList();
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUserPage(null));
        }
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<Users>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count()} элементов? ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var user in usersForRemoving)
                    {
                        var appointments = Core.Context.Appointments.Where(a => a.UserID == user.UserID).ToList();
                        Core.Context.Appointments.RemoveRange(appointments);

                        var carts = Core.Context.Cart.Where(c => c.UserID == user.UserID).ToList();
                        foreach (var cart in carts)
                        {
                            var productsCart = Core.Context.ProductsCart.Where(pc => pc.CartID == cart.CartID).ToList();
                            Core.Context.ProductsCart.RemoveRange(productsCart);
                        }
                        Core.Context.Cart.RemoveRange(carts);

                        var orders = Core.Context.Orders.Where(o => o.UserID == user.UserID).ToList();
                        foreach (var order in orders)
                        {
                            var productsOrder = Core.Context.ProductsOrder.Where(po => po.OrderID == order.OrderID).ToList();
                            Core.Context.ProductsOrder.RemoveRange(productsOrder);
                        }
                        Core.Context.Orders.RemoveRange(orders);

                        var services = Core.Context.Services.Where(s => s.MasterID == user.UserID).ToList();
                        foreach (var service in services)
                        {
                            var serviceAppointments = Core.Context.Appointments.Where(a => a.ServiceID == service.ServiceID).ToList();
                            Core.Context.Appointments.RemoveRange(serviceAppointments);
                        }
                        Core.Context.Services.RemoveRange(services);
                    }

                    Core.Context.Users.RemoveRange(usersForRemoving);

                    Core.Context.SaveChanges();

                    MessageBox.Show("Данные успешно удалены!");
                    DataGridUser.ItemsSource = Core.Context.Users.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) 
        { 
            NavigationService.Navigate(new Pages.AddUserPage((sender as Button).DataContext as Users)); 
        }
    }
}

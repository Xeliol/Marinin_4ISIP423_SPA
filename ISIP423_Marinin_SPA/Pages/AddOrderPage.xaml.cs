using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AddOrderPage : Page
    {
        private Orders _currentOrder = new Orders();
        private bool _isNewOrder = true;

        public AddOrderPage(Orders selectedOrder)
        {
            InitializeComponent();

            cmbUser.ItemsSource = Core.Context.Users.ToList();
            cmbPaymentType.ItemsSource = Core.Context.PaymentTypes.ToList();

            if (selectedOrder != null)
            {
                _currentOrder = selectedOrder;
                _isNewOrder = false;
            }
            else
            {
                _currentOrder.PreferedDate = DateTime.Now;
                _currentOrder.Delivered = false;
            }

            DataContext = _currentOrder;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (cmbUser.SelectedValue == null)
                errors.AppendLine("Выберите пользователя/клиента!");

            if (dpPreferedDate.SelectedDate == null)
                errors.AppendLine("Укажите желаемую дату доставки!");

            if (cmbPaymentType.SelectedValue == null)
                errors.AppendLine("Выберите тип оплаты!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentOrder.UserID = (int)cmbUser.SelectedValue;
            _currentOrder.UserID = Convert.ToInt32(cmbUser.SelectedValue);
            _currentOrder.PaymentTypeID = Convert.ToInt32(cmbPaymentType.SelectedValue);

            if (_isNewOrder)
            {
                Core.Context.Orders.Add(_currentOrder);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Заказ успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в базу данных: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentOrder = new Orders
            {
                PreferedDate = DateTime.Now,
                Delivered = false
            };
            DataContext = _currentOrder;
            cmbUser.SelectedIndex = -1;
            cmbPaymentType.SelectedIndex = -1;
            _isNewOrder = true;
        }
    }
}

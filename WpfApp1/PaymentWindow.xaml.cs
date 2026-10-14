using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        Cart cart = new Cart();
        public PaymentWindow(Cart crt)
        {
            InitializeComponent();

            PaymentBox.ItemsSource = Core.Context.PaymentTypes.Select(p => p.Name).ToList();

            PaymentBox.SelectedIndex = 0;

            cart = crt;
            SortCalendar.SelectedDate = DateTime.Now.AddDays(1);
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            CurData UserInfo = NavigationData.CurrentData as CurData;
            Orders order = new Orders
            {
                UserID = UserInfo.ID,
                Delivered = false,
                PreferedDate = SortCalendar.SelectedDate.Value,
                PaymentTypeID = PaymentBox.SelectedIndex + 1,
            };
            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            List<ProductsCart> pcs = Core.Context.ProductsCart.Where(pc => pc.CartID == cart.CartID).ToList();

            foreach (ProductsCart pc in pcs)
            {
                Core.Context.ProductsOrder.Add(
                new ProductsOrder
                {
                    ProductID = pc.Products.ProductID,
                    OrderID = order.OrderID,
                });

                Core.Context.ProductsCart.Remove(pc);

                Core.Context.SaveChanges();
            }

            MessageBox.Show("Заказ успешно оформлен. Статус можно проверить на странице акааунта.");

            this.Close();  
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SortCalendar.SelectedDates.Clear();
        }

        private void SortCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortCalendar.SelectedDates.Count() > 1)
            {
                MessageBox.Show("Выберите одну дату.");
                SortCalendar.SelectedDate = DateTime.Now.AddDays(1);
            }
            if (SortCalendar.SelectedDate.Value.DayOfYear - DateTime.Now.DayOfYear > 7)
            {
                MessageBox.Show("Дата должна быть не более чем 7 дней после текущего числа.");
                SortCalendar.SelectedDate = DateTime.Now.AddDays(1);
            }
            if (SortCalendar.SelectedDate.Value.DayOfYear - DateTime.Now.DayOfYear < 0)
            {
                MessageBox.Show("Дата должна быть не более чем 7 дней после текущего числа.");
                SortCalendar.SelectedDate = DateTime.Now.AddDays(1);
            }
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Message",
            MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
            else

                e.Cancel = false;
        }
    }
}

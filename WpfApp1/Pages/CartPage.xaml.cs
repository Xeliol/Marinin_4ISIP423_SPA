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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        Cart cart = new Cart();
        public CartPage()
        {
            InitializeComponent();

            var curdata = NavigationData.CurrentData as CurData;
            Users user = Core.Context.Users.ToList()[curdata.ID - 1];

            this.DataContext = user;

            cart = Core.Context.Cart.First(c => c.UserID == user.UserID);

            ResetCart();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            ListBoxItem listBoxItem = (ListBoxItem)CartListBox.ContainerFromElement(clickedButton);

            if (listBoxItem != null)
            {
                object dataItem = listBoxItem.Content;

                Products prod = dataItem as Products;

                Core.Context.ProductsCart.Remove(Core.Context.ProductsCart.First(pc => pc.CartID == cart.CartID && pc.ProductID == prod.ProductID));

                Core.Context.SaveChanges();

                ResetCart();
            }
        }

        private void ResetCart()
        {
            List<ProductsCart> pcs = Core.Context.ProductsCart.Where(pc => pc.CartID == cart.CartID).ToList();

            List<Products> prods = new List<Products>();

            double summ = 0;

            foreach (ProductsCart pc in pcs)
            {
                prods.Add(pc.Products);

                summ += pc.Products.Price;
            }

            CartListBox.ItemsSource = prods;

            Price.Text = "Total Price: " + summ +" Rub.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ORDER PAGE
            Window1 win = new Window1(cart);
            win.Show();
        }
    }
}

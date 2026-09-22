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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        CurData user = NavigationData.CurrentData as CurData;
        public ProductsPage()
        {
            InitializeComponent();

            List<Products> products = Core.Context.Products.ToList();

            ProductListBox.ItemsSource = products;

            List<string> TypeSorts = Core.Context.ProductTypes.Select(pt => pt.Name).ToList();

            List<string> ManufacturerSorts = Core.Context.Manufacturers.Select(pt => pt.Name).ToList();

            TypeSorts.Insert(0, "None");

            ManufacturerSorts.Insert(0, "None");

            TypeSortBox.ItemsSource = TypeSorts;

            ManufacturerSortBox.ItemsSource = ManufacturerSorts;

            TypeSortBox.SelectedIndex = 0;

            ManufacturerSortBox.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((NavigationData.CurrentData as CurData).Type == 1)
            {
                Button clickedButton = sender as Button;

                ListBoxItem listBoxItem = (ListBoxItem)ProductListBox.ContainerFromElement(clickedButton);

                if (listBoxItem != null)
                {
                    Products dataItem = listBoxItem.Content as Products;

                    if (Core.Context.Cart.Where(c => c.UserID == user.ID).Count() == 0)
                    {
                        Core.Context.Cart.Add(new Cart { CartID = Core.Context.Cart.Count() + 1, UserID = user.ID });
                        Core.Context.SaveChanges();
                    }

                    Core.Context.ProductsCart.Add(new ProductsCart { CartID = Core.Context.Cart.Where(c => c.UserID == user.ID).First().CartID, ProductID = dataItem.ProductID });

                    Core.Context.SaveChanges();
                }
            }
            else
                MessageBox.Show("Only registered Clients can use the cart.");
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if ((NavigationData.CurrentData as CurData).ID == -1) NavigationService.Navigate(new LoginPage());
            else NavigationService.Navigate(new AccountPage());
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SortProducts();
            List<Products> newsrc = new List<Products>();
            foreach(Products p in ProductListBox.Items)
            {
                newsrc.Add(p);
            }
            if (SearchBox.Text.Length != 0)
            {
                ProductListBox.ItemsSource = newsrc.Where(p => p.Name.ToLower().Contains(SearchBox.Text.ToLower()));
            }
            else ProductListBox.ItemsSource = newsrc;
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            if((NavigationData.CurrentData as CurData).Type == 1)
            {
                NavigationService.Navigate(new CartPage());
            }
            else
            {
                MessageBox.Show("Only registered Clients can use the cart.");
            }
        }

        private void TypeSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortProducts();
        }

        private void ManufacturerSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortProducts();
        }

        private void SortProducts()
        {
            List<Products> newsource = Core.Context.Products.ToList();
            if (TypeSortBox.SelectedIndex == 0 && ManufacturerSortBox.SelectedIndex == 0)
            {
                ProductListBox.ItemsSource = Core.Context.Products.ToList();
            }
            else if (TypeSortBox.SelectedIndex != 0 && ManufacturerSortBox.SelectedIndex == 0)
            {
                ProductListBox.ItemsSource = Core.Context.Products.Where(u => u.ProductTypes.Name == TypeSortBox.SelectedItem).ToList();
            }
            else if (TypeSortBox.SelectedIndex == 0 && ManufacturerSortBox.SelectedIndex != 0)
            {
                ProductListBox.ItemsSource = Core.Context.Products.Where(u => u.Manufacturers.Name == ManufacturerSortBox.SelectedItem).ToList();
            }
            else if (TypeSortBox.SelectedIndex != 0 && ManufacturerSortBox.SelectedIndex != 0)
            {
                ProductListBox.ItemsSource = Core.Context.Products.Where(u => u.Manufacturers.Name == ManufacturerSortBox.SelectedItem && u.ProductTypes.Name == TypeSortBox.SelectedItem).ToList();
            }
        }
    }
}

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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        List<int> types = new List<int> { 1, 2, 3 };
        public AdminPage()
        {
            InitializeComponent();

            this.DataContext = types;

            UserListBox.ItemsSource = Core.Context.Users.Where(u => u.TypeID != 4).ToList();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationData.CurrentData = new CurData
            {
                ID = -1,
                Login = "Anon",
                Type = 0
            };
            MessageBox.Show("Logged Out.");
            NavigationService.Navigate(new StartPage());
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            Core.Context.Users.Add(new Users 
            { 
                Login = "Temp",
                Name = "Temp",
                Password = "Temp",
                Phone = "Temp",
                ImagePath = "/Images/default.jpg",
                TypeID = 1,
            });
            Core.Context.SaveChanges();

            UserListBox.ItemsSource = Core.Context.Users.Where(u => u.TypeID != 4).ToList();
        }

        private void EditButton(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            foreach(Users us in UserListBox.Items)
            {
                if(us.Name == "" || us.Login == "" || us.Password == "" || !(types.Contains(us.TypeID)))
                {
                    flag = false; 
                    break;
                }
            }
            if (flag)
            {
                Core.Context.SaveChanges();
            }
            else
            {
                MessageBox.Show("Error: check everything again, something is wrong.");
            }
        }

        private void RemoveButton(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            ListBoxItem listBoxItem = (ListBoxItem)UserListBox.ContainerFromElement(clickedButton);

            if (listBoxItem != null)
            {
                object dataItem = listBoxItem.Content;

                Users prod = dataItem as Users;

                if (prod.TypeID != 4)
                {
                    Core.Context.Users.Remove(prod);

                    Core.Context.SaveChanges();

                    UserListBox.ItemsSource = Core.Context.Users.Where(u => u.TypeID != 4).ToList();
                }
                else MessageBox.Show("Cannot remove Admin.");
            }
        }
    }
}

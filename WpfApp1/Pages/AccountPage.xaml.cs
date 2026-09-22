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
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();

            var curdata = NavigationData.CurrentData as CurData;
            Users user = Core.Context.Users.ToList()[curdata.ID - 1];

            this.DataContext = user;

            SessionListBox.ItemsSource = Core.Context.Appointments.Where(t => t.UserID == user.UserID).ToList();

            ProductListBox.ItemsSource = Core.Context.Orders.Where(o => o.UserID == user.UserID).ToList();
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
    }
}

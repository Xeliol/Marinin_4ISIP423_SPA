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
    /// Логика взаимодействия для MasterAccountPage.xaml
    /// </summary>
    public partial class MasterAccountPage : Page
    {
        List<Appointments> checked_a = new List<Appointments>();
        List<Appointments> apps_source = new List<Appointments>();
        List<int> services = new List<int>();

        public MasterAccountPage()
        {
            InitializeComponent();

            var curdata = NavigationData.CurrentData as CurData;
            Users user = Core.Context.Users.ToList()[curdata.ID - 1];

            this.DataContext = user;

            services = user.Services.Select(s => s.ServiceID).ToList();
            
            SessionListBox.ItemsSource = Core.Context.Appointments.Where(a => services.Contains(a.Services.ServiceID) && a.UserID != null).ToList();
            ServiceListBox.ItemsSource = user.Services.ToList();
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

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {

                Appointments app = clickedButton.DataContext as Appointments;

                if (app != null)
                {
                    Core.Context.Appointments.Remove(app);
                    Core.Context.SaveChanges();
                    SessionListBox.ItemsSource = Core.Context.Appointments.Where(a => services.Contains(a.Services.ServiceID) && a.UserID != null).ToList();
                }
            }
        }

        private void ManageAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AppointmentsTabPage());
        }

        private void ManageServicesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ServicesTabPage());
        }
    }
}

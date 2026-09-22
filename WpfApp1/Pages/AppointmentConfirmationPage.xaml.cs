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
    /// Логика взаимодействия для AppointmentConfirmationPage.xaml
    /// </summary>
    public partial class AppointmentConfirmationPage : Page
    {
        Appointments appointment;
        public AppointmentConfirmationPage(Appointments app)
        {
            this.DataContext = app;

            InitializeComponent();

            appointment = app;
        }

        private void MasterButton_Click(object sender, RoutedEventArgs e)
        {
            Users master = Core.Context.Users.FirstOrDefault(u => u.UserID == appointment.Services.MasterID);
            if (master != null) NavigationService.Navigate(new MasterPage(master));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Appointments cur_app = Core.Context.Appointments.First(a => a.AppointmentID == appointment.AppointmentID);
            CurData cur_user = NavigationData.CurrentData as CurData;
            cur_app.UserID = cur_user.ID;
            cur_app.Reserved = true;
            Core.Context.SaveChanges();

            MessageBox.Show("Booked the appointment!");
            NavigationService.Navigate(new AccountPage());
        }
    }
}

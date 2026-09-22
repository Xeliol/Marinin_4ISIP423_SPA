using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для AppointmentPage.xaml
    /// </summary>
    public partial class AppointmentPage : Page
    {
        Services service;        
        public AppointmentPage(Services srv)
        {
            service = srv;
            this.DataContext = srv;
            InitializeComponent();

            AppointmentListBox.ItemsSource = srv.Appointments.Where(a => a.Reserved == false).ToList();
        }

        private void MasterButton_Click(object sender, RoutedEventArgs e)
        {
            Users master = Core.Context.Users.FirstOrDefault(u => u.UserID == service.MasterID);
            if(master != null)  NavigationService.Navigate(new MasterPage(master));
        }

        private void SortCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            AppointmentListBox.ItemsSource = service.Appointments.Where(a => a.Reserved == false && SortCalendar.SelectedDates.Contains(a.Date.Date)).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SortCalendar.SelectedDates.Clear();
            AppointmentListBox.ItemsSource = service.Appointments.Where(a => a.Reserved == false).ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((NavigationData.CurrentData as CurData).Type == 1)
            {
                Button clickedButton = sender as Button;

                if (clickedButton != null)
                {

                    var app = clickedButton.DataContext as Appointments;

                    if (app != null)
                    {
                        NavigationService.Navigate(new AppointmentConfirmationPage(app));
                    }
                }
            }else
            {
                MessageBox.Show("Only registered Clients can book appointments.");
            }
        }
    }
}

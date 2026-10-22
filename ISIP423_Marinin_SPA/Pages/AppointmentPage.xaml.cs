using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ISIP423_Marinin_SPA.Pages
{
    public partial class AppointmentPage : Page
    {
        Services service;

        private List<TimeSpan> workingHours = new List<TimeSpan>
        {
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
            new TimeSpan(16, 0, 0),
            new TimeSpan(17, 0, 0)
        };

        public AppointmentPage(Services srv)
        {
            service = srv;
            this.DataContext = srv;
            InitializeComponent();

            RefreshAvailableSlots(DateTime.Today);
        }

        private void RefreshAvailableSlots(DateTime selectedDate)
        {
            if (selectedDate.Date < DateTime.Today)
            {
                AppointmentListBox.ItemsSource = null;
                return;
            }

            int currentMasterId = service.MasterID;

            var masterServiceIds = Core.Context.Services
                .Where(s => s.MasterID == currentMasterId)
                .Select(s => s.ServiceID)
                .ToList();

            var reservedTimes = Core.Context.Appointments
                .Where(a => masterServiceIds.Contains(a.ServiceID) && a.Reserved == true)
                .AsEnumerable()
                .Where(a => a.Date.Date == selectedDate.Date)
                .Select(a => a.Date.TimeOfDay)
                .ToList();

            List<Appointments> availableSlots = new List<Appointments>();
            TimeSpan nowTime = DateTime.Now.TimeOfDay;

            foreach (var time in workingHours)
            {
                bool isReserved = reservedTimes.Contains(time);

                bool isPastTime = selectedDate.Date == DateTime.Today && time <= nowTime;

                if (!isReserved && !isPastTime)
                {
                    availableSlots.Add(new Appointments
                    {
                        ServiceID = service.ServiceID,
                        Services = service,
                        Date = selectedDate.Date.Add(time),
                        Reserved = false
                    });
                }
            }

            AppointmentListBox.ItemsSource = availableSlots;
        }

        private void MasterButton_Click(object sender, RoutedEventArgs e)
        {
            Users master = Core.Context.Users.FirstOrDefault(u => u.UserID == service.MasterID);
            if (master != null) NavigationService.Navigate(new MasterPage(master));
        }

        private void SortCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortCalendar.SelectedDate.HasValue)
            {
                RefreshAvailableSlots(SortCalendar.SelectedDate.Value);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SortCalendar.SelectedDates.Clear();
            RefreshAvailableSlots(DateTime.Today);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurData cur_user = NavigationData.CurrentData as CurData;

            if (cur_user != null && cur_user.Type == 1)
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
            }
            else
            {
                MessageBox.Show("Записываться могут только зарегистрированные клиенты.");
            }
        }
    }
}

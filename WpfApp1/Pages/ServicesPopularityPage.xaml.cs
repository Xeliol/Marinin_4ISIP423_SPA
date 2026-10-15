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
    public partial class ServicesPopularityPage : Page
    {
        public ServicesPopularityPage()
        {
            InitializeComponent();
            LoadPopularityData();
        }

        private void LoadPopularityData()
        {
            var popularityData = Core.Context.Appointments
                .Where(a => a.UserID != null)
                .GroupBy(a => a.Services.Name)
                .Select(g => new
                {
                    ServiceName = g.Key,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentCount)
                .ToList();

            int max = popularityData.Count > 0 ? popularityData.Max(x => x.AppointmentCount) : 100;

            DgServices.ItemsSource = popularityData;

            IcPopularityChart.ItemsSource = popularityData;
            IcPopularityChart.DataContext = new { MaxCount = max };
        }

    }
}

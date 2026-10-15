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
    public partial class MastersLoadPage : Page
    {
        public MastersLoadPage()
        {
            InitializeComponent();
            LoadMastersData();
        }

        private void LoadMastersData()
        {
            var mastersData = Core.Context.Services
                .Where(s => s.Users != null)
                .GroupBy(s => s.Users.Name)
                .Select(g => new
                {
                    MasterName = g.Key,
                    ServicesCount = g.Count()
                })
                .OrderByDescending(x => x.ServicesCount)
                .ToList();

            int max = mastersData.Count > 0 ? mastersData.Max(x => x.ServicesCount) : 100;

            DgMasters.ItemsSource = mastersData;

            IcMastersChart.ItemsSource = mastersData;
            IcMastersChart.DataContext = new { MaxServices = max };
        }
    }
}


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
    public partial class OrdersDynamicReportPage : Page
    {
        public OrdersDynamicReportPage()
        {
            InitializeComponent();
            LoadOrdersData();
        }

        private void LoadOrdersData()
        {
            var ordersData = Core.Context.Orders
                .ToList()
                .GroupBy(o => o.PreferedDate.Date)
                .Select(g => new
                {
                    OrderDate = g.Key,
                    TotalOrders = g.Count()
                })
                .OrderBy(d => d.OrderDate)
                .ToList();

            int max = ordersData.Count > 0 ? ordersData.Max(x => x.TotalOrders) : 100;

            DgOrdersReport.ItemsSource = ordersData;

            IcOrdersChart.ItemsSource = ordersData;
            IcOrdersChart.DataContext = new { MaxOrders = max };
        }
    }
}


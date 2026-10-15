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
    public partial class ProductTypesReportPage : Page
    {
        public ProductTypesReportPage()
        {
            InitializeComponent();
            LoadReportData();
        }

        private void LoadReportData()
        {
            var reportData = Core.Context.Products
                .GroupBy(p => p.ProductTypes.Name)
                .Select(g => new
                {
                    TypeName = g.Key,
                    ProductCount = g.Count()
                })
                .ToList();

            int max = reportData.Count > 0 ? reportData.Max(x => x.ProductCount) : 100;

            DgReport.ItemsSource = reportData;

            IcProductChart.ItemsSource = reportData;
            IcProductChart.DataContext = new { MaxCount = max };
        }
    }
}


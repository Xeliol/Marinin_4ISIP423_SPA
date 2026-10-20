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

namespace ISIP423_Marinin_SPA.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        private void ServicesPopularity_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ServicesPopularityPage());
        }

        private void MastersLoad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MastersLoadPage());
        }

        private void OrdersDynamic_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersDynamicReportPage());
        }

        private void ProductTypes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductTypesReportPage());
        }
    }
}

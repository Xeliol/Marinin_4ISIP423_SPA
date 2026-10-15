using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Pages
{
    public partial class ServiceTypesTabPage : Page
    {
        public ServiceTypesTabPage()
        {
            InitializeComponent();
            DataGridServiceTypes.ItemsSource = Core.Context.ServiceTypes.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridServiceTypes.ItemsSource = Core.Context.ServiceTypes.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddServiceTypePage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var typesForRemoving = DataGridServiceTypes.SelectedItems.Cast<ServiceTypes>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {typesForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.ServiceTypes.RemoveRange(typesForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    DataGridServiceTypes.ItemsSource = Core.Context.ServiceTypes.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении. Возможно, к этому типу уже привязаны существующие услуги (Services).\n\nДетали: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = (sender as Button).DataContext as ServiceTypes;
            NavigationService?.Navigate(new AddServiceTypePage(selectedType));
        }
    }
}

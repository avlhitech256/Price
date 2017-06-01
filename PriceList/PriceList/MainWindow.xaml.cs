using System;
using System.Linq;
using System.Windows;
using Common.ServiceContract;
using PriceList.Service;

namespace PriceList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            IPriceListService service = PriceListService.GetFactory().CreateChannel();

            using (service as IDisposable)
            {
                string items = service.UpdatePriceList("Hello", DateTime.Now).Items?.FirstOrDefault()?.Manufacturer;
                LabelResult.Content = items;
            }
        }
    }
}

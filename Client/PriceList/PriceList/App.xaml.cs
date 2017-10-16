using System.Windows;
using Common.Thread;

namespace PriceList
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            UIContext.Initialize();
        }
    }
}

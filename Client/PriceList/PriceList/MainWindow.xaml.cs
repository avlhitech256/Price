using System.Windows;
using Common.Messenger;
using Domain.DomainContext;

namespace PriceList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            DomainContext = new DomainContext();
            //DataContext = new MainWindowViewModel(DomainContext);

            SetDomainContext();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }
        private IMessenger Messenger => DomainContext?.Messenger;

        #endregion

        #region Methods

        private void SetDomainContext()
        {
            TopMenuControl.DomainContext = DomainContext;
            FooterStatusBarControl.DomainContext = DomainContext;
        }

        #endregion

    }
}

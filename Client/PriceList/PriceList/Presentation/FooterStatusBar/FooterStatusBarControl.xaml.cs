using System.Windows.Controls;
using Domain.DomainContext;

namespace PriceList.Presentation.FooterStatusBar
{
    /// <summary>
    /// Логика взаимодействия для FooterStatusBarControl.xaml
    /// </summary>
    public partial class FooterStatusBarControl : UserControl
    {
        #region Constructors

        public FooterStatusBarControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext
        {
            set
            {
                DataContext = value;
            }

        }

        #endregion
    }
}

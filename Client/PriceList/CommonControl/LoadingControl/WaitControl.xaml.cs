using System.Windows.Controls;

namespace CommonControl.LoadingControl
{
    /// <summary>
    /// Логика взаимодействия для WaitControl.xaml
    /// </summary>
    public partial class WaitControl : UserControl
    {
        #region Constructors

        public WaitControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string Text
        {
            get
            {
                return (string) LoadingLabel.Content;
            }
            set
            {
                if (((string) LoadingLabel.Content) != value)
                {
                    LoadingLabel.Content = value;
                }
            }
        }

        #endregion
    }
}

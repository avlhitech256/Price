using System.Windows;

namespace CommonControl.LoadingControl
{
    /// <summary>
    /// Логика взаимодействия для WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {
        public WaitWindow()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return WaitControl.Text;
            }
            set
            {
                if (WaitControl.Text != value)
                {
                    WaitControl.Text = value;
                }
            }
        }
    }
}

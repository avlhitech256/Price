using System.Windows.Controls;
using System.Windows.Input;
using Photo.ViewModel;

namespace Photo.View
{
    /// <summary>
    /// Логика взаимодействия для PhotoControl.xaml
    /// </summary>
    public partial class PhotoControl : UserControl
    {
        #region Constructors
        public PhotoControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties
        private PhotoViewModel ViewModel => (PhotoViewModel) DataContext;

        #endregion

        #region Methods

        private void PhotoControl_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Equals(e.OriginalSource, Photo))
            {
                ViewModel?.Scale();
            }
        }

        #endregion
    }
}

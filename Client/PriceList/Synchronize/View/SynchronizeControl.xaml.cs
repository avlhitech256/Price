using CommonControl.SearchControl;
using System.Windows;

namespace Synchronize.View
{
    /// <summary>
    /// Логика взаимодействия для SynchronizeControl.xaml
    /// </summary>
    public partial class SynchronizeControl : SearchControl
    {
        public SynchronizeControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void SetDomainContext()
        {
            //throw new NotImplementedException();
        }

        protected override void SubscribeMessenger()
        {
            //throw new NotImplementedException();
        }

        protected override void UnsubscribeMessenger()
        {
            //throw new NotImplementedException();
        }
    }
}

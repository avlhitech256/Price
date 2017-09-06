using System;
using CommonControl.SearchControl;

namespace Order.View
{
    /// <summary>
    /// Логика взаимодействия для OrderControl.xaml
    /// </summary>
    public partial class OrderControl : SearchControl
    {
        public OrderControl()
        {
            InitializeComponent();
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

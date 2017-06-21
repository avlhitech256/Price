using System.Windows.Controls;
using Common.Messenger;
using Domain.DomainContext;

namespace CommonControl.SearchControl
{
    public class SearchControl : UserControl
    {
        #region Members

        private IDomainContext domainContext;
        private IMessenger messenger;

        #endregion

        #region Properties

        public IDomainContext DomainContext
        {
            get
            {
                return domainContext;
            }

            set
            {
                if (domainContext != value)
                {
                    domainContext = value;

                    if (value != null)
                    {
                        Messenger = value.Messenger;
                    }

                }

            }

        }

        private IMessenger Messenger
        {
            get
            {
                return messenger;
            }

            set
            {
                if (messenger != value)
                {
                    //UnsubscribeMessenger();
                    messenger = value;
                    //SubscribeMessenger();
                }

            }

        }

        #endregion
    }
}

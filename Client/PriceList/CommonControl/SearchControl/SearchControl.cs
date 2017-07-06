using System.Windows.Controls;
using Common.Messenger;
using Domain.DomainContext;

namespace CommonControl.SearchControl
{
    public class SearchControl : UserControl
    {
        #region Members

        private IDomainContext domainContext;

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
                    UnsubscribeMessenger();
                    domainContext = value;
                    SubscribeMessenger();
                }
            }
        }

        public IMessenger Messenger => DomainContext?.Messenger;

        #endregion

        #region Methods

        protected virtual void SubscribeMessenger()
        {
            
        }

        protected virtual void UnsubscribeMessenger()
        {
            
        }

        #endregion
    }
}

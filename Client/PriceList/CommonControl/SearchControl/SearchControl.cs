﻿using System.Windows.Controls;
using Common.Messenger;
using Domain.DomainContext;

namespace CommonControl.SearchControl
{
    abstract public class SearchControl : UserControl
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
                    SetDomainContext();
                    SubscribeMessenger();
                }
            }
        }

        public IMessenger Messenger => DomainContext?.Messenger;

        #endregion

        #region Methods

        abstract protected void SetDomainContext();

        abstract protected void SubscribeMessenger();

        protected abstract void UnsubscribeMessenger();

        #endregion
    }
}

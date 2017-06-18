﻿using System;
using Common.Annotations;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Media.Color;

namespace Domain.DomainContext
{
    public class DomainContext : Notifier, IDomainContext
    {
        #region Members

        private string dataBaseServer;

        #endregion

        #region Constructors

        public DomainContext()
        {
            Messenger = new Messenger();
            ColorService = new ColorService();
            UserName = Environment.UserName;
            UserDomain = Environment.UserDomainName;
            Workstation = Environment.MachineName;
            DataBaseServer = string.Empty;
        }

        #endregion

        #region Properties

        public IMessenger Messenger { get; }
        public IColorService ColorService { get; }

        [CanBeNull]
        public string UserName { get; }

        [CanBeNull]
        public string UserDomain { get; }

        [CanBeNull]
        public string Workstation { get; }

        [CanBeNull]
        public string DataBaseServer
        {
            get
            {
                return dataBaseServer;
            }

            set
            {
                if (dataBaseServer != value)
                {
                    dataBaseServer = value;
                    OnPropertyChanged();
                }

            }

        }
        
        #endregion
    }
}

using System;
using Common.Annotations;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.ViewModel;
using Media.Color;

namespace Domain.DomainContext
{
    public class DomainContext : Notifier, IDomainContext
    {
        #region Members

        private string dataBaseServer;
        private IControlViewModel viewModel;
        private bool isEditControl;

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
        public IControlViewModel ViewModel
        {
            get
            {
                return viewModel;
            }

            set
            {
                if (viewModel != value)
                {
                    viewModel = value;
                    OnPropertyChanged();
                }
            }

        }

        public bool IsEditControl
        {
            get
            {
                return isEditControl;
            }

            set
            {
                if (isEditControl != value)
                {
                    isEditControl = value;
                    OnPropertyChanged();
                }
            }
        }
        
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

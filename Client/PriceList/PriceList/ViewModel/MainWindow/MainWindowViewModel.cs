﻿using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.DomainContext;

namespace PriceList.ViewModel.MainWindow
{
    public class MainWindowViewModel : Notifier
    {
        #region Members

        private object view;


        #endregion

        #region Constructors

        public MainWindowViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            ViewModelRouter = new ViewModelRouter(DomainContext);
            SubscribeMessenger();
            SubscribeEvents();
        }

        #endregion


        #region Properties

        private IDomainContext DomainContext { get; }
        private IMessenger Messenger => DomainContext?.Messenger;
        private ViewModelRouter ViewModelRouter { get; }

        public object View
        {
            get
            {
                return view;
            }

            set
            {
                if (view != value)
                {
                    view = value;
                    OnPropertyChanged();
                }

            }

        }

        public bool IsLoading => DomainContext?.IsLoading ?? false;

        public bool IsWaiting => DomainContext?.IsWaiting ?? false;

        #endregion

        #region Methods

        private void SubscribeMessenger()
        {
            Messenger?.Register<MenuChangedEventArgs>(CommandName.SetEntryControl, SetEntryControl, CanSetEntryControl);
        }

        private void SubscribeEvents()
        {
            if (DomainContext != null)
            {
                DomainContext.PropertyChanged += DomainContext_PropertyChanged;
            }
        }

        private void DomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DomainContext.IsLoading):
                    OnPropertyChanged(nameof(IsLoading));
                    break;
                case nameof(DomainContext.IsWaiting):
                    OnPropertyChanged(nameof(IsWaiting));
                    break;
            }
        }

        public void SetEntryControl(MenuChangedEventArgs args)
        {
            var viewFactory = new ViewFactory(DomainContext, ViewModelRouter);
            View = viewFactory.GetView(args.MenuItemName, View);
            Messenger?.Send(CommandName.SelectLeftMenu, args);
        }

        private bool CanSetEntryControl(MenuChangedEventArgs args)
        {
            return true;
        }
        
        #endregion
    }
}

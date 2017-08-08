using System.Collections.ObjectModel;
using Basket.Model;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.ViewModel.Command;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;

namespace Basket.ViewModel
{
    public class BasketViewModel : Notifier, IControlViewModel
    {
        #region Constructors

        public BasketViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new BasketModel(domainContext);
            HasChanges = false;
            ShowPhotoOnMouseDoubleClick = false;
            InitCommands();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }
        public IMessenger Messenger => DomainContext?.Messenger;

        public bool ShowPhotoOnMouseDoubleClick { get; }

        private BasketModel Model { get; }

        public CatalogItem SelectedItem
        {
            get
            {
                return Model?.SelectedItem;
            }

            set
            {
                if (Model != null)
                {
                    Model.SelectedItem = value;
                }

            }

        }

        public ObservableCollection<CatalogItem> Entities => Model?.Entities;

        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }
        public bool IsEditControl { get; set; }
        public bool HasChanges { get; }

        public DelegateCommand AddCommand { get; private set; }

        #endregion

        #region Methods
        public void ApplySearchCriteria()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void View()
        {
            throw new System.NotImplementedException();
        }

        public void Edit()
        {
            throw new System.NotImplementedException();
        }

        public bool Save()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        private void InitCommands()
        {
            CreateCommand();
            SubscribeCommand();
        }

        private void SubscribeCommand()
        {
            
        }

        private void CreateCommand()
        {
            AddCommand = new DelegateCommand(DoAdd);
        }

        private void DoAdd(object parametr)
        {
            Messenger?.Send(CommandName.SetFocusTopMenu, new SetMenuFocusEventArgs(MenuItemName.PriceList));
            //Messenger?.Send(CommandName.SetEntryControl, new MenuChangedEventArgs(MenuItemName.PriceList));
        }

        #endregion
    }
}

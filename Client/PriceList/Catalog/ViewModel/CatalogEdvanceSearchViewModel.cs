using System.Collections.ObjectModel;
using System.ComponentModel;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Notifier;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.ViewModel
{
    public class CatalogEdvanceSearchViewModel : Notifier
    {
        #region Constructors

        public CatalogEdvanceSearchViewModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            Model = new CatalogBrandModel(domainContext, searchCriteria);
            SearchCriteria = searchCriteria;
            SubscribeEvents();
        }

        #endregion

        #region Properties

        private CatalogBrandModel Model { get; }

        public CatalogSearchCriteria SearchCriteria { get; }

        public BrandItem SelectedItem
        {
            get { return Model?.SelectedItem; }
            set { Model.SelectedItem = value; }
        }

        public ObservableCollection<BrandItem> Entities => Model?.Entities;

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
            }

            if (SearchCriteria != null)
            {
                SearchCriteria.EnabledEdvanceSearchChanged += SearchCriteria_EnabledEdvanceSearchChanged;
            }
        }

        private void SearchCriteria_EnabledEdvanceSearchChanged(object sender, Common.Event.DoubleAnimationEventArgs e)
        {
            if (SearchCriteria != null)
            {
                if (SearchCriteria.Vaz || SearchCriteria.Zaz || SearchCriteria.Gaz)
                {
                    Refresh();
                }
            }
            else
            {
                Refresh();
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(Model.SelectedItem):
                        OnPropertyChanged(nameof(SelectedItem));
                        break;
                    case nameof(Model.Entities):
                        OnPropertyChanged(nameof(Entities));
                        break;
                }
            }
        }

        private void Refresh()
        {
            Model.SelectEntities();
        }

        #endregion
    }
}

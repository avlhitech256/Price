using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
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

        public BrandItemEntity SelectedItem
        {
            get { return Model?.SelectedItem; }
            set { Model.SelectedItem = value; }
        }

        public List<BrandItemEntity> Entities => Model?.Entities;

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
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

        public void Refresh()
        {
            Model.SelectEntities();
        }

        #endregion
    }
}

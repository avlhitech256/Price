using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Notifier;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.ViewModel
{
    public class CatalogBrandViewModel : Notifier //CatalogEdvanceSearchViewModel
    {
        #region Constructors

        public CatalogBrandViewModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
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

        public List<BrandItem> Entities => Model?.Entities;

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
                        SearchCriteria.BrandComplited();
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

        public void OnCheck(BrandItem item)
        {
            if (item.Selected)
            {
                AddItem(item);
                SetCheck(item);
            }
            else
            {
                DeleteItem(item);
                SetUncheck(item);
            }

            SearchCriteria.BrandComplited();
        }

        private void SetCheck(BrandItem item)
        {
            AddItem(item);
        }

        private void SetUncheck(BrandItem item)
        {
            if (item.Subbrands != null && item.Subbrands.Any())
            {
                List<BrandItem> subdirectories = item.Subbrands.ToList();
                subdirectories.ForEach(
                    x =>
                    {
                        x.Selected = item.Selected && x.Selected;

                        if (!x.Selected)
                        {
                            DeleteItem(x);
                        }
                    });
                subdirectories.ForEach(SetUncheck);
            }
        }

        private void AddItem(BrandItem item)
        {
            if (SearchCriteria != null && !SearchCriteria.SelectedBrandItems.Contains(item))
            {
                SearchCriteria.SelectedBrandItems.Add(item);
            }
        }

        private void DeleteItem(BrandItem item)
        {
            SearchCriteria?.SelectedBrandItems?.Remove(item);
        }

        #endregion
    }
}

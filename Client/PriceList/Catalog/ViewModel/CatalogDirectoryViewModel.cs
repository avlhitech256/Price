using System.Collections.Generic;
using System.Linq;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Notifier;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.ViewModel
{
    public class CatalogDirectoryViewModel: Notifier
    {
        #region Members

        private readonly CatalogSearchCriteria searchCriteria;

        #endregion

        #region Constructors

        public CatalogDirectoryViewModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            this.searchCriteria = searchCriteria;
            Model = new CatalogDirectoryModel(domainContext, searchCriteria);
            SubscribeEvents();
        }

        #endregion

        #region Properties

        private CatalogDirectoryModel Model { get; }

        public DirectoryItem SelectedItem
        {
            get
            {
                return Model?.SelectedItem;
            }
            set
            {
                if (Model != null && Model.SelectedItem != value)
                {
                    Model.SelectedItem = value;
                }
            }
        }

        public List<DirectoryItem> Entities => Model?.Entities;

        #endregion

        #region Methods

        public void Refresh()
        {
            Model?.SelectEntities();
        }

        public void OnCheck(DirectoryItem item)
        {
            if (item != null)
            {
                SelectTreeItem(item);
                searchCriteria.DirectoryComplited();
            }
        }

        private void SelectTreeItem(DirectoryItem item)
        {
            if (item != null)
            {
                if (item.Selected)
                {
                    AddItem(item);
                }
                else
                {
                    DeleteItem(item);
                }

                if (item.Subdirectories != null && item.Subdirectories.Any())
                {
                    item.Subdirectories.ForEach(
                        x =>
                        {
                            x.Selected = item.Selected;
                            SelectTreeItem(x);
                        });
                }
            }
        }

        private void AddItem(DirectoryItem item)
        {
            searchCriteria?.SelectedDirectoryItems?.Add(item);
        }

        private void DeleteItem(DirectoryItem item)
        {
            searchCriteria?.SelectedDirectoryItems?.Remove(item);
        }

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
            }

        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        #endregion
    }
}

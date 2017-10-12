using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<DirectoryItem> Entities => Model?.Entities;
        #endregion

        #region Methods

        public void Refresh()
        {
            Model?.SelectEntities();
        }

        public void OnCheck(DirectoryItem item)
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

            searchCriteria.DirectoryComplited();
        }

        private void SetCheck(DirectoryItem item)
        {
            while (item.Parent != null)
            {
                item.Parent.Selected = item.Selected;
                item = item.Parent;
                AddItem(item);
            }
        }

        private void SetUncheck(DirectoryItem item)
        {
            if (item.Subdirectories != null && item.Subdirectories.Any())
            {
                List<DirectoryItem> subdirectories = item.Subdirectories.ToList();
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

        private void AddItem(DirectoryItem item)
        {
            if (!searchCriteria.SelectedDirectoryItems.Contains(item))
            {
                searchCriteria.SelectedDirectoryItems.Add(item);
            }
        }

        private void DeleteItem(DirectoryItem item)
        {
            searchCriteria.SelectedDirectoryItems.Remove(item);
        }

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
            }

            if (searchCriteria != null)
            {
                searchCriteria.EnabledEdvanceSearchChanged += SearchCriteria_EnabledEdvanceSearchChanged;
            }
        }

        private void SearchCriteria_EnabledEdvanceSearchChanged(object sender, Common.Event.DoubleAnimationEventArgs e)
        {
            if (searchCriteria.EnabledEdvanceSearch)
            {
                Model.SelectEntities();
                searchCriteria.SelectedDirectoryItems.Clear();
                searchCriteria.DirectoryComplited();
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

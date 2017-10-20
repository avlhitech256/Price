using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Service;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.ViewModel
{
    public class CatalogDirectoryViewModel: Notifier
    {
        #region Members

        private readonly CatalogSearchCriteria searchCriteria;
        private readonly IAsyncOperationService asyncOperationService;
        public Action<string> ShowWaitScreen;
        public Action HideWaitScreen;
        private readonly AsyncOperationType[] waitFormSupported;

        #endregion

        #region Constructors

        public CatalogDirectoryViewModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            asyncOperationService = domainContext.AsyncOperationService;
            this.searchCriteria = searchCriteria;
            ShowWaitScreen = delegate { };
            HideWaitScreen = delegate { };
            waitFormSupported = new[] { AsyncOperationType.LoadCatalog };
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

        private void ShowWaitScreenWithType(AsyncOperationType type)
        {
            ShowWaitScreen.Invoke(GetDescription(type));
        }

        private static string GetDescription(AsyncOperationType type)
        {
            string description = string.Empty;

            if (type == AsyncOperationType.LoadCatalog)
            {
                description = "Подождите, идет формирование каталогов";
            }
            else 
            {
                description = string.Empty;
            }

            return description;
        }


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

            if (asyncOperationService != null)
            {
                asyncOperationService.OperationStarted += (s, args) =>
                {
                    if (waitFormSupported.Contains(args.Value))
                    {
                        ShowWaitScreenWithType(args.Value);
                    }
                };

                asyncOperationService.OperationCompleted += (s, args) =>
                {
                    if (waitFormSupported.Contains(args.Value))
                    {
                        HideWaitScreen();
                    }
                };
            }
        }

        private void SearchCriteria_EnabledEdvanceSearchChanged(object sender, Common.Event.DoubleAnimationEventArgs e)
        {
            if (searchCriteria.EnabledEdvanceSearch)
            {
                asyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadCatalog, LoadDirectory, true, null);
                //Model.SelectEntities();
                //searchCriteria.SelectedDirectoryItems.Clear();
                //searchCriteria.DirectoryComplited();
            }
        }

        private bool LoadDirectory(bool needToLoad)
        {
            bool result = false;

            if (needToLoad)
            {
                Model.SelectEntities();
                searchCriteria.SelectedDirectoryItems.Clear();
                searchCriteria.DirectoryComplited();
                result = true;
            }

            return result;
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

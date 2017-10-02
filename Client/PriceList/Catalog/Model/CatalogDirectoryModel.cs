using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Holders;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.Model
{
    public class CatalogDirectoryModel : Notifier
    {
        #region Members

        private TreeViewItem selectedItem;
        private List<TreeViewItem> entities;
        private IDomainContext domainContext;

        #endregion

        #region Constructors

        public CatalogDirectoryModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            DomainContext = domainContext;
            SearchCriteria = searchCriteria;
            Entities = new List<TreeViewItem>();
        }

        private IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        public CatalogSearchCriteria SearchCriteria { get; }


        #endregion

        #region Properties

        public TreeViewItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<TreeViewItem> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                if (entities != value)
                {
                    entities = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods
        public void SelectEntities()
        {
            if (SearchCriteria != null)
            {
                long directoryId = ((DirectoryItem) SelectedItem?.Tag)?.Id ?? -1L;
                Entities.Clear();
                LongHolder position = new LongHolder { Value = 0 };

                CreateDirectoryItems().ForEach(x => Entities.Add(CreateTreeViewItem(x)));
                OnPropertyChanged(nameof(Entities));
                SelectedItem = Entities.FirstOrDefault(x => ((DirectoryItem)x?.Tag)?.Id == directoryId) ?? Entities.FirstOrDefault();
            }
        }

        private TreeViewItem CreateTreeViewItem(DirectoryItem item)
        {
            TreeViewItem treeViewItem = null;

            if (item != null)
            {
                treeViewItem = new TreeViewItem
                {
                    Tag = item,
                    Header = item.Name
                };

                if (item.Subdirectories != null && item.Subdirectories.Any())
                {
                    item.Subdirectories.ForEach(x => treeViewItem.Items.Add(CreateTreeViewItem(x)));
                } 
            }

            return treeViewItem;
        }

        private List<DirectoryItem> CreateDirectoryItems()
        {
            List<DirectoryEntity> items = GetItems().ToList();
            List<DirectoryItem> result = new List<DirectoryItem>();
            items.ForEach(x => CreateItems(result, new DirectoryItem(x)));
            return result;
        }

        private void CreateItems(List<DirectoryItem> result, DirectoryItem item)
        {
            if (SearchInResult(result, item) && DataService?.DataBaseContext != null)
            {
                DirectoryEntity entity = DataService.DataBaseContext.DirectoryEntities
                    .FirstOrDefault(x => x.SubDirectory.Contains(item.Entity));

                if (entity == null)
                {
                    result.Add(item);
                }
                else
                {
                    DirectoryItem newItem = new DirectoryItem(entity);
                    newItem.Subdirectories.Add(item);
                    SearchInResult(result, newItem);
                }
            }
        }

        private bool SearchInResult(List<DirectoryItem> result, DirectoryItem item)
        {
            bool isSearch = false;

            foreach (DirectoryItem directoryItem in result)
            {
                if (directoryItem.Entity.SubDirectory.Contains(item.Entity))
                {
                    directoryItem.Subdirectories.Add(item);
                    isSearch = true;
                    break;
                }

                if (SearchInResult(directoryItem.Subdirectories, item))
                {
                    isSearch = true;
                    break;
                }
            }

            return isSearch;
        }

        private IQueryable<DirectoryEntity> GetItems()
        {

            return SearchCriteria != null && SearchCriteria.EnabledEdvanceSearch ? GetEdvanceItems() : GetStandardItems();
        }

        private IQueryable<DirectoryEntity> GetStandardItems()
        {
            IQueryable<DirectoryEntity> items = null;

            if (SearchCriteria != null)
            {
                Func<string, string[]> prepareArray =
                    x =>
                    {
                        List<string> results = new List<string>();

                        if (!string.IsNullOrWhiteSpace(x))
                        {
                            results = x.Split(',', ' ').ToList();
                            results.RemoveAll(string.IsNullOrWhiteSpace);
                        }

                        return results.ToArray();
                    };

                string[] codes = prepareArray(SearchCriteria.Code);
                string[] lexemes = prepareArray(SearchCriteria.Name);
                string[] articles = prepareArray(SearchCriteria.Article);
                DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
                DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);

                items = DataService.Select<DirectoryEntity>()
                    .SelectMany(x => x.CatalogItems)
                    .Distinct()
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                (SearchCriteria.IsNew &&
                                 x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                (SearchCriteria.PriceIsDown &&
                                 x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
                                (SearchCriteria.PriceIsUp &&
                                 x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Select(x => x.Directory)
                    .Distinct()
                    .OrderBy(x => x.Name);
            }

            return items;
        }

        private IQueryable<DirectoryEntity> GetEdvanceItems()
        {
            IQueryable<DirectoryEntity> items = null;

            if (SearchCriteria != null)
            {
                Func<string, string[]> prepareArray =
                    x =>
                    {
                        List<string> results = new List<string>();

                        if (!string.IsNullOrWhiteSpace(x))
                        {
                            results = x.Split(',', ' ').ToList();
                            results.RemoveAll(string.IsNullOrWhiteSpace);
                        }

                        return results.ToArray();
                    };

                string[] codes = prepareArray(SearchCriteria.Code);
                string[] lexemes = prepareArray(SearchCriteria.Name);
                string[] articles = prepareArray(SearchCriteria.Article);
                DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
                DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);
                List<Guid> commodityDirectionCriteria = SearchCriteria?.GetCommodityDirectionCriteria() ?? new List<Guid>();

                items = DataService.Select<BrandItemEntity>()
                    .SelectMany(x => x.CatalogItems)
                    .Distinct()
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                (SearchCriteria.IsNew &&
                                 x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                (SearchCriteria.PriceIsDown &&
                                 x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
                                (SearchCriteria.PriceIsUp &&
                                 x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Where(x => !commodityDirectionCriteria.Any() ||
                                x.CommodityDirection.Any(c => commodityDirectionCriteria.Contains(c.Code)))
                    .Select(x => x.Directory)
                    .Distinct()
                    .OrderBy(x => x.Name);
            }

            return items;
        }

        #endregion
    }
}

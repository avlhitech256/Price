using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catalog.SearchCriteria;
using Common.Data.Enum;
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

        private DirectoryItem selectedItem;
        private List<DirectoryItem> entities;

        #endregion

        #region Constructors

        public CatalogDirectoryModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            DomainContext = domainContext;
            SearchCriteria = searchCriteria;
            Entities = new List<DirectoryItem>();
        }

        private IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        public CatalogSearchCriteria SearchCriteria { get; }


        #endregion

        #region Properties

        public DirectoryItem SelectedItem
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

        public List<DirectoryItem> Entities
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
                try
                {
                    long directoryId = SelectedItem?.Id ?? -1L;

                    Entities.Clear();
                    List<DirectoryItem> loadedEntities = GetItems();//CreateDirectoryItems();

                    if (SearchCriteria != null)
                    {
                        SearchCriteria.DirectoryItems = GetDirectiryItems(loadedEntities);

                        if (SearchCriteria.SelectedDirectoryItems != null && SearchCriteria.SelectedDirectoryItems.Any())
                        {
                            SearchCriteria.DirectoryItems.ForEach(x => x.Selected = SearchCriteria.SelectedDirectoryItems.Any(d => d.Id == x.Id && d.Selected));
                            SearchCriteria.SelectedDirectoryItems.Clear();
                            SearchCriteria.DirectoryItems.Where(x => x.Selected).ToList().ForEach(s => SearchCriteria.SelectedDirectoryItems.Add(s));
                        }
                    }

                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            Entities.AddRange(loadedEntities);
                            OnPropertyChanged(nameof(Entities));
                            SelectedItem = Entities?.FirstOrDefault(x => x.Id == directoryId) ?? Entities?.FirstOrDefault();
                            SearchCriteria?.DirectoryComplited();
                        });
                }
                catch (Exception e)
                {
                    ;
                    //throw;
                }
            }
        }

        private List<DirectoryItem> GetDirectiryItems(List<DirectoryItem> items)
        {
            List<DirectoryItem> result = new List<DirectoryItem>(items);

            items.ForEach(
                x =>
                {
                    if (x != null && x.Subdirectories.Any())
                    {
                        result.AddRange(GetDirectiryItems(x.Subdirectories));
                    }
                });

            return result;
        } 

        //private List<DirectoryItem> CreateDirectoryItems()
        //{
        //    List<DirectoryEntity> items = GetItems().ToList();
        //    List<DirectoryItem> result = new List<DirectoryItem>();

        //    if (SearchCriteria.EnableEdvanceFakeTree)
        //    {
        //        result.Add(new DirectoryItem(DataService, 
        //            new DirectoryEntity { Id = -2, Name = "Запчасти легковых автомобилей"}));
        //    }

        //    items.ForEach(x => CreateItems(result, x));
        //    return result;
        //}

        //private void CreateItems(List<DirectoryItem> result, DirectoryEntity item)
        //{
        //    DirectoryItem newItem = new DirectoryItem(DataService, item);
        //    bool continueProcessing = true;

        //    do
        //    {
        //        DataService.LoadParent(newItem.Entity);
        //        DirectoryEntity parentEntity = newItem.Entity.Parent;

        //        if (parentEntity == null)
        //        {
        //            result.Add(newItem);
        //            continueProcessing = false;
        //        }
        //        else 
        //        {
        //            if (AddDirectoryItem(result, newItem))
        //            {
        //                continueProcessing = false;
        //            }
        //            else
        //            {
        //                DirectoryItem parentItem = new DirectoryItem(DataService, parentEntity);
        //                newItem.Parent = parentItem;
        //                parentItem.Subdirectories.Add(newItem);
        //                newItem = parentItem;
        //            }
        //        }

        //    } while (continueProcessing);
        //}

        //private bool AddDirectoryItem(List<DirectoryItem> items, DirectoryItem item)
        //{
        //    bool result = false;

        //    foreach (DirectoryItem resultItem in items)
        //    {
        //        result = AddItem(resultItem, item);

        //        if (result)
        //        {
        //            break;
        //        }
        //    }

        //    return result;
        //}

        //private bool AddItem(DirectoryItem resultItem, DirectoryItem item)
        //{
        //    bool result = false;

        //    if (resultItem.Id == item.Entity.Parent.Id)
        //    {
        //        item.Parent = resultItem;
        //        resultItem.Subdirectories.Add(item);
        //        result = true;
        //    }
        //    else if (resultItem.Subdirectories != null && resultItem.Subdirectories.Any())
        //    {
        //        foreach (DirectoryItem subdirectoryItem in resultItem.Subdirectories)
        //        {
        //            result = AddItem(subdirectoryItem, item);

        //            if (result)
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    return result;
        //}

        //private IQueryable<DirectoryEntity> GetItems()
        //{

        //    return SearchCriteria != null && SearchCriteria.EnabledAdvancedSearch ? GetEdvanceItems() : GetStandardItems();
        //}

        //private IQueryable<DirectoryEntity> GetStandardItems()
        //{
        //    IQueryable<DirectoryEntity> items = null;

        //    if (SearchCriteria != null)
        //    {
        //        Func<string, string[]> prepareArray =
        //            x =>
        //            {
        //                List<string> results = new List<string>();

        //                if (!string.IsNullOrWhiteSpace(x))
        //                {
        //                    results = x.Split(',', ' ').ToList();
        //                    results.RemoveAll(string.IsNullOrWhiteSpace);
        //                }

        //                return results.ToArray();
        //            };

        //        string[] codes = prepareArray(SearchCriteria.Code);
        //        string[] lexemes = prepareArray(SearchCriteria.Name);
        //        string[] articles = prepareArray(SearchCriteria.Article);
        //        DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
        //        DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);

        //        items = DataService.Select<DirectoryEntity>()
        //            .SelectMany(x => x.CatalogItems)
        //            .Distinct()
        //            .Where(x => !codes.Any() || codes.Contains(x.Code))
        //            .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
        //            .Where(x => !articles.Any() || articles.Contains(x.Article))
        //            .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
        //                        (SearchCriteria.IsNew &&
        //                         x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
        //                        (SearchCriteria.PriceIsDown &&
        //                         x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
        //                        (SearchCriteria.PriceIsUp &&
        //                         x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
        //            .Select(x => x.Directory)
        //            .Distinct()
        //            .OrderBy(x => x.Name);
        //    }

        //    return items;
        //}

        //private IQueryable<DirectoryEntity> GetEdvanceItems()
        //{
        //    IQueryable<DirectoryEntity> items = null;

        //    if (SearchCriteria != null)
        //    {
        //        Func<string, string[]> prepareArray =
        //            x =>
        //            {
        //                List<string> results = new List<string>();

        //                if (!string.IsNullOrWhiteSpace(x))
        //                {
        //                    results = x.Split(',', ' ').ToList();
        //                    results.RemoveAll(string.IsNullOrWhiteSpace);
        //                }

        //                return results.ToArray();
        //            };

        //        string[] codes = prepareArray(SearchCriteria.Code);
        //        string[] lexemes = prepareArray(SearchCriteria.Name);
        //        string[] articles = prepareArray(SearchCriteria.Article);
        //        DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
        //        DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);
        //        List<Guid> commodityDirectionCriteria = 
        //            SearchCriteria?.GetCommodityDirectionCriteria(CommodityDirectionType.Other) ?? new List<Guid>();

        //        items = DataService.Select<BrandItemEntity>()
        //            .SelectMany(x => x.CatalogItems)
        //            .Distinct()
        //            .Where(x => !codes.Any() || codes.Contains(x.Code))
        //            .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
        //            .Where(x => !articles.Any() || articles.Contains(x.Article))
        //            .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
        //                        (SearchCriteria.IsNew &&
        //                         x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
        //                        (SearchCriteria.PriceIsDown &&
        //                         x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
        //                        (SearchCriteria.PriceIsUp &&
        //                         x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
        //            .Where(x => commodityDirectionCriteria.Any() &&
        //                        x.CommodityDirection.Any(c => commodityDirectionCriteria.Contains(c.Code)))
        //            .Select(x => x.Directory)
        //            .Distinct()
        //            .OrderBy(x => x.Name);
        //    }

        //    return items;
        //}

        private List<DirectoryItem> GetItems()
        {
            var items = new List<DirectoryItem>();
            DirectoryItem item;

            if (SearchCriteria.Vaz)
            {
                item = GetDirectoryTopItem(CommodityDirection.Vaz);
                AddItem(items, item);
            }

            if (SearchCriteria.Gaz)
            {
                item = GetDirectoryTopItem(CommodityDirection.Gaz);
                AddItem(items, item);
            }

            if (SearchCriteria.Zaz)
            {
                item = GetDirectoryTopItem(CommodityDirection.Zaz);
                AddItem(items, item);
            }

            if (SearchCriteria.Chemistry)
            {
                item = GetDirectoryTopItem(CommodityDirection.Chemistry);
                AddItem(items, item);
            }

            if (SearchCriteria.Battery)
            {
                item = GetDirectoryTopItem(CommodityDirection.Battery);
                AddItem(items, item);
            }

            if (SearchCriteria.Gas)
            {
                item = GetDirectoryTopItem(CommodityDirection.Gas);
                AddItem(items, item);
            }

            if (SearchCriteria.Instrument)
            {
                item = GetDirectoryTopItem(CommodityDirection.Instrument);
                AddItem(items, item);
            }

            if (SearchCriteria.EnabledAdvancedSearch)
            {
                item = GetCommonDirectory();
                AddItem(items, item);
            }

            return items;
        }

        private void AddItem(List<DirectoryItem> items, DirectoryItem item)
        {

            if (item != null)
            {
                items.Add(item);
            }
        }
        private DirectoryItem CreateDirectoryItem(CommodityDirection direction)
        {
            DirectoryItem item = GetDirectoryTopItem(direction);
            DirectoryItem commonItem = GetCommonDirectory();
            item.Subdirectories.Add(commonItem);
            commonItem.Parent = item;
            return item;
        }

        private DirectoryItem GetCommonDirectory()
        {
            return GetDirectoryTopItem(CommodityDirection.Common);
        }

        private DirectoryItem GetDirectoryTopItem(CommodityDirection direction)
        {
            DirectoryItem item = null;

            DirectoryEntity entity =
                DataService.Select<ProductDirectionEntity>()
                    .FirstOrDefault(x => x.Direction == direction)?
                    .Directory;

            if (entity != null)
            {
                item = new DirectoryItem(DataService, entity);
                CreateSubDirectory(item);
            }

            return item;
        }

        private void CreateSubDirectory(DirectoryItem item)
        {
            if (item != null && item.Entity.SubDirectory.Any())
            {
                item.Entity.SubDirectory.ForEach(
                    x =>
                    {
                        DirectoryItem subDirectory = new DirectoryItem(DataService, x);
                        item.Subdirectories.Add(subDirectory);
                        subDirectory.Parent = item;
                        CreateSubDirectory(subDirectory);
                    });
            }

        }

        #endregion
    }
}

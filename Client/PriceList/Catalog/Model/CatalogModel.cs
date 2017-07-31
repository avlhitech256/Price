using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Catalog.Properties;
using Catalog.SearchCriteria;
using Common.Data.Holders;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Service.Precision;
using Media.Image;

namespace Catalog.Model
{
    public class CatalogModel : Notifier
    {
        #region Members

        private CatalogItem selectedItem;
        private CatalogItem oldSelectedItem;
        private ObservableCollection<CatalogItem> entities;
        private decimal amount;
        public event CountChangedEventHandler CountChanged;

        #endregion

        #region Constructors

        public CatalogModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Amount = 0;
            SearchCriteria = new CatalogSearchCriteria();
            InitData();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        private IImageService ImageService => DomainContext?.ImageService;

        private IPrecisionService PrecisionService => DomainContext?.PrecisionService;

        private IDataService DataServise => DomainContext.DataService;

        public CatalogSearchCriteria SearchCriteria { get; }

        public CatalogItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    CatalogItem oldValue = SelectedItem;
                    selectedItem = value;
                    OnPropertyChanged();
                    OnChangeSelectedItem(oldValue, value);
                    SendSetImageMessage();
                }
            }
        }

        public ObservableCollection<CatalogItem> Entities
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

        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void OnChangeSelectedItem(CatalogItem oldItem, CatalogItem newItem)
        {
            UnsubscribeSelectedItemEvents(oldItem);
            SubscribeSelectedItemEvents(newItem);
        }

        private void UnsubscribeSelectedItemEvents(CatalogItem oldItem)
        {
            if (oldItem != null)
            {
                oldItem.CountChanged -= OnAmountChanged;
            }
        }

        private void SubscribeSelectedItemEvents(CatalogItem newItem)
        {
            if (newItem != null)
            {
                newItem.CountChanged += OnAmountChanged;
            }
        }

        private void OnAmountChanged(object sender, DecimalValueChangedEventArgs e)
        {
            Amount = Amount - e.OldValue + e.NewValue;
            CountChanged?.Invoke(this, e);
        }

        private void SendSetImageMessage()
        {
            Messenger.Send(CommandName.SetImage, SelectedItem);
        }

        public void SelectEntities()
        {
            Func<string, string[]> prepareArray =
                x =>
                {
                    List<string> results = x.Split(',', ' ').ToList();
                    results.RemoveAll(string.IsNullOrWhiteSpace);
                    return results.ToArray();
                };

            Entities.Clear();

            string[] codes = prepareArray(SearchCriteria.Code);
            string[] lexemes = prepareArray(SearchCriteria.Name);
            string[] articles = prepareArray(SearchCriteria.Article);
            LongHolder position = new LongHolder();

            DataServise.Select<CatalogItemEntity>()
                .Include(x => x.Brand)
                .Where(x => !codes.Any() || codes.Contains(x.Code))
                .Where(n => lexemes.Any(s => n.Name.Contains(s)))
                .Where(x => !articles.Any() || articles.Contains(x.Article))
                .Where(x => !SearchCriteria.IsNew || (x.IsNew && x.LastUpdated <= DateTimeOffset.Now.AddDays(-14)))
                .Where(x => !SearchCriteria.PriceIsDown || (x.PriceIsDown && x.LastUpdated <= DateTimeOffset.Now.AddDays(-7)))
                .Where(x => !SearchCriteria.PriceIsUp || (x.PriceIsUp && x.LastUpdated <= DateTimeOffset.Now.AddDays(-7)))
                .Where(x => SearchCriteria.BrandId <= -1L || x.Brand.Id == SearchCriteria.BrandId)
                .ToList()
                .ForEach(x => Entities.Add(new CatalogItem(x, PrecisionService, ImageService) { Position = ++position.Value}));
        }

        private void InitData()
        {
            List<CatalogItemEntity> catalogItemEntities = DataServise.DataBaseContext.CatalogItemEntities.ToList();

            //ResourceReader recourceReader = new ResourceReader("Resources.resx");

            long id = 0L;
            long position = 1L;
            var brand1 = new BrandItem
            {
                Id = 0,
                Code = Guid.NewGuid(),
                Name = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД"
            };
            var brand2 = new BrandItem
            {
                Id = 1,
                Code = Guid.NewGuid(),
                Name = "АВТОВАЗ"
            };
            var brand3 = new BrandItem
            {
                Id = 1,
                Code = Guid.NewGuid(),
                Name = "Daewoo-ZAZ"
            };
            entities = new ObservableCollection<CatalogItem>();

            CatalogItem item = new CatalogItem(PrecisionService, ImageService)
            {
                Id = id++,
                Position = position++,
                Code = "04157",
                Article = "11180-1108054-00",
                Brand = brand1,
                Name = "Трос акселератора ВАЗ 1118 дв. 1,6л",
                EnterpriceNormPack = "10 шт.",
                Balance = "свыше 100 шт.",
                Unit = "шт.",
                Price = 50.52M,
                Count = "0.00",
                Currency = "EUR",
                Photos = new List<byte[]>
                {
                    ImageService?.ConvertToByteArray(Resources.Photo1),
                    ImageService?.ConvertToByteArray(Resources.Photo2),
                    ImageService?.ConvertToByteArray(Resources.Photo3)
                }
            };
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem(PrecisionService, ImageService)
            {
                Id = id++,
                Position = position++,
                Code = "87930",
                Article = "3160-3819010-00",
                Brand = brand2,
                Name = "Ступица передняя HYUNDAI H-1 07- Без ABS",
                EnterpriceNormPack = "10 шт.",
                Balance = "свыше 100 шт.",
                Unit = "шт.",
                Price = 320.45M,
                Count = "0.00",
                Currency = "грн.",
                Photos = new List<byte[]>
                {
                    ImageService?.ConvertToByteArray(Resources.Photo2),
                    ImageService?.ConvertToByteArray(Resources.Photo3),
                    ImageService?.ConvertToByteArray(Resources.Photo4)
                }
            };
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem(PrecisionService, ImageService)
            {
                Id = id++,
                Position = position++,
                Code = "98486",
                Article = "21213",
                Brand = brand1,
                Name = "Крышка бачка расширительного DAEWOO LANOS/SENS",
                EnterpriceNormPack = "10 шт.",
                Balance = "от 10 до 100 шт.",
                Unit = "шт.",
                Price = 18.65M,
                Count = "0.00",
                Currency = "грн.",
                Photos = new List<byte[]>
                {
                    ImageService?.ConvertToByteArray(Resources.Photo3),
                    ImageService?.ConvertToByteArray(Resources.Photo2)
                }
            };
            //recourceReader.GetResourceData("Photo3", out typeResource, out photo);
            //item.Photos = photo;
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem(PrecisionService, ImageService)
            {
                Id = id++,
                Position = position++,
                Code = "24029",
                Article = "11180-3508180-00",
                Brand = brand3,
                Name = "Баллон тороидальный 42л 600х200мм наружный, Atiker",
                EnterpriceNormPack = "1 шт.",
                Balance = "8 шт.",
                Unit = "шт.",
                Price = 120.78M,
                Count = "0.00",
                Currency = "USD",
                Photos = new List<byte[]>
                {
                    ImageService?.ConvertToByteArray(Resources.Photo4),
                    ImageService?.ConvertToByteArray(Resources.Photo5)

                }
            };
            //recourceReader.GetResourceData("Photo4", out typeResource, out photo);
            //item.Photos = photo;
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem(PrecisionService, ImageService)
            {
                Id = id++,
                Position = position++,
                Code = "24029",
                Article = "3302-8406140-00",
                Brand = brand1,
                Name = "Трос капота ГАЗ 3302",
                EnterpriceNormPack = "1 шт.",
                Balance = "2 шт.",
                Unit = "шт.",
                Price = 1850.46M,
                Count = "0.00",
                Currency = "грн.",
                Photos = new List<byte[]>
                {
                    ImageService?.ConvertToByteArray(Resources.Photo5),
                    ImageService?.ConvertToByteArray(Resources.Photo6),
                    ImageService?.ConvertToByteArray(Resources.Photo3)
                }
            };
            //recourceReader.GetResourceData("Photo5", out typeResource, out photo);
            //item.Photos = photo;
            Entities.Add(item);

            SelectedItem = Entities[0];
        }

        #endregion

    }
}

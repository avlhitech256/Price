﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.Annotations;
using Common.Data.Notifier;
using Common.Event;
using DatabaseService.DataBaseContext.Entities;
using Domain.Data.Object;

namespace Catalog.SearchCriteria
{
    public class CatalogSearchCriteria : Notifier
    {
        #region Members

        private string code;
        private string name;
        private string article;
        private long brandId;
        private string brandNeme;
        private bool priceIsDown;
        private bool priceIsUp;
        private bool isNew;
        private bool isModified;
        private bool isEmpty;
        private bool vaz;
        private bool gaz;
        private bool zaz;
        private bool chemistry;
        private bool battery;
        private bool gas;
        private bool instrument;
        private bool enabledEdvanceSearch;
        private int edvanceSearchWidth;

        private string oldCode;
        private string oldName;
        private string oldArticle;
        private long oldBrandId;
        private bool oldPriceIsDown;
        private bool oldPriceIsUp;
        private bool oldIsNew;
        private bool oldVaz;
        private bool oldGaz;
        private bool oldZaz;
        private bool oldChemistry;
        private bool oldBattery;
        private bool oldGas;
        private bool oldInstrument;

        private const int MinWidthMainWindowWithoutAdvancedSearch = 1070;

        private List<Guid> commodityVazDirection;
        private List<Guid> commodityGazDirection;
        private List<Guid> commodityZazDirection;
        private List<Guid> commodityChemistryDirection;
        private List<Guid> commodityBatteryDirection;
        private List<Guid> commodityGasDirection;
        private List<Guid> commodityInstrumentDirection;

        #endregion

        #region Constructors

        public CatalogSearchCriteria()
        {
            edvanceSearchWidth = 0;
            enabledEdvanceSearch = false;
            FirstBrandItemEntity = new BrandItemEntity {Id = -1L, Code = Guid.NewGuid(), Name = "Все бренды"};
            SelectedDirectoryItems = new ObservableCollection<DirectoryItem>();
            InitCommodityDirection();
            Clear();
            SearchComplited();
            IsModified = true;
        }

        #endregion

        #region Properties

        public List<Guid> CommodityVazDirection => commodityVazDirection;
        public List<Guid> CommodityGazDirection => commodityGazDirection;
        public List<Guid> CommodityZazDirection => commodityZazDirection;
        public List<Guid> CommodityChemistryDirection => commodityChemistryDirection;
        public List<Guid> CommodityBatteryDirection => commodityBatteryDirection;
        public List<Guid> CommodityGasDirection => commodityGasDirection;
        public List<Guid> CommodityInstrumentDirection => commodityInstrumentDirection;


        public bool IsModified
        {
            get
            {
                return isModified;
            }
            private set
            {
                if (isModified != value)
                {
                    isModified = value;
                    OnPropertyChanged();
                    OnSearchCriteriaChanged();
                }
            }
        }

        public BrandItemEntity FirstBrandItemEntity { get; }
        
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                if (code != value)
                {
                    code = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Article
        {
            get
            {
                return article;
            }
            set
            {
                if (article != value)
                {
                    article = value;
                    OnPropertyChanged();
                }
            }
        }

        public long BrandId
        {
            get
            {
                return brandId;
            }
            set
            {
                if (brandId != value)
                {
                    brandId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BrandName
        {
            get
            {
                return brandNeme;
            }
            set
            {
                if (brandNeme != value)
                {
                    brandNeme = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PriceIsDown
        {
            get
            {
                return priceIsDown;
            }
            set
            {
                if (priceIsDown != value)
                {
                    priceIsDown = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PriceIsUp
        {
            get
            {
                return priceIsUp;
            }
            set
            {
                if (priceIsUp != value)
                {
                    priceIsUp = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNew
        {
            get
            {
                return isNew;
            }
            set
            {
                if (isNew != value)
                {
                    isNew = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged();
                    OnSearchCriteriaCleared();
                }
            }
        }

        public bool Vaz
        {
            get
            {
                return vaz;
            }
            set
            {
                if (vaz != value)
                {
                    vaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Gaz
        {
            get
            {
                return gaz;
            }
            set
            {
                if (gaz != value)
                {
                    gaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Zaz
        {
            get
            {
                return zaz;
            }
            set
            {
                if (zaz != value)
                {
                    zaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Chemistry
        {
            get
            {
                return chemistry;
            }
            set
            {
                if (chemistry != value)
                {
                    chemistry = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Battery
        {
            get
            {
                return battery;
            }
            set
            {
                if (battery != value)
                {
                    battery = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Gas
        {
            get
            {
                return gas;
            }
            set
            {
                if (gas != value)
                {
                    gas = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Instrument
        {
            get
            {
                return instrument;
            }
            set
            {
                if (instrument != value)
                {
                    instrument = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool EnabledEdvanceSearch
        {
            get
            {
                return Vaz || Gaz || Zaz || Chemistry || Battery || Gas || Instrument; 
            }
            private set
            {
                if (enabledEdvanceSearch != value)
                {
                    enabledEdvanceSearch = value;
                    OnEnabledEdvanceSearchChanged();
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EnableEdvanceTree));
                    OnPropertyChanged(nameof(EnableBrandComboBox));
                }
            }
        }

        public bool EnableBrandComboBox => !EnabledEdvanceSearch;

        public bool EnableEdvanceTree => Chemistry || Battery || Gas || Instrument;

        private int EdvanceSearchWidth
        {
            get
            {
                return edvanceSearchWidth;
            }
            set
            {
                if (edvanceSearchWidth != value)
                {
                    edvanceSearchWidth = value;
                    OnEdvanceSearchWidthChanged();
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DirectoryItem> SelectedDirectoryItems { get; }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(IsModified) &&
                propertyName != nameof(EnabledEdvanceSearch) &&
                propertyName != nameof(EdvanceSearchWidth) &&
                propertyName != nameof(IsEmpty))
            {
                IsModified = GetModifyStatus();
            }

            IsEmpty = SearchCriteriaIsEmpty();
        }

        private void OnSearchCriteriaChanged()
        {
            SearchCriteriaChanged?.Invoke(this, new EventArgs());
        }

        private void OnSearchCriteriaCleared()
        {
            SearchCriteriaCleared?.Invoke(this, new EventArgs());
        }

        private void OnDirectoryChanged()
        {
            DirectoryItemsChanged?.Invoke(this, new EventArgs());
        }

        private void OnEnabledEdvanceSearchChanged()
        {
            var args = EnabledEdvanceSearch
                ? new DoubleAnimationEventArgs(0, 150)
                : new DoubleAnimationEventArgs(150, 0);
            EnabledEdvanceSearchChanged?.Invoke(this, args);
        }

        private void OnEdvanceSearchWidthChanged()
        {
            int minWidth = MinWidthMainWindowWithoutAdvancedSearch + EdvanceSearchWidth;
            EdvanceSearchWidthChanged?.Invoke(this, new MinWidthEventArgs(minWidth));
        }

        public void Clear()
        {
            Code = string.Empty;
            Name = string.Empty;
            Article = string.Empty;
            BrandId = FirstBrandItemEntity.Id;
            BrandName = FirstBrandItemEntity.Name;
            PriceIsDown = false;
            PriceIsUp = false;
            IsNew = false;
            Vaz = false;
            Gaz = false;
            Zaz = false;
            Chemistry = false;
            Battery = false;
            Gas = false;
            Instrument = false;
            SelectedDirectoryItems.Clear();
        }

        private bool SearchCriteriaIsEmpty()
        {
            return Code == string.Empty &&
                   Name == string.Empty &&
                   Article == string.Empty &&
                   BrandId == FirstBrandItemEntity.Id &&
                   !PriceIsDown &&
                   !PriceIsUp &&
                   !IsNew &&
                   !Vaz &&
                   !Gaz &&
                   !Zaz &&
                   !Chemistry &&
                   !Battery &&
                   !Gas &&
                   !Instrument;
        }

        public void SearchComplited()
        {
            CopyValueToOld();
            IsModified = false;
        }

        public void DirectoryComplited()
        {
            OnDirectoryChanged();
        }

        private void CopyValueToOld()
        {
            oldArticle = Article;
            oldBrandId = BrandId;
            oldCode = Code;
            oldName = Name;
            oldIsNew = IsNew;
            oldPriceIsDown = PriceIsDown;
            oldPriceIsUp = PriceIsUp;
            oldVaz = Vaz;
            oldGaz = Gaz;
            oldZaz = Zaz;
            oldChemistry = Chemistry;
            oldBattery = Battery;
            oldGas = Gas;
            oldInstrument = Instrument;
        }

        private bool GetModifyStatus()
        {
            return Article != oldArticle ||
                   BrandId != oldBrandId ||
                   Code != oldCode ||
                   Name != oldName ||
                   IsNew != oldIsNew ||
                   PriceIsDown != oldPriceIsDown ||
                   PriceIsUp != oldPriceIsUp ||
                   Vaz != oldVaz ||
                   Gaz != oldGaz ||
                   Zaz != oldZaz ||
                   Chemistry != oldChemistry ||
                   Battery != oldBattery ||
                   Gas != oldGas ||
                   Instrument != oldInstrument;
        }

        private void CalculateEdvanceSearchWidth()
        {
            EnabledEdvanceSearch = Vaz || Gaz || Zaz || Chemistry || Battery || Gas || Instrument;
            EdvanceSearchWidth = EnabledEdvanceSearch ? 150 : 0;
        }

        private void InitCommodityDirection()
        {
            commodityVazDirection = new List<Guid>
            {
                Guid.Parse("01b0f02e-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("01b0f02d-eb13-11e3-8064-00163e6000f8")
            };

            commodityGazDirection = new List<Guid>
            {
                Guid.Parse("01b0f032-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("228decab-d8a3-11e6-ae82-0025909a3c49"),
                Guid.Parse("01b0f02f-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("01b0f030-eb13-11e3-8064-00163e6000f8")
            };

            commodityZazDirection = new List<Guid>
            {
                Guid.Parse("01b0f025-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("01b0f038-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("4dfccc74-a71b-11e6-bdf3-0025909a3c49"),
                Guid.Parse("01b0f02b-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("c914c4e3-1b57-11e6-a58f-0025909a3c49"),
                Guid.Parse("aca2b2fb-7637-11e4-8fbf-b1a970ab8551"),
                Guid.Parse("01b0f027-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("85cf60de-cc06-11e1-af29-00163e791aa4"),
                Guid.Parse("8e592e83-77bc-11e5-a68c-0025909a3c49")
            };

            commodityChemistryDirection = new List<Guid>
            {
                Guid.Parse("01b0f03b-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("01b0f03c-eb13-11e3-8064-00163e6000f8")
            };

            commodityBatteryDirection = new List<Guid>
            {
                Guid.Parse("e2eda7fa-5452-11e4-a274-d8a704927acb")
            };

            commodityGasDirection = new List<Guid>
            {
                Guid.Parse("01b0f038-eb13-11e3-8064-00163e6000f8"),
                Guid.Parse("01b0f039-eb13-11e3-8064-00163e6000f8")
            };

            commodityInstrumentDirection = new List<Guid>
            {
                Guid.Parse("e2eda7fb-5452-11e4-a274-d8a704927acb"),
                Guid.Parse("711e97be-53a0-11e4-a274-d8a704927acb"),
                Guid.Parse("01b0f03d-eb13-11e3-8064-00163e6000f8")
            };
        }

        public List<Guid> GetCommodityDirectionCriteria()
        {
            List<Guid> criteria = new List<Guid>();

            if (Vaz)
            {
                criteria.AddRange(CommodityVazDirection);
            }

            if (Gaz)
            {
                criteria.AddRange(CommodityGazDirection);
            }

            if (Zaz)
            {
                criteria.AddRange(CommodityZazDirection);
            }

            if (Chemistry)
            {
                criteria.AddRange(CommodityChemistryDirection);
            }

            if (Battery)
            {
                criteria.AddRange(CommodityBatteryDirection);
            }

            if (Gas)
            {
                criteria.AddRange(CommodityGasDirection);
            }

            if (Instrument)
            {
                criteria.AddRange(CommodityInstrumentDirection);
            }

            return criteria;
        }

        public List<long> GetDirectoryIds()
        {
            return SelectedDirectoryItems.Select(x => x.Id).ToList();
        } 

        #endregion

        #region Events

        public event EventHandler SearchCriteriaChanged;

        public event EventHandler DirectoryItemsChanged;

        public event EventHandler SearchCriteriaCleared;

        public event DoubleAnimationEventHandler EnabledEdvanceSearchChanged;

        public event MinWidthEventHandler EdvanceSearchWidthChanged;

        #endregion
    }
}

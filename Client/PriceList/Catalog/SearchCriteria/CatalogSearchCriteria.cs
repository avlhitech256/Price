using System;
using System.Runtime.CompilerServices;
using Common.Annotations;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using DatabaseService.DataBaseContext.Entities;

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

        #endregion

        #region Constructors

        public CatalogSearchCriteria(IMessenger messenger)
        {
            Messenger = messenger;
            edvanceSearchWidth = 0;
            enabledEdvanceSearch = false;
            FirstBrandItemEntity = new BrandItemEntity {Id = -1L, Code = Guid.NewGuid(), Name = "Все бренды"};
            Clear();
            SearchComplited();
        }

        #endregion

        #region Properties

        private IMessenger Messenger { get; }

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
                return enabledEdvanceSearch;
            }
            set
            {
                if (enabledEdvanceSearch != value)
                {
                    enabledEdvanceSearch = value;
                    OnPropertyChanged();
                    var args = value ? new DoubleAnimationEventArgs(0, 150) : new DoubleAnimationEventArgs(150, 0);
                    Messenger?.Send(CommandName.ShowAdvanceSearchControl, args);
                }
            }
        }

        public int EdvanceSearchWidth
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
                    OnPropertyChanged();
                    int minWidth = 1070 + EdvanceSearchWidth;
                    Messenger?.Send(CommandName.SetMinWidth, new MinWidthEventArgs(minWidth));
                }
            }
        }

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

        #endregion

        #region Events

        public event EventHandler SearchCriteriaChanged;

        public event EventHandler SearchCriteriaCleared;

        #endregion
    }
}

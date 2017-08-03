using System;
using System.Runtime.CompilerServices;
using Common.Annotations;
using Common.Data.Notifier;

namespace Catalog.SearchCriteria
{
    public class CatalogSearchCriteria : Notifier
    {
        #region Members

        private string code;
        private string name;
        private string article;
        private long brandId;
        private bool priceIsDown;
        private bool priceIsUp;
        private bool isNew;
        private bool isModified;
        private bool isEmpty;

        #endregion

        #region Constructors

        public CatalogSearchCriteria()
        {
            Code = string.Empty;
            Name = string.Empty;
            Article = string.Empty;
            BrandId = -1L;
            PriceIsDown = false;
            PriceIsUp = false;
            IsNew = false;
            IsEmpty = true;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            IsModified = true;
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
            BrandId = -1L;
            PriceIsDown = false;
            PriceIsUp = false;
            IsNew = false;
        }

        private bool SearchCriteriaIsEmpty()
        {
            return Code == string.Empty && 
                   Name == string.Empty &&
                   Article == string.Empty &&
                   BrandId == -1L &&
                   !PriceIsDown &&
                   !PriceIsUp &&
                   !IsNew;

        }

        public void SearchComplited()
        {
            IsModified = false;
        }

        #endregion

        #region Events

        public event EventHandler SearchCriteriaChanged;

        public event EventHandler SearchCriteriaCleared;

        #endregion
    }
}

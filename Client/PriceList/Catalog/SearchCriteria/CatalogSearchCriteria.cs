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
        }

        #endregion

        #region Properties

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

        #endregion
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.Data.Constant;
using Common.Data.Enum;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using DatabaseService.DataService.Implementation;

namespace Options.Service.Implementation
{
    public class OptionService : IOptionService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly Dictionary<string, string> optionCache;
        private readonly List<string> existOptionCache;

        #endregion

        #region Constructors

        public OptionService()
        {
            this.dataService = new DataService();
            optionCache = new Dictionary<string, string>();
            existOptionCache = new List<string>();
        }

        #endregion

        #region Properties

        public string Login
        {
            get
            {
                return GetOption(OptionName.Login);
            }
            set
            {
                SetOption(OptionName.Login, value);
            }
        }

        public string Password
        {
            get
            {
                return GetOption(OptionName.Password);
            }
            set
            {
                SetOption(OptionName.Password, value);
            }
        }

        public string OverdueAccountsReceivable
        {
            get
            {
                return GetOption(OptionName.OverdueAccountsReceivable);
            }
            set
            {
                SetOption(OptionName.OverdueAccountsReceivable, value);
            }
        }

        public string Debt
        {
            get
            {
                return GetOption(OptionName.Debt);
            }
            set
            {
                SetOption(OptionName.Debt, value);
            }
        }

        public long LastOrderNumber
        {
            get
            {
                return GetLongOption(OptionName.LastOrderNumber);
            }
            set
            {
                SetLongOption(OptionName.LastOrderNumber, value);
            }
        }

        private bool ExistShowPhotoOnMouseDoubleClickOption => ExistOption(OptionName.ShowPhotoOnMouseDoubleClick);

        public bool ShowPhotoOnMouseDoubleClick
        {
            get
            {
                if (!ExistShowPhotoOnMouseDoubleClickOption)
                {
                    SetBooleanOption(OptionName.ShowPhotoOnMouseDoubleClick, false);
                }

                return GetBooleanOption(OptionName.ShowPhotoOnMouseDoubleClick);
            }
            set
            {
                SetBooleanOption(OptionName.ShowPhotoOnMouseDoubleClick, value);
            }
        }

        public int CatalogMaximumRows
        {
            get
            {
                return GetIntOption(OptionName.CatalogMaximumRows);
            }
            set
            {
                SetIntOption(OptionName.CatalogMaximumRows, value);
            }
        }

        public double SplitterPosition
        {
            get
            {
                return GetDoubleOption(OptionName.SplitterPosition);
            }
            set
            {
                SetDoubleOption(OptionName.SplitterPosition, value);
            }
        }

        #endregion

        #region Methods

        private bool ExistOption(string optionCode)
        {
            bool value = existOptionCache.Contains(optionCode);

            if (!value)
            {
                value = ExistDbOption(optionCode);

                if (value)
                {
                    existOptionCache.Add(optionCode);
                }
            }

            return value;
        }

        private bool GetBooleanOption(string optionCode)
        {
            bool value = GetOption(optionCode) == Flag.Yes;

            return value;
        }

        private void SetBooleanOption(string optionCode, bool value)
        {
            SetOption(optionCode, value ? Flag.Yes : Flag.No);
        }

        private long GetLongOption(string optionCode)
        {
            long value;
            
            if (!long.TryParse(GetOption(optionCode), out value))
            {
                value = 0L;
                SetLongOption(optionCode, value);
            }

            return value;
        }

        private void SetLongOption(string optionCode, long value)
        {
            SetOption(optionCode, value.ToString());
        }

        private int GetIntOption(string optionCode)
        {
            int value;

            if (!int.TryParse(GetOption(optionCode), out value))
            {
                value = 0;
                SetLongOption(optionCode, value);
            }

            return value;
        }

        private void SetIntOption(string optionCode, int value)
        {
            SetOption(optionCode, value.ToString());
        }

        private void SetDoubleOption(string optionCode, double value)
        {
            SetOption(optionCode, value.ToString(CultureInfo.InvariantCulture));
        }

        private double GetDoubleOption(string optionCode)
        {
            double value;

            if (!double.TryParse(GetOption(optionCode), out value))
            {
                value = 0;
                SetDoubleOption(optionCode, value);
            }

            return value;
        }

        private string GetOption(string optionCode)
        {
            string value;

            if (!optionCache.TryGetValue(optionCode, out value))
            {
                value = GetDbOption(optionCode);

                if (value != null)
                {
                    optionCache.Add(optionCode, value);
                }
            }

            return value;
        }

        private void SetOption(string optionCode, string value)
        {
            if (!string.IsNullOrWhiteSpace(optionCode) && value != GetOption(optionCode))
            {
                if (optionCache.ContainsKey(optionCode))
                {
                    optionCache[optionCode] = value;
                }
                else
                {
                    optionCache.Add(optionCode, value);
                }

                SetDbOption(optionCode, value);
            }
        }

        private bool ExistDbOption(string optionCode)
        {
            bool value = false;

            if (!string.IsNullOrWhiteSpace(optionCode))
            {
                OptionItemEntity option = dataService.Select<OptionItemEntity>().FirstOrDefault(x => x.Code == optionCode);

                if (option != null)
                {
                    value = true;
                }
            }

            return value;
        }

        public string GetDbOption(string optionCode)
        {
            string value = string.Empty;

            if (!string.IsNullOrWhiteSpace(optionCode))
            {
                OptionItemEntity option = dataService.Select<OptionItemEntity>().FirstOrDefault(x => x.Code == optionCode);

                if (option != null)
                {
                    value = option.Value;
                }
            }

            return value;
        }

        public void SetDbOption(string optionCode, string value)
        {
            if (!string.IsNullOrWhiteSpace(optionCode))
            {
                OptionItemEntity option = dataService.Select<OptionItemEntity>().FirstOrDefault(x => x.Code == optionCode);

                if (option == null)
                {
                    option = dataService.DataBaseContext.OptionItemEntities.Create();
                    option.Code = optionCode;
                    dataService.DataBaseContext.OptionItemEntities.Add(option);
                }

                option.Value = value;
                dataService.DataBaseContext.SaveChanges();
            }
        }

        #endregion
    }
}

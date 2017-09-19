using System.Collections.Generic;
using Common.Data.Constant;
using DatabaseService.DataService;

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

        public OptionService(IDataService dataService)
        {
            this.dataService = dataService;
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

        #endregion

        #region Methods

        private bool ExistOption(string optionCode)
        {
            bool value = existOptionCache.Contains(optionCode);

            if (!value)
            {
                value = dataService.ExistOption(optionCode);

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

        private string GetOption(string optionCode)
        {
            string value;

            if (!optionCache.TryGetValue(optionCode, out value))
            {
                value = dataService.GetOption(optionCode);
                optionCache.Add(optionCode, value);
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

                dataService.SetOption(optionCode, value);
            }
        }

        #endregion
    }
}

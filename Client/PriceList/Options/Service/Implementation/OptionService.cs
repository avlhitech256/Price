﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.Data.Constant;
using Common.Data.Holders;
using Common.Service;
using Common.Service.Implementation;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using DatabaseService.DataService.Implementation;

namespace Options.Service.Implementation
{
    public class OptionService : IOptionService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly Dictionary<string, StringHolder> optionCache;
        private readonly IConvertService convertService;

        #endregion

        #region Constructors

        public OptionService()
        {
            dataService = new DataService();
            optionCache = new Dictionary<string, StringHolder>();
            convertService = new ConvertService();
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

        public bool ExistOption(string optionCode)
        {
            StringHolder option;

            bool value = optionCache.TryGetValue(optionCode, out option);

            if (!value)
            {
                option = new StringHolder(GetDbOption(optionCode, out value));

                if (value)
                {
                    optionCache.Add(optionCode, option);
                }
            }

            return value;
        }

        public bool GetBooleanOption(string optionCode)
        {
            bool value = GetOption(optionCode) == Flag.Yes;

            return value;
        }

        public Dictionary<string, bool> GetBooleanOptions(IEnumerable<string> optionCodes)
        {
            var result = new Dictionary<string, bool>();
            optionCodes.ToList().ForEach(x => result.Add(x, GetBooleanOption(x)));
            return result;
        }

        public void SetBooleanOption(string optionCode, bool value)
        {
            SetOption(optionCode, value ? Flag.Yes : Flag.No);
        }

        public void SetBooleanOptions(Dictionary<string, bool> options)
        {
            foreach (KeyValuePair<string, bool> item in options)
            {
                SetBooleanOption(item.Key, item.Value);
            }
        }

        public int GetIntOption(string optionCode)
        {
            int value;

            if (!int.TryParse(GetOption(optionCode), out value))
            {
                value = 0;
                SetLongOption(optionCode, value);
            }

            return value;
        }

        public Dictionary<string, int> GetIntOptions(IEnumerable<string> optionCodes)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            optionCodes.ToList().ForEach(x => result.Add(x, GetIntOption(x)));
            return result;
        }

        public void SetIntOption(string optionCode, int value)
        {
            SetOption(optionCode, value.ToString());
        }

        public void SetIntOptions(Dictionary<string, int> options)
        {
            foreach (KeyValuePair<string, int> item in options)
            {
                SetIntOption(item.Key, item.Value);
            }
        }

        public long GetLongOption(string optionCode)
        {
            long value;
            
            if (!long.TryParse(GetOption(optionCode), out value))
            {
                value = 0L;
                SetLongOption(optionCode, value);
            }

            return value;
        }

        public Dictionary<string, long> GetLongOptions(IEnumerable<string> optionCodes)
        {
            var result = new Dictionary<string, long>();
            optionCodes.ToList().ForEach(x => result.Add(x, GetLongOption(x)));
            return result;
        }

        public void SetLongOption(string optionCode, long value)
        {
            SetOption(optionCode, value.ToString());
        }

        public void SetLongOptions(Dictionary<string, long> options)
        {
            foreach (KeyValuePair<string, long> item in options)
            {
                SetLongOption(item.Key, item.Value);
            }
        }

        public double GetDoubleOption(string optionCode)
        {
            double value;

            if (!convertService.ConvertToDouble(GetOption(optionCode), out value))
            {
                value = 0;
                SetDoubleOption(optionCode, value);
            }

            return value;
        }

        public Dictionary<string, double> GetDoubleOptions(IEnumerable<string> optionCodes)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            optionCodes.ToList().ForEach(x => result.Add(x, GetDoubleOption(x)));
            return result;
        }

        public void SetDoubleOption(string optionCode, double value)
        {
            SetOption(optionCode, value.ToString(CultureInfo.CurrentCulture));
        }

        public void SetDoubleOptions(Dictionary<string, double> options)
        {
            foreach (KeyValuePair<string, double> item in options)
            {
                SetDoubleOption(item.Key, item.Value);
            }
        }

        public string GetOption(string optionCode)
        {
            string option;
            StringHolder value;

            if (optionCache.TryGetValue(optionCode, out value))
            {
                option = value.Value;
            }
            else
            {
                bool existOption;
                option = GetDbOption(optionCode, out existOption);

                if (existOption)
                {
                    value = new StringHolder(option);
                    optionCache.Add(optionCode, value);
                }
            }

            return option;
        }

        public Dictionary<string, string> GetOption(IEnumerable<string> optionCodes)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            optionCodes.ToList().ForEach(x => result.Add(x, GetOption(x)));
            return result;
        }

        public void SetOption(string optionCode, string value)
        {
            string oldValue = GetOption(optionCode);

            if (!string.IsNullOrWhiteSpace(optionCode) && 
                (value != oldValue || (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(oldValue))))
            {
                StringHolder option;

                if (optionCache.TryGetValue(optionCode, out option))
                {
                    if (option == null)
                    {
                        option = new StringHolder(value);
                        optionCache[optionCode] = option;
                    }
                    else
                    {
                        option.Value = value;
                    }
                }
                else
                {
                    option = new StringHolder(value);
                    optionCache.Add(optionCode, option);
                }

                SetDbOption(optionCode, value);
            }
        }

        public void SetOptions(Dictionary<string, string> options)
        {
            foreach (KeyValuePair<string, string> item in options)
            {
                SetOption(item.Key, item.Value);
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

        public string GetDbOption(string optionCode, out bool existOption)
        {
            string value = string.Empty;
            existOption = false;

            if (!string.IsNullOrWhiteSpace(optionCode))
            {
                try
                {
                    OptionItemEntity option = dataService.Select<OptionItemEntity>().FirstOrDefault(x => x.Code == optionCode);

                    if (option != null)
                    {
                        value = option.Value;
                        existOption = true;
                    }
                }
                catch (Exception e)
                {

                    ;//throw;
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

using System.Collections.Generic;
using System.Linq;
using Common.Data.Constant;
using Options.Service;
using Options.Service.Implementation;

namespace Template.Service.Implementation
{
    public class TemplateService : ITemplateService
    {
        private readonly IOptionService optionService;
        private readonly List<string> catalogOptionName; 

        public TemplateService(IOptionService optionService)
        {
            this.optionService = optionService;
            catalogOptionName = new List<string>
            {
                CatalogColumnNames.NumberColumnName,
                CatalogColumnNames.CodeColumnName,
                CatalogColumnNames.ArticleColumn,
                CatalogColumnNames.NameColumn,
                CatalogColumnNames.BrandColumn,
                CatalogColumnNames.UnitColumn,
                CatalogColumnNames.EnterpriceNormPackColumn,
                CatalogColumnNames.BatchOfSalesColumn,
                CatalogColumnNames.BalanceColumn,
                CatalogColumnNames.PriceColumn,
                CatalogColumnNames.CountColumn
            };
        }

        public bool IsExistCatalogTemplate(bool isAdvanceSearch)
        {
            string optionName = isAdvanceSearch
                ? PrefixOptions.Advance + catalogOptionName.FirstOrDefault() + PrefixOptions.Width
                : catalogOptionName.FirstOrDefault() + PrefixOptions.Width;
            bool existOption = optionService.ExistOption(optionName);
            return existOption;
        }

        public CatalogTemplate GetCatalogTemplate(bool isAdvanceSearch)
        {
            var options = new Dictionary<string, string>();
            catalogOptionName.ForEach(x => options.Add(isAdvanceSearch 
                                                           ? PrefixOptions.Advance + x + PrefixOptions.Width 
                                                           : x + PrefixOptions.Width, x));
            var template = new CatalogTemplate();
            template.IsAdvanceSearch = isAdvanceSearch;
            Dictionary<string, double> columnsWidth = optionService.GetDoubleOptions(options.Keys);
            template.GridColumnsWidth = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> item in columnsWidth)
            {
                string columnName;

                if (options.TryGetValue(item.Key, out columnName))
                {
                    template.GridColumnsWidth.Add(columnName, item.Value);
                }
            }

            return template;
        }

        public void SetCatalogTemplate(CatalogTemplate template)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            catalogOptionName.ForEach(x => options.Add(x, template.IsAdvanceSearch 
                                                              ? PrefixOptions.Advance + x + PrefixOptions.Width 
                                                              : x + PrefixOptions.Width));
            var columnsWidth = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> item in template.GridColumnsWidth)
            {
                string optionName;

                if (options.TryGetValue(item.Key, out optionName))
                {
                    columnsWidth.Add(optionName, item.Value);
                }
            }

            optionService.SetDoubleOptions(columnsWidth);
        }
    }
}

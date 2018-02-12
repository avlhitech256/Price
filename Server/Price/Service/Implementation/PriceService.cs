using System.Data.Entity;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Objects;
using DataBase.Service;

namespace Price.Service.Implementation
{
    public class PriceService : IPriceService
    {
        #region Members

        private readonly IDataService dataService;

        #endregion

        #region Constructors

        public PriceService(IDataService dataService)
        {
            this.dataService = dataService;
        }

        #endregion

        #region Methods

        public PriceInfo GetPrice(CatalogItemEntity catalogItem, string login)
        {
            ContragentItemEntity contragentItem = GetContagent(login);
            PriceInfo result = GetPrice(catalogItem, contragentItem);

            return result;
        }

        private ContragentItemEntity GetContagent(string login)
        {
            return dataService.DataBaseContext.ContragentItemEntities
                .Include(x => x.Discounts)
                .Include(x => x.PriceTypePriceGroups)
                .Include(x => x.PriceTypeNomenclatureGroups)
                .FirstOrDefault(x => x.Login == login);
        }

        private PriceInfo GetPrice(CatalogItemEntity catalogItem, ContragentItemEntity contragentItem)
        {
            var result = new PriceInfo();

            if (catalogItem != null && contragentItem != null)
            {
                TypeOfPriceItemEntity typeOfPrice = GetTypeOfPrice(catalogItem, contragentItem);
                result = GetPrice(catalogItem, typeOfPrice);
                decimal rate = GetRate(catalogItem, contragentItem);
                result.Price += result.Price + rate / 100;
            }

            return result;
        }

        private decimal GetRate(CatalogItemEntity catalogItem, ContragentItemEntity contragentItem)
        {
            decimal rate = 0;

            if (contragentItem.Discounts != null)
            {
                DiscountsContragentEntity discount =
                    contragentItem.Discounts.FirstOrDefault(x => x.CatalogItem.Id == catalogItem.Id);

                if (discount != null)
                {
                    rate = discount.Rate;
                }
            }

            return rate;
        }

        private TypeOfPriceItemEntity GetTypeOfPrice(CatalogItemEntity catalogItem, ContragentItemEntity contragentItem)
        {
            TypeOfPriceItemEntity typeOfPrice = null;

            PriceTypeNomenclatureGroupContragentEntity priceTypeNomenclatureGroupContragent =
                GetPriceTypeNomenclatureGroup(catalogItem, contragentItem);

            if (priceTypeNomenclatureGroupContragent != null)
            {
                typeOfPrice = priceTypeNomenclatureGroupContragent.TypeOfPriceItem;
            }
            else
            {
                PriceTypePriceGroupContragentEntity priceTypePriceGroup = 
                    GetPriceTypePriceGroup(catalogItem, contragentItem);

                if (priceTypePriceGroup != null)
                {
                    typeOfPrice = priceTypePriceGroup.TypeOfPriceItem;
                }
            }

            return typeOfPrice;
        }

        private PriceTypePriceGroupContragentEntity GetPriceTypePriceGroup(
            CatalogItemEntity catalogItem, ContragentItemEntity contragentItem)
        {
            PriceTypePriceGroupContragentEntity priceTypePriceGroup = null;

            PriceGroupItemEntity priceGroup = catalogItem.PriceGroup;

            if (priceGroup != null && contragentItem.PriceTypePriceGroups != null)
            {
                priceTypePriceGroup =
                    contragentItem
                        .PriceTypePriceGroups.FirstOrDefault(x => x.PriceGroupItem.Id == priceGroup.Id);
            }

            return priceTypePriceGroup;
        }

        private PriceTypeNomenclatureGroupContragentEntity GetPriceTypeNomenclatureGroup(
            CatalogItemEntity catalogItem, ContragentItemEntity contragentItem)
        {
            PriceTypeNomenclatureGroupContragentEntity priceTypeNomenclatureGroupContragent = null;
            NomenclatureGroupEntity nomenclatureGroup = catalogItem.NomenclatureGroup;

            if (nomenclatureGroup != null && contragentItem.PriceTypeNomenclatureGroups != null)
            {
                priceTypeNomenclatureGroupContragent =
                    contragentItem
                        .PriceTypeNomenclatureGroups.FirstOrDefault(
                            x => x.NomenclatureGroupItem.Id == nomenclatureGroup.Id);
            }

            return priceTypeNomenclatureGroupContragent;
        }

        private PriceInfo GetPrice(CatalogItemEntity catalogItem, TypeOfPriceItemEntity typeOfPrice)
        {
            var result = new PriceInfo();

            if (typeOfPrice != null)
            {
                TypeOfPricesNomenclatureItemEntity typeOfPricesNomenclature =
                                catalogItem.TypeOfPriceItems.FirstOrDefault(x => x.Id == typeOfPrice.Id);

                if (typeOfPricesNomenclature != null)
                {
                    result.Price = typeOfPricesNomenclature.Price;
                    result.Currency = typeOfPricesNomenclature.Currency;
                }
            }

            return result;
        }

        #endregion
    }
}

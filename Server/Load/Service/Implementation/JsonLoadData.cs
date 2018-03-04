using System.Collections.Generic;
using Json.Contract;

namespace Load.Service.Implementation
{
    public class JsonLoadData
    {
        #region Constructors

        public JsonLoadData()
        {
            Catalogs = new List<Directory>();
            PriceGroups = new List<PriceGroup>();
            NomenclatureGroups = new List<NomenclatureGroup>();
            Brands = new List<Brand>();
            TypesOfPrices = new List<PriceType>();
            CommodityDirections = new List<CommodityDirection>();
            Nomenclatures = new List<Nomenclature>();
            Clients = new List<Client>();
        }

        #endregion

        #region Properties

        public List<Directory> Catalogs { get; set; }

        public List<PriceGroup> PriceGroups { get; set; }

        public List<NomenclatureGroup> NomenclatureGroups { get; set; }

        public List<Brand> Brands { get; set; }

        public List<PriceType> TypesOfPrices { get; set; }

        public List<CommodityDirection> CommodityDirections { get; set; }

        public List<Nomenclature> Nomenclatures { get; set; }

        public List<Client> Clients { get; set; }

        #endregion

        #region Methods

        public void AddMetaData(MetaData metaData)
        {
            if (metaData != null)
            {
                if (metaData.Catalog != null)
                {
                    Catalogs.AddRange(metaData.Catalog);
                }

                if (metaData.PriceGroups != null)
                {
                    PriceGroups.AddRange(metaData.PriceGroups);
                }

                if (metaData.NomenclatureGroups != null)
                {
                    NomenclatureGroups.AddRange(metaData.NomenclatureGroups);
                }

                if (metaData.Brands != null)
                {
                    Brands.AddRange(metaData.Brands);
                }

                if (metaData.TypesOfPrices != null)
                {
                    TypesOfPrices.AddRange(metaData.TypesOfPrices);
                }

                if (metaData.CommodityDirections != null)
                {
                    CommodityDirections.AddRange(metaData.CommodityDirections);
                }
            }
        }

        public void AddPriceList(PriceList priceList)
        {
            Nomenclatures.AddRange(priceList.Nomenclature);
        }

        public void AddClient(Clients clients)
        {
            Clients.AddRange(clients.Contragent);
        }

        #endregion
    }
}

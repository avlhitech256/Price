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
            Catalogs.AddRange(metaData.Catalog);
            PriceGroups.AddRange(metaData.PriceGroups);
            NomenclatureGroups.AddRange(metaData.NomenclatureGroups);
            Brands.AddRange(metaData.Brands);
            TypesOfPrices.AddRange(metaData.TypesOfPrices);
            CommodityDirections.AddRange(metaData.CommodityDirections);
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

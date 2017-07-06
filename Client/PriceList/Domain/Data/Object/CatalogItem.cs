using System;
using System.Globalization;

namespace Domain.Data.Object
{
    public class CatalogItem
    {
        public long Id { get; set; }

        public long Position { get; set; }

        public string Code { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public BrandItem Brand { get; set; }

        public string Unit { get; set; }

        public string EnterpriceNormPack { get; set; }

        public string Balance { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public byte[] Photo { get; set; }

        public string FullPrice => Price.ToString(CultureInfo.InvariantCulture) + " " + Currency;
    }
}

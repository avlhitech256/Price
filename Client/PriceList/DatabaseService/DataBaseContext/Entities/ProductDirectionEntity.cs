using Common.Data.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class ProductDirectionEntity
    {
        public long Id { get; set; }

        public CommodityDirection Direction { get; set; }

        public virtual DirectoryEntity Directory { get; set; }
    }
}

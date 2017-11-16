using DataBase.Context.Object;

namespace DataBase.Context.Entities
{
    public class ProductDirectionEntity
    {
        public long Id { get; set; }

        public CommodityDirection Direction { get; set; }

        public virtual DirectoryEntity Directory { get; set; }
    }
}

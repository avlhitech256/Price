using DataBase.Context.Entities;
using DataBase.Objects;

namespace Price.Service
{
    public interface IPriceService
    {
        PriceInfo GetPrice(CatalogItemEntity catalogItem, string login);
    }
}

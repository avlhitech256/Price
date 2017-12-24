using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public class CatalogRepository : Repository<CatalogItemEntity>, ICatalogRepository
    {
        #region Constructors

        public CatalogRepository(IDataService dataService) : base(dataService)
        {
        }

        #endregion

        #region Methods

        protected override long GetId(CatalogItemEntity entity)
        {
            return entity.Id;
        }

        public override void Load(IEnumerable<long> ids)
        {
            Clear();
            DataService.Select<CatalogItemEntity>().Where(x => ids.Contains(x.Id)).ToList().ForEach(
                x =>
                {
                    RepositoryItems.Add(x.Id, x);
                });
        }

        #endregion
    }
}

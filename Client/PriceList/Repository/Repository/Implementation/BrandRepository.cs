using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public class BrandRepository : Repository<BrandItemEntity>, IBrandRepository
    {
        #region Constructors

        public BrandRepository(IDataService dataService) : base(dataService) { }

        #endregion

        #region Methods

        protected override long GetId(BrandItemEntity entity)
        {
            return entity.Id;
        }

        public override void Load()
        {
            Clear();
            DataService.Select<BrandItemEntity>().ToList().ForEach(
                x =>
                {
                    RepositoryItems.Add(x.Id, x);
                });
        }

        public override void Load(IEnumerable<long> ids)
        {
            Clear();
            DataService.Select<BrandItemEntity>().Where(x => ids.Contains(x.Id)).ToList().ForEach(
                x =>
                {
                    RepositoryItems.Add(x.Id, x);
                });
        }
        
        #endregion
    }
}

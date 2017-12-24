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

        #endregion
    }
}

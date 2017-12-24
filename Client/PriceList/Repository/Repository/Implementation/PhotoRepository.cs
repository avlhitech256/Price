using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public class PhotoRepository : Repository<PhotoItemEntity>, IPhotoRepository
    {
        #region Constructors

        public PhotoRepository(IDataService dataService) : base(dataService)
        {
        }

        #endregion

        #region Methods

        protected override long GetId(PhotoItemEntity entity)
        {
            return entity.Id;
        }

        public override void Load(IEnumerable<long> ids)
        {
            Clear();
            DataService.Select<PhotoItemEntity>().Where(x => ids.Contains(x.Id)).ToList().ForEach(
                x =>
                {
                    RepositoryItems.Add(x.Id, x);
                });
        }

        #endregion
    }
}

using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public class DirectoryRepository : Repository<DirectoryEntity>, IDirectoryRepository
    {
        #region Constructors

        public DirectoryRepository(IDataService dataService) : base(dataService) { }

        #endregion

        #region Methods

        protected override long GetId(DirectoryEntity entity)
        {
            return entity.Id;
        }

        #endregion
    }
}

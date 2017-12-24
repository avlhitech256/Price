using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Members

        protected readonly List<TEntity> RepositoryItems;
        protected readonly IDataService DataService;

        #endregion

        #region Constructors

        protected Repository(IDataService dataService)
        {
            this.DataService = dataService;
            RepositoryItems = new List<TEntity>();
        }

        #endregion

        #region Methods

        protected abstract long GetId(TEntity entity);

        public TEntity GetItem(long? id)
        {
            return id.HasValue ? GetItem(id.Value) : null;
        }

        public TEntity GetItem(long id)
        {
            TEntity entity = RepositoryItems.FirstOrDefault(x => GetId(x) == id);

            if (entity == null)
            {
                entity = DataService.Find<TEntity>(id);

                if (entity != null)
                {
                    RepositoryItems.Add(entity);
                }
            }

            return entity;
        }

        public IList<TEntity> GetItems()
        {
            return RepositoryItems.AsReadOnly();
        }

        public void Add(TEntity item)
        {
            RepositoryItems.Add(item);
        }

        public void Clear()
        {
            RepositoryItems.Clear();
        }

        public void Load()
        {
            Clear();
            RepositoryItems.AddRange(DataService.Select<TEntity>());
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataService;

namespace Repository.Repository.Implementation
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Members

        protected readonly Dictionary<long, TEntity> RepositoryItems;
        protected readonly IDataService DataService;

        #endregion

        #region Constructors

        protected Repository(IDataService dataService)
        {
            this.DataService = dataService;
            RepositoryItems = new Dictionary<long, TEntity>();
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
            TEntity entity;

            if (!RepositoryItems.TryGetValue(id, out entity))
            {
                entity = DataService.Find<TEntity>(id);
                RepositoryItems.Add(id, entity);
            }

            return entity;
        }

        public IEnumerable<TEntity> GetItems()
        {
            return RepositoryItems.Values;
        }

        public void Add(TEntity item)
        {
            if (item != null)
            {
                TEntity entity;

                if (RepositoryItems.TryGetValue(GetId(item), out entity))
                {
                    RepositoryItems[GetId(item)] = item;
                }
                else
                {
                    RepositoryItems.Add(GetId(item), item);
                }
            }
        }

        public void Clear()
        {
            RepositoryItems.Clear();
        }

        public void Load()
        {
            Clear();
            DataService.Select<TEntity>().ToList().ForEach(
                x =>
                {
                    RepositoryItems.Add(GetId(x), x);
                });
        }

        public abstract void Load(IEnumerable<long> ids);

        #endregion
    }
}

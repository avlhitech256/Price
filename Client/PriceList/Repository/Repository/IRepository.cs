using System.Collections.Generic;

namespace Repository.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetItem(long? id);

        TEntity GetItem(long id);

        IList<TEntity> GetItems();

        void Add(TEntity item);

        void Clear();

        void Load();
    }
}

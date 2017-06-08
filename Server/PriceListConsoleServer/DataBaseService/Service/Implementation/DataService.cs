using System.Data.Entity;
using DataBaseService.Model;

namespace DataBaseService.Service.Implementation
{
    public class DataService : IDataService
    {
        #region Members

        #endregion

        #region Constructors

        public DataService()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBContext>());
            DBContext = new DBContext();
            DBContext.Database.Connection.Open();
        }

        #endregion

        #region Properties

        public DBContext DBContext { get; }

        #endregion

        #region Methods

        #endregion
    }
}

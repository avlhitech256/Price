using System.Data.Entity;

namespace DatabaseService.DataService.Implementation
{
    public class DataService : IDataService
    {
        public DataService()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataBaseContext.DataBaseContext>());
            DataBaseContext = new DataBaseContext.DataBaseContext();
        }

        public DataBaseContext.DataBaseContext DataBaseContext { get; }
    }
}

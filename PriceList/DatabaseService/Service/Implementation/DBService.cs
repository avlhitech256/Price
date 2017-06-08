using DatabaseService.Model;

namespace DatabaseService.Service.Implementation
{
    public class DBService
    {
        public DBService()
        {
            DBContext = new DBContext();
        }

        public DBContext DBContext { get; }
    }
}

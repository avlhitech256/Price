using DataBaseService.Model;

namespace DataBaseService.Service
{
    public interface IDataService
    {
        DBContext DBContext { get; }
    }
}

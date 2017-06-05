using System.Security.Cryptography.X509Certificates;
using DataService.Model;

namespace DataService.Service
{
    public interface IDataService
    {
        DBContext DBContext { get; }
        void Open();
    }
}

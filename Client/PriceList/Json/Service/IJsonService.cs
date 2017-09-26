using System.IO;
using Json.Contract;

namespace Json.Service
{
    public interface IJsonService
    {
        Clients ConvertToClients(MemoryStream stream);

        Clients ConvertToClients(string json);

        MetaData ConvertToMetaData(MemoryStream stream);

        MetaData ConvertToMetaData(string json);

        PriceList ConvertToPriceList(MemoryStream stream);

        PriceList ConvertToPriceList(string json);

        T Convert<T>(MemoryStream stream) where T : class, new ();

        T Convert<T>(string json) where T : class, new ();
    }
}

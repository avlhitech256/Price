using System.IO;
using JSON.Contract;

namespace JSON.Service
{
    interface IJsonService
    {
        Clients ConvertToClients(MemoryStream stream);

        Clients ConvertToClients(string json);

        MetaData ConvertToMetaData(MemoryStream stream);

        MetaData ConvertToMetaData(string json);

        T Convert<T>(MemoryStream stream) where T : class;

        T Convert<T>(string json) where T : class;
    }
}

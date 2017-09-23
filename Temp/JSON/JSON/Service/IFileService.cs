using System.IO;
using JSON.Contract;

namespace JSON.Service
{
    public interface IFileService
    {
        MemoryStream ReadFile(string fileName);

        T ReadJsonFile<T>(string fileName) where T : class;

        Clients ReadClients(string fileName);

        MetaData ReadMetaData(string fileName);

        PriceList ReadPriceList(string fileName);
    }
}

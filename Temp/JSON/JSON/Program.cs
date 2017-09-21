using System.IO;
using JSON.Contract;
using JSON.Service;
using JSON.Service.Implementation;

namespace JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileService fileService = new FileService();
            IJsonService jsonService = new JsonService();
            string metaDataFileName = "MetaData.json";
            string clientsFileName = "Clients.json";
            MetaData metaData;
            Clients clients;

            using (MemoryStream stream = fileService.ReadFile(metaDataFileName))
            {
                metaData = jsonService.ConvertToMetaData(stream);
            }

            using (MemoryStream stream = fileService.ReadFile(clientsFileName))
            {
                clients = jsonService.ConvertToClients(stream);
            }
        }
    }
}

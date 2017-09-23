using System.Collections.Generic;
using System.Linq;
using JSON.Contract;
using JSON.Service;
using JSON.Service.Implementation;

namespace JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            IJsonService jsonService = new JsonService();
            IFileService fileService = new FileService(jsonService);
            string metaDataFileName = "MetaData.json";
            string clientsFileName = "Clients.json";
            string priceFileName = "PriceList.json";
            MetaData metaData = fileService.ReadMetaData(metaDataFileName);
            Clients clients = fileService.ReadClients(clientsFileName);
            PriceList pricelist = fileService.ReadPriceList(priceFileName);
            List<string> clientCodes = clients.Contragent.Select(x => x.Code).ToList(); 
            List<string> priceListCodes = pricelist.Nomenclature.Select(x => x.Code).ToList();
        }
    }
}

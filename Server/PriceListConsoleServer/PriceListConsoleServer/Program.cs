using System;
using DataBaseService.Model;

namespace PriceListConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBaseService.Service.Implementation.DataService dataService = 
                new DataBaseService.Service.Implementation.DataService();
            Client client = dataService.DBContext.Client.Create();
            client.UID = Guid.NewGuid();
            dataService.DBContext.Client.Add(client);
            dataService.DBContext.SaveChanges();
        }
    }
}

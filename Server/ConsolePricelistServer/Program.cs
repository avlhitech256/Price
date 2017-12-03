using System;
using System.ServiceModel;
using DataBase.Context.Entities;
using DataBase.Service;
using DataBase.Service.Implementation;

namespace ConsolePricelistServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Pricelist Application Server";
            Console.WriteLine("[{0}] Servrer is started", DateTimeOffset.Now);
            IDataService dataService = new DataService();
            BrandItemEntity entity = null;

            try
            {
                entity = dataService.DataBaseContext.BrandItemEntities.Find(1);
                //List<BrandItemEntity> list = dataService.DataBaseContext.BrandItemEntities.ToList();
                //entity = dataService.DataBaseContext.BrandItemEntities.FirstOrDefault(x => x.Id == 1);
            }
            catch (Exception e)
            {
                ;
            }

            Console.WriteLine("Entity Name: " + entity?.Name);

            using (var host = new ServiceHost(typeof (PricelistService.Service.Implementation.PricelistService)))
            {
                host.Open();
                Console.ReadKey();
            }
        }
    }
}

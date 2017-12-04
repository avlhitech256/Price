using System;
using System.ServiceModel;

namespace ConsolePricelistServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Pricelist Application Server";
            Console.WriteLine("[{0}] Servrer is started", DateTimeOffset.Now);

            using (var host = new ServiceHost(typeof (PricelistService.Service.Implementation.PricelistService)))
            {
                host.Open();
                Console.ReadKey();
            }
        }
    }
}

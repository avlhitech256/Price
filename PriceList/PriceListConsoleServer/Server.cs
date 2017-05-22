using System;
using System.ServiceModel;
using Common.ServiceContract;


namespace PriceListConsoleServer
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.Title = "PriceList Application Server";

            Uri address = new Uri("http://localhost:48057/IPriceList");
            BasicHttpBinding binding = new BasicHttpBinding();
            Type contract = typeof (IPriceList);

            ServiceHost host = new ServiceHost(typeof(PriceListService.PriceListService));
            host.AddServiceEndpoint(contract, binding, address);
            host.Open();
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Modifiers != ConsoleModifiers.Control || keyInfo.Key == ConsoleKey.C)
            {
                keyInfo = Console.ReadKey();
            }
        }
    }
}

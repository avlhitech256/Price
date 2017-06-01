using System;
using System.Linq;
using System.ServiceModel;
using DataService.Model;


namespace PriceListConsoleServer
{
    class Server
    {
        static void Main(string[] args)
        {
            DBContext context =  new DBContext();
            Client client = context.Client.Create();
            context.Client.Add(client);
            context.SaveChanges();
            Console.Title = "PriceList Application Server";
            Console.WriteLine("Servrer is started");

            //Uri address = new Uri("http://IT-1:4000/PriceList");

            //Console.WriteLine("Address was created.");
            //Console.WriteLine("Uri = {0}", address.AbsoluteUri);

            //BasicHttpBinding binding = new BasicHttpBinding();

            //Console.WriteLine("Binding was created.");
            //Console.WriteLine("Binding = ", binding.ToString());

            //Type contract = typeof (IPriceList);

            //Console.WriteLine("Contract was created.");
            //Console.WriteLine("Contract = {0}", contract.ToString());

            ServiceHost host = new ServiceHost(typeof(PriceListService.Service.Implementation.PriceListService)); //, address);

            Console.WriteLine("Host war created.");
            Console.WriteLine("Host = {0}", host?.BaseAddresses?.FirstOrDefault()?.Host);

            //host.AddServiceEndpoint(contract, binding, "");

            Console.WriteLine("EndPoint was added to host.");
            Console.WriteLine("EndPoint = {0}", host?.BaseAddresses?.FirstOrDefault()?.AbsoluteUri);
            host.Open();
            Console.WriteLine("Host is open.");
            Console.WriteLine("Server is runing.");
            Console.WriteLine("Press [Ctrl-C] to stop server");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Modifiers != ConsoleModifiers.Control || keyInfo.Key == ConsoleKey.C)
            {
                keyInfo = Console.ReadKey();
            }
        }
    }
}

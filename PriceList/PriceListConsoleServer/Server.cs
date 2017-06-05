using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using Common.Domain;
using DataService.Model;
using LogService.Service;
using LogService = LogService.Service.Implementation.LogService;


namespace PriceListConsoleServer
{
    class Server
    {
        private static DBContext context;
        private static ILogService logService;
        private static Timer timer;
        static void Main(string[] args)
        {
            logService = new global::LogService.Service.Implementation.LogService();
            Console.Title = "PriceList Application Server";
            Console.WriteLine("");
            logService.SendMessage("Servrer is started", MessageType.Info, MessageLevel.High);
            
            try
            {
                context = new DBContext();
                context.Database.Connection.Open();
                while (context.Database.Connection.State == ConnectionState.Connecting ||
                       context.Database.Connection.State == ConnectionState.Executing ||
                       context.Database.Connection.State == ConnectionState.Fetching) {}
                if (context.Database.Connection.State == ConnectionState.Open)
                {
                    logService.SendMessage($"State = \"{context.Database.Connection.State}\"");
                }
                else
                {
                    
                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                throw;
            }
            Client client = context.Client.Create();
            context.Client.Add(client);
            context.SaveChanges();

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

            Console.WriteLine("Host was created.");
            Console.WriteLine("Host = {0}", host?.BaseAddresses?.FirstOrDefault()?.Host);

            //host.AddServiceEndpoint(contract, binding, "");

            Console.WriteLine("EndPoint was added to host.");
            Console.WriteLine("EndPoint = {0}", host?.BaseAddresses?.FirstOrDefault()?.AbsoluteUri);
            host.Open();
            Console.WriteLine("Host is open.");
            Console.WriteLine("Server is runing.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press [Ctrl-C] to stop server");
            Console.ForegroundColor = color;
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Modifiers != ConsoleModifiers.Control || keyInfo.Key == ConsoleKey.C)
            {
                keyInfo = Console.ReadKey();
            }
        }
    }
}

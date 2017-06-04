using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using DataService.Model;


namespace PriceListConsoleServer
{
    class Server
    {
        private static DBContext context;
        static void Main(string[] args)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.Title = "PriceList Application Server";
            Console.WriteLine("");
            Console.WriteLine("Servrer is started");
            Console.WriteLine("");
            Console.WriteLine("Connect to/create DataBse.");
            
            try
            {
                context = new DBContext();
                Console.WriteLine("SQL-Server Name = \"{0}\"", context.Database.Connection.DataSource);
                Console.WriteLine("SQL-Server Version = \"{0}\"", context.Database.Connection.ServerVersion);
                Console.WriteLine("DataBase Name = \"{0}\"", context.Database.Connection.Database);
                context.Database.Connection.Open();
                while (context.Database.Connection.State == ConnectionState.Connecting ||
                       context.Database.Connection.State == ConnectionState.Executing ||
                       context.Database.Connection.State == ConnectionState.Fetching) {}
                if (context.Database.Connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("State = \"{0}\"", context.Database.Connection.State);
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

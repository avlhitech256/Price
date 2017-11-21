using System.ServiceProcess;

namespace PricelistLoadService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PricelistLoaderService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

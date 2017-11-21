using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Load.Service;
using Load.Service.Implementation;

namespace ConsolePricelistLoader
{
    class Program
    {
        #region Members

        private static IDownLoadService downLoadService;
        private static string sourcePath;
        private static string destinationPath;
        private static string[] searchPatterns;

        #endregion

        #region Constructors

        static void Main(string[] args)
        {
            sourcePath = "In\\";
            destinationPath = "Out\\";
            searchPatterns = new[] { "Clients*.json", "MetaData*.json", "PriceList*.json", "RTiU*.json" };
            downLoadService = new DownLoadService(600000, 1000, 1000, sourcePath, destinationPath, searchPatterns);
            downLoadService.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        #endregion
    }
}

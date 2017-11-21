using System;
using System.Collections.Generic;
using File.Service.Implementation;
using Load.Service;
using Load.Service.Implementation;
using Option.Service;
using Options.Service.Implementation;

namespace ConsolePricelistLoader
{
    class Program
    {
        #region Members

        private static IDownLoadService downLoadService;
        private static string sourcePath;
        private static string destinationPath;
        private static string[] searchPatterns;
        private static List<MovingQueueItem> movingQueue;

        #endregion

        #region Constructors

        static void Main(string[] args)
        {
            IOptionService optionService = new OptionService();

            movingQueue = new List<MovingQueueItem>
            {
                new MovingQueueItem(optionService.SourcePath, optionService.DestinationPath, new[] { "Clients*.json", "MetaData*.json", "PriceList*.json", "RTiU*.json"}),
                new MovingQueueItem("In\\Photo\\", "Out\\Photo\\", new[] { "*.jpeg"})
            };

            downLoadService = new DownLoadService(600000, 1000, 1000, movingQueue);
            downLoadService.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        #endregion
    }
}

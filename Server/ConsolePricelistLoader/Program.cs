using System;
using System.Collections.Generic;
using File.Service.Implementation;
using Load.Service;
using Load.Service.Implementation;
using Option.Service;
using Option.Service.Implementation;

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
                new MovingQueueItem(optionService.SourcePath, 
                                    optionService.WorkingSourcePath, 
                                    optionService.SourcePatterns),
                new MovingQueueItem(optionService.SourcePath + optionService.SubDirForPhoto,
                                    optionService.WorkingSourcePath + optionService.SubDirForPhoto, 
                                    optionService.PhotoPatterns)
            };

            downLoadService = new DownLoadService(600000, 1000, 1000, movingQueue, optionService);
            downLoadService.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        #endregion
    }
}

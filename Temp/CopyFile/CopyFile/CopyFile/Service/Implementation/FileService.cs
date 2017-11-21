using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyFile.Service.Implementation
{
    public class FileService : IFileService
    {
        public string[] GetFileNames(string dirPath, string searchPattern = null)
        {
            string[] fileNames =
            GetFileInfos(dirPath, searchPattern)
                .Where(f => f.Exists)
                .Select(x => x.Name)
                .ToArray();

            return fileNames;
        }

        public FileInfo[] GetFileInfos(string dirPath, string searchPattern = null)
        {
            FileInfo[] fileInfos;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                fileInfos = string.IsNullOrWhiteSpace(searchPattern)
                    ? dirInfo.GetFiles()
                    : dirInfo.GetFiles(searchPattern);

            }
            catch (Exception)
            {
                fileInfos = new FileInfo[0];
            }

            return fileInfos;
        }

        public string[] MoveFiles(string inPath, string outPath, string searchPattern = null)
        {
            List<string> result = new List<string>();
            string[] fileNames = GetFileNames(inPath, searchPattern);

            foreach (string fileName in fileNames)
            {
                try
                {
                    File.Move(inPath + fileName, outPath + fileName);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Moved file: " + fileName, DateTime.Now);
                    Console.WriteLine("-------------------------------------------------------------------------");
                }
                catch (Exception e)
                {

                    if (e.HResult == -2147024864) // Файл захвачен другим процессом
                    {
                        //result.Add(fileName);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        //throw;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }

                    //Console.WriteLine("File Name: " + fileName);
                    //Console.WriteLine("Exception Type: " + e.GetType().FullName);
                    //Console.WriteLine("HResult:" + e.HResult);
                    //Console.WriteLine("Message: " + e.Message);
                    DateTime now = DateTime.Now;
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  File Name: " + fileName, now);
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Exception Type: " + e.GetType().FullName, now);
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  HResult:" + e.HResult, now);
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Message: " + e.Message, now);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("-------------------------------------------------------------------------");
                }

            }

            return result.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CopyFile.Service;
using CopyFile.Service.Implementation;

namespace CopyFile
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileService fileService = new FileService();
            //Timer timer = new Timer();

            Console.WriteLine("-------------------------------------------------------------------------");

            while (true)
            {
                fileService.MoveFiles("In\\", "Out\\", "*.Json");
            }

        }

        //private void TimerCallback()
        //{
        //    System.Threading.TimerCallback result = new TimerCallback();
        //}
    }
}

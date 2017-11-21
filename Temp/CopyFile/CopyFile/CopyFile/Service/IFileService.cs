using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFile.Service
{
    interface IFileService
    {
        string[] MoveFiles(string inPath, string outPath, string searchPattern = null);
    }
}

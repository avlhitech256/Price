using System.Drawing;
using System.IO;
using Json.Contract;

namespace File.Service
{
    public interface IFileService
    {
        MemoryStream ReadFile(string fileName);

        T ReadJsonFile<T>(string fileName) where T : class, new ();

        Bitmap ReadBitmapPicture(string fileName);

        byte[] ReadByteArrayPicture(string fileName);

        Clients ReadClients(string fileName);

        MetaData ReadMetaData(string fileName);

        PriceList ReadPriceList(string fileName);

        string[] GetFileNames(string dirPath, string searchPattern = null);

        FileInfo[] GetFileInfos(string dirPath, string searchPattern = null);

        string GetFileName(string fileNameWithExtention);
    }
}

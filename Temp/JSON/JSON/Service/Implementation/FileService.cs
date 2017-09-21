using System.IO;

namespace JSON.Service.Implementation
{
    public class FileService : IFileService
    {
        public MemoryStream ReadFile(string fileName)
        {
            MemoryStream memoryStream = new MemoryStream();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
            }

            return memoryStream;
        }
    }
}

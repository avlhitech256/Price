using System.IO;

namespace JSON.Service
{
    public interface IFileService
    {
        MemoryStream ReadFile(string fileName);
    }
}

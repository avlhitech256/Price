using System.IO;
using JSON.Contract;

namespace JSON.Service.Implementation
{
    public class FileService : IFileService
    {
        #region Members

        private readonly IJsonService jsonService;

        #endregion

        #region#

        public FileService(IJsonService jsonService = null)
        {
            this.jsonService = jsonService ?? new JsonService();
        }

        #endregion

        #region#

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

        public T ReadJsonFile<T>(string fileName) where T : class
        {
            T result;

            using (MemoryStream stream = ReadFile(fileName))
            {
                result = jsonService.Convert<T>(stream);
            }

            return result;
        }

        public Clients ReadClients(string fileName)
        {
            Clients result = ReadJsonFile<Clients>(fileName);
            return result;
        }

        public MetaData ReadMetaData(string fileName)
        {
            MetaData result = ReadJsonFile<MetaData>(fileName);
            return result;
        }

        public PriceList ReadPriceList(string fileName)
        {
            PriceList result = ReadJsonFile<PriceList>(fileName);
            return result;
        }

        #endregion
    }
}

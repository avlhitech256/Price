using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Json.Contract;
using Json.Service;
using Json.Service.Implementation;
using Media.Service;
using Media.Service.Implementation;

namespace File.Service.Implementation
{
    public class FileService : IFileService
    {
        #region Members

        private readonly IJsonService jsonService;
        private readonly IImageService imageService;

        #endregion

        #region Constructors

        public FileService(IJsonService jsonService = null, IImageService imageService = null)
        {
            this.jsonService = jsonService ?? new JsonService();
            this.imageService = imageService ?? new ImageService();
        }

        #endregion

        #region Methods

        public MemoryStream ReadFile(string fileName)
        {
            MemoryStream memoryStream = new MemoryStream();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        GC.Collect();
                        fileStream.CopyTo(memoryStream);
                    }
                }
                catch (Exception)
                {
                    memoryStream = null;
                }
            }

            return memoryStream;
        }

        public T ReadJsonFile<T>(string fileName) where T : class, new ()
        {
            T result;

            try
            {
                using (MemoryStream stream = ReadFile(fileName))
                {
                    result = jsonService.Convert<T>(stream);
                }
            }
            catch (Exception)
            {
                result = new T();
            }

            return result;
        }

        public Bitmap ReadBitmapPicture(string fileName)
        {
            Bitmap picture;

            try
            {
                picture = new Bitmap(fileName);
            }
            catch (Exception)
            {
                picture = null;
            }

            return picture;
        }

        public byte[] ReadByteArrayPicture(string fileName)
        {
            Image image = ReadBitmapPicture(fileName);
            byte[] picture = image != null ? imageService.ConvertToByteArray(image) : null;
            return picture;
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

        public string GetFileName(string fileNameWithExtention)
        {
            string fileName = string.Empty;

            if (!string.IsNullOrWhiteSpace(fileNameWithExtention))
            {
                string[] fileFilds = fileNameWithExtention.Split('.');

                if (fileFilds.Any())
                {
                    fileName = fileFilds[0];
                }
            }

            return fileName;
        }

        #endregion
    }
}

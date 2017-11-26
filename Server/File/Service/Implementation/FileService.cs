using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Json.Contract;
using Json.Service;
using Json.Service.Implementation;
using Media.Service;
using Media.Service.Implementation;
using File = System.IO.File;

namespace File.Service.Implementation
{
    public class FileService : IFileService
    {
        #region Members

        private readonly IJsonService jsonService;
        private readonly IImageService imageService;
        private Timer timer;
        private MovingThreadInfo oldMovingThreadInfo;

        #endregion

        #region Constructors

        public FileService(IJsonService jsonService = null, IImageService imageService = null)
        {
            this.jsonService = jsonService ?? new JsonService();
            this.imageService = imageService ?? new ImageService();
            timer = null;
            oldMovingThreadInfo = null;
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

        public T ReadJsonFile<T>(string fileName) where T : class, new()
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
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                    //************************************
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Create directory " + dirPath, DateTime.Now);
                    Console.WriteLine("-------------------------------------------------------------------------");
                    //************************************
                }

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

        private string[] GetFileNames(MovingQueueItem movingQueueItem)
        {
            return GetFileNames(movingQueueItem.SourcePath, movingQueueItem.SearchPatterns);
        }

        public string[] GetFileNames(string dirPath, IEnumerable<string> searchPatterns = null)
        {
            string[] fileNames =
                GetFileInfos(dirPath, searchPatterns)
                    .Where(f => f.Exists)
                    .Select(x => x.Name)
                    .Distinct()
                    .ToArray();

            return fileNames;
        }

        public FileInfo[] GetFileInfos(string dirPath, IEnumerable<string> searchPatterns = null)
        {
            FileInfo[] result;

            List<string> searchPatternsArray = searchPatterns != null
                ? new List<string>(searchPatterns)
                : new List<string>();

            if (searchPatternsArray.Any())
            {

                List<FileInfo> resultList = new List<FileInfo>();

                searchPatternsArray.ForEach(
                    searchPattern =>
                    {
                        FileInfo[] filesInfo = GetFileInfos(dirPath, searchPattern);

                        foreach (FileInfo fileInfo in filesInfo)
                        {
                            if (resultList.All(x => x.Name != fileInfo.Name))
                            {
                                resultList.Add(fileInfo);
                            }
                        }
                    });

                result = resultList.ToArray();
            }
            else
            {
                result = GetFileInfos(dirPath, (string) null);
            }

            return result;
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

        public MovingInfo MoveFiles(string sourcePath, string destinationPath, IEnumerable<string> searchPatterns = null)
        {
            MovingInfo result = new MovingInfo();

            if (!string.IsNullOrWhiteSpace(sourcePath) && !string.IsNullOrWhiteSpace(destinationPath))
            {
                string inPath = ValidatePath(sourcePath);
                string outPath = ValidatePath(destinationPath);

                if (PrepareDirectory(inPath) && PrepareDirectory(outPath))
                {
                    string[] fileNames = GetFileNames(inPath, searchPatterns);

                    foreach (string fileName in fileNames)
                    {
                        try
                        {
                            if (!System.IO.Directory.Exists(outPath))
                            {
                                System.IO.Directory.CreateDirectory(outPath);
                                //************************************
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Create directory " + outPath, DateTime.Now);
                                Console.WriteLine("-------------------------------------------------------------------------");
                                //************************************
                            }

                            System.IO.File.Move(inPath + fileName, outPath + fileName);
                            result.MovedFiles.Add(new FileOperationInfo(FileOperation.Move, fileName, DateTimeOffset.Now));
                            //************************************
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Moved file " + fileName, DateTime.Now);
                            Console.WriteLine("-------------------------------------------------------------------------");
                            //************************************
                        }
                        catch (Exception e)
                        {
                            FileOperationInfo operationInfo =
                                new FileOperationInfo(FileOperation.Move, fileName, DateTimeOffset.Now, e);

                            if (e is IOException && e.HResult == -2147024864) // Файл захвачен другим процессом
                            {
                                result.WaitingFiles.Add(operationInfo);
                                ; // TODO Write Exception to LOG
                            }
                            else
                            {
                                result.ExceptionFiles.Add(operationInfo);
                                ; // TODO Write Exception to LOG
                            }

                            //************************************
                            DateTime now = DateTime.Now;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  File Name: " + fileName, now);
                            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Exception Type: " + e.GetType().FullName, now);
                            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  HResult:" + e.HResult, now);
                            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Message: " + e.Message, now);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("-------------------------------------------------------------------------");
                            //************************************
                        }
                    }
                }
            }

            return result;
        }

        private string ValidatePath(string dirPath)
        {
            string result = !string.IsNullOrWhiteSpace(dirPath) ? dirPath.Trim() : String.Empty;

            if (!result.EndsWith("\\"))
            {
                result = result + "\\";
            }

            return result;
        }

        private bool PrepareDirectory(string dirPath)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(dirPath))
            {
                if (System.IO.Directory.Exists(dirPath))
                {
                    result = true;
                }
                else
                {
                    try
                    {
                        DirectoryInfo dirInfo = System.IO.Directory.CreateDirectory(dirPath);
                        result = dirInfo.Exists;
                    }
                    catch (Exception)
                    {
                        ; //TODO Write to LOG
                        //throw;
                    }
                }
            }

            return result;
        }

        public void AsyncMoveFiles(MovingThreadInfo movingThreadInfo)
        {
            if (timer == null && movingThreadInfo != null && movingThreadInfo.TimeOut >= movingThreadInfo.DueTime &&
                movingThreadInfo.MovingQueue.Any(x => GetFileNames(x).Any()))
            {
                timer = new Timer(MoveFiles, movingThreadInfo, Timeout.Infinite, Timeout.Infinite);

                if (oldMovingThreadInfo != movingThreadInfo)
                {
                    if (oldMovingThreadInfo != null)
                    {
                        oldMovingThreadInfo.Completed -= DisposeTimer;
                        oldMovingThreadInfo.TimeOutIsOver -= DisposeTimer;
                    }

                    oldMovingThreadInfo = movingThreadInfo;

                    movingThreadInfo.Completed += DisposeTimer;
                    movingThreadInfo.TimeOutIsOver += DisposeTimer;
                }

                timer.Change(movingThreadInfo.DueTime, movingThreadInfo.Period);
            }
        }

        private void DisposeTimer(object sender, EventArgs args)
        {
            timer?.Dispose();
            timer = null;
        }

        private void MoveFiles(object info)
        {
            var movingThreadInfo = info as MovingThreadInfo;

            if (movingThreadInfo != null)
            {
                MoveFiles(movingThreadInfo);
            }
        }

        private void MoveFiles(MovingThreadInfo movingThreadInfo)
        {
            if (movingThreadInfo?.MovingQueue != null &&
                movingThreadInfo.MovingQueue.All(x => !string.IsNullOrWhiteSpace(x.SourcePath)) &&
                movingThreadInfo.MovingQueue.All(x => !string.IsNullOrWhiteSpace(x.DestinationPath)))
            {
                movingThreadInfo.MovingQueue.ForEach(x =>
                {
                    x.SourcePath = ValidatePath(x.SourcePath);
                    x.DestinationPath = ValidatePath(x.DestinationPath);
                });

                if (movingThreadInfo.MovingQueue.All(x => PrepareDirectory(x.SourcePath)) && 
                    movingThreadInfo.MovingQueue.All(x => PrepareDirectory(x.DestinationPath)))
                {
                    if (Validate(movingThreadInfo))
                    {
                        movingThreadInfo.EndOfProcess();
                    }
                    else
                    {
                        movingThreadInfo.MovingQueue.ForEach(
                            x =>
                            {
                                if (GetFileNames(x.SourcePath, x.SearchPatterns).Length > 0)
                                {
                                    Stop();

                                    if (!movingThreadInfo.Start.HasValue)
                                    {
                                        movingThreadInfo.Start = DateTimeOffset.Now;
                                    }

                                    //movingThreadInfo.LastMovingInfo = movingThreadInfo.MovingQueue.Last() == x;
                                    movingThreadInfo.MovingInfo = MoveFiles(x.SourcePath,
                                                                            x.DestinationPath,
                                                                            x.SearchPatterns);
                                }
                            });
                    }

                    Start(movingThreadInfo);
                }
            }
        }

        private bool Validate(MovingThreadInfo movingThreadInfo)
        {
            bool isValid = movingThreadInfo.HasOutOfTime || 
                           (SourceIsEmpty(movingThreadInfo) && ValidateDestination(movingThreadInfo));

            return isValid;
        }

        private bool ValidateDestination(MovingThreadInfo movingThreadInfo)
        {
            bool valid = movingThreadInfo.MovingQueue.All(ValidateInputFiles);
            return valid;
        }

        private bool ValidateInputFiles(MovingQueueItem item)
        {
            bool valid = item.SearchPatterns.All(x => ValidateInputFile(item.DestinationPath, x));
            return valid;
        }

        private bool ValidateInputFile(string dirPath, string searchPattern)
        {
            bool valid = false;
            string[] files = GetFileNames(dirPath, new[] {searchPattern});

            if (files.Length > 0)
            {
                string firstFileName = FileName(files[0]);
                string startPattern = StartPattern(searchPattern);

                if (files.Length == 1 && firstFileName == startPattern)
                {
                    valid = true;
                }
                else if (files.Length > 1 && firstFileName != startPattern)
                {
                    int length = NemericPathLength(files[0], startPattern);

                    if (files.All(x => NemericPathLength(x, startPattern) == length))
                    {
                        valid = true;
                        int fileNember = 0;
                        int count = 0;

                        foreach (string fileName in files.OrderBy(x => x))
                        {
                            int? number = GetFileNumber(fileName, startPattern);

                            if (number.HasValue && fileNember == number)
                            {
                                fileNember++;
                                count++;
                            }
                            else
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid && count != GetCountFiles(files[0], startPattern))
                        {
                            valid = false;
                        }
                    }
                }
            }

            return valid;
        }

        private int NemericPathLength(string fileName, string startPattern)
        {
            int length = GetNemericPathFileName(fileName, startPattern).Length;
            return length;
        }

        private int? GetFileNumber(string fileName, string startPattern)
        {
            int? fileNumber = null;
            string numericPath = GetNemericPathFileName(fileName, startPattern);

            if (numericPath.Length % 2 == 0)
            {
                string stringFileNumber = numericPath.Substring(0, numericPath.Length / 2);
                int number;

                if (int.TryParse(stringFileNumber, out number))
                {
                    fileNumber = number;
                }
            }

            return fileNumber;
        }

        private int? GetCountFiles(string fileName, string startPattern)
        {
            int? count = null;
            string numericPath = GetNemericPathFileName(fileName, startPattern);

            if (numericPath.Length % 2 == 0)
            {
                string stringFilesCount = numericPath.Substring(numericPath.Length / 2);
                int number;

                if (int.TryParse(stringFilesCount, out number))
                {
                    count = number;
                }
            }

            return count;
        }

        private string GetNemericPathFileName(string fileName, string startPattern)
        {
            string numericPath = fileName.StartsWith(startPattern)
                ? fileName.Substring(startPattern.Length)
                : String.Empty;
            return numericPath;
        }

        private string FileName(string fileNameWithExrantion)
        {
            int separateIndex = fileNameWithExrantion.LastIndexOf(".", StringComparison.CurrentCulture);
            string fileName = fileNameWithExrantion.Substring(0, separateIndex);
            return fileName;
        }

        private string StartPattern(string searchPattern)
        {
            string startPattern = FileName(searchPattern);
            startPattern = startPattern.TrimEnd('*');
            return startPattern;
        }

        private bool SourceIsEmpty(MovingThreadInfo movingThreadInfo)
        {
            bool isEmpty = movingThreadInfo.MovingQueue.All(x => !GetFileNames(x.SourcePath, x.SearchPatterns).Any());
            return isEmpty;
        }

        private void Start(MovingThreadInfo movingThreadInfo)
        {
            timer?.Change(movingThreadInfo.DueTime, movingThreadInfo.Period);
        }

        private void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion
    }
}

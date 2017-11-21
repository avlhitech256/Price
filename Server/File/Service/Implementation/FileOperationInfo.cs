using System;

namespace File.Service.Implementation
{
    public class FileOperationInfo
    {
        #region Constructors

        public FileOperationInfo(FileOperation operation, string fileName, DateTimeOffset dateTime, Exception e = null)
        {
            Operation = operation;
            FileName = fileName;
            DateTime = dateTime;
            Exception = e;
        }

        #endregion

        #region Properties

        public FileOperation Operation { get; }

        public string FileName { get; }

        public DateTimeOffset DateTime { get; }

        public Exception Exception { get; }

        #endregion
    }
}

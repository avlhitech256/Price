using System.Collections.Generic;

namespace File.Service.Implementation
{
    public class MovingInfo
    {
        #region Constructors

        public MovingInfo(IEnumerable<FileOperationInfo> movedFiles = null, IEnumerable<FileOperationInfo> waitingFiles = null, IEnumerable<FileOperationInfo> exceptionFiles = null)
        {
            MovedFiles = movedFiles != null ? new List<FileOperationInfo>(movedFiles) : new List<FileOperationInfo>();
            WaitingFiles = waitingFiles != null ? new List<FileOperationInfo>(waitingFiles) : new List<FileOperationInfo>();
            ExceptionFiles = exceptionFiles != null ? new List<FileOperationInfo>(exceptionFiles) : new List<FileOperationInfo>();
        }

        #endregion

        #region Properties

        public List<FileOperationInfo> MovedFiles { get; }

        public List<FileOperationInfo> WaitingFiles { get; }

        public List<FileOperationInfo> ExceptionFiles { get; }

        #endregion
    }
}

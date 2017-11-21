using System;
using System.Collections.Generic;
using System.Threading;
using File.Service;
using File.Service.Implementation;

namespace Load.Service.Implementation
{
    public class DownLoadService : IDownLoadService
    {
        #region Members

        private readonly Timer timer;
        private readonly LoadingThreadInfo loadingThreadInfo;
        private readonly MovingThreadInfo movingThreadInfo;
        private readonly IFileService fileService;
        private bool isLoading;

        #endregion

        #region Constructors

        public DownLoadService(uint timeOut, uint dueTime, uint period,
                               string sourcePath, string destinationPath,
                               IEnumerable<string> searchPatterns = null)
        {
            isLoading = false;
            movingThreadInfo = new MovingThreadInfo(timeOut, dueTime, period, sourcePath, destinationPath, searchPatterns);
            movingThreadInfo.StartMoving += StartMovingFiles;
            movingThreadInfo.Completed += MovingCompleted;
            movingThreadInfo.TimeOutIsOver += MovingTimeOutIsOver;
            loadingThreadInfo = new LoadingThreadInfo();
            loadingThreadInfo.Start += (sender, args) => Stop();
            loadingThreadInfo.End += (sender, args) => Start();
            timer = new Timer(LoadData, loadingThreadInfo, Timeout.Infinite, Timeout.Infinite);
            fileService = new FileService();
            //Start();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Start()
        {
            timer.Change(movingThreadInfo.DueTime, movingThreadInfo.Period);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void LoadData(object info)
        {
            LoadingThreadInfo threadInfo = info as LoadingThreadInfo;

            if (loadingThreadInfo != null)
            {
                LoadData(threadInfo);
            }
        }

        private void LoadData(LoadingThreadInfo threadInfo)
        {
            if (!threadInfo.IsLoading)
            {
                movingThreadInfo.MovingInfo.MovedFiles.Clear();
                movingThreadInfo.MovingInfo.WaitingFiles.Clear();
                movingThreadInfo.MovingInfo.ExceptionFiles.Clear();
                movingThreadInfo.Start = null;
                fileService.AsyncMoveFiles(movingThreadInfo);
            }
        }

        private void StartMovingFiles(object sender, EventArgs args)
        {
            loadingThreadInfo.IsLoading = true;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Start moving files...", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void MovingCompleted(object sender, EventArgs e)
        {
            LoadData();
            loadingThreadInfo.IsLoading = false;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  End moving files", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void MovingTimeOutIsOver(object sender, EventArgs e)
        {
            SaveMovedErrors(sender);
            MovingCompleted(sender, e);
        }

        private void LoadData()
        {
            // TODO Load Json Files to DataBase
        }

        private void SaveMovedErrors(object sender)
        {
            MovingThreadInfo movingInfo = sender as MovingThreadInfo;

            if (movingInfo != null)
            {
                SaveMovedErrors(movingInfo);
            }
        }

        private void SaveMovedErrors(MovingThreadInfo movingInfo)
        {
            ; // TODO Save Moved Errors to DataBase & LOG;
        }

        #endregion

        #region Events
        #endregion

    }
}

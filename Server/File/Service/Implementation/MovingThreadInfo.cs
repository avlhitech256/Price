using System;
using System.Collections.Generic;
using System.Linq;

namespace File.Service.Implementation
{
    public class MovingThreadInfo
    {
        #region Members

        private readonly MovingInfo movingInfo;
        private DateTimeOffset? start;

        #endregion

        #region Constructors
        public MovingThreadInfo(uint timeOut, uint dueTime, uint period, List<MovingQueueItem> movingQueue)
        {
            TimeOut = timeOut;
            DueTime = dueTime;
            Period = period;
            MovingQueue = movingQueue;
            Start = null;
            End = null;
            LastMovingInfo = false;
            movingInfo = new MovingInfo();
        }

        #endregion

        #region Properties

        public MovingInfo MovingInfo
        {
            get
            {
                return movingInfo;
            }
            set
            {
                if (null != value)
                {
                    Merge(movingInfo, value);

                    if (LastMovingInfo)
                    {
                        EndOfProcess();
                    }
                }
            }
        }

        public bool LastMovingInfo { get; set; }

        public uint TimeOut { get; }

        public uint DueTime { get; }

        public uint Period { get; }

        public List<MovingQueueItem> MovingQueue { get; }

        public DateTimeOffset? Start
        {
            get { return start; }
            set
            {
                if (start != value)
                {
                    start = value;

                    if (value != null)
                    {
                        OnStartMoving();
                    }
                }
            }
        }

        public DateTimeOffset? End { get; private set; }

        #endregion

        #region Methods

        private void OnStartMoving()
        {
            StartMoving?.Invoke(this, new EventArgs());
        }

        private void OnCompleted()
        {
            Completed?.Invoke(this, new EventArgs());
        }

        private void OnTimeOutIsOver()
        {
            TimeOutIsOver?.Invoke(this, new EventArgs());
        }

        private void Merge(List<string> source, List<string> destination)
        {
            if (source != null)
            {
                destination?.AddRange(source.Where(x => destination.All(m => m != x)));
            }
        }

        private void Merge(List<FileOperationInfo> source, List<FileOperationInfo> destination)
        {
            if (source != null && destination != null)
            {
                destination.RemoveAll(x => source.Any(s => s.FileName == x.FileName));
                destination.AddRange(source);
            }
        }

        private void Merge(MovingInfo source, MovingInfo destination)
        {
            if (source != null && destination !=null)
            {
                Merge(source.MovedFiles, destination.MovedFiles);
                Merge(source.WaitingFiles, destination.WaitingFiles);
                Merge(source.ExceptionFiles, destination.ExceptionFiles);
                RemoveDublicate(source.MovedFiles, source.WaitingFiles, source.ExceptionFiles);
            }
        }

        private void RemoveDublicate(List<FileOperationInfo> source1, 
                                     List<FileOperationInfo> source2,
                                     List<FileOperationInfo> source3)
        {
            RemoveDublicate(source1, source2);
            RemoveDublicate(source1, source3);
            RemoveDublicate(source2, source3);
        }

        private void RemoveDublicate(List<FileOperationInfo> source1, List<FileOperationInfo> source2)
        {
            source1.ForEach(x => source2.RemoveAll(s => s.FileName == x.FileName && s.DateTime < x.DateTime));
            source2.ForEach(x => source1.RemoveAll(s => s.FileName == x.FileName && s.DateTime < x.DateTime));
        }

        private void EndOfProcess()
        {
            if (!movingInfo.ExceptionFiles.Any() && !movingInfo.WaitingFiles.Any())
            {
                End = DateTimeOffset.Now;
                OnCompleted();
            }
            else if (Start.HasValue && (DateTimeOffset.Now - Start.Value).Milliseconds > TimeOut)
            {
                End = DateTimeOffset.Now;
                OnTimeOutIsOver();
            }
        }

        #endregion

        #region Events

        public event EventHandler StartMoving;

        public event EventHandler Completed;

        public event EventHandler TimeOutIsOver;

        #endregion
    }
}

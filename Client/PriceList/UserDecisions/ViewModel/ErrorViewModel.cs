using System;
using System.Text;
using Common.Data.Notifier;
using Common.ViewModel.Command;

namespace UserDecisions.ViewModel
{
    public class ErrorViewModel : Notifier
    {
        #region Members

        private string message;

        #endregion

        #region Constructors

        public ErrorViewModel()
        {
            CreateCommand();
        }

        #endregion

        #region Properties

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (value != message)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand OkCommand { get; private set; }

        public Action CloseExceptionWindow { get; set; }

        #endregion

        #region Methods

        public void SetException(Exception e)
        {
            StringBuilder builder = new StringBuilder();

            if (e.InnerException != null)
            {
                builder.AppendLine("InnerException:");
                builder.AppendLine(e.InnerException.Message);
                builder.AppendLine(" ");
                builder.AppendLine(e.InnerException.StackTrace);
                builder.AppendLine("-----------------------------------------------");
                builder.AppendLine(" ");
            }

            builder.AppendLine("Exception:");
            builder.AppendLine(e.Message);
            builder.AppendLine(" ");
            builder.AppendLine(e.StackTrace);
            builder.AppendLine("-----------------------------------------------");
            builder.AppendLine(" ");

            Exception baseException = e.GetBaseException();
            builder.AppendLine("BaseException:");
            builder.AppendLine(baseException.Message);
            builder.AppendLine(" ");
            builder.AppendLine(baseException.StackTrace);
            builder.AppendLine("-----------------------------------------------");
            builder.AppendLine(" ");

            Message = builder.ToString();
        }

        private void CreateCommand()
        {
            OkCommand = new DelegateCommand(DoOk, CanDoOk);
        }

        private void DoOk(object obj)
        {
            CloseExceptionWindow?.Invoke();
        }

        private bool CanDoOk(object arg)
        {
            return true;
        }

        #endregion
    }
}

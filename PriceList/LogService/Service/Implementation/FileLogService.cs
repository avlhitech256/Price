using System;

namespace LogService.Service.Implementation
{
    class FileLogService : IFileLogService
    {
        public MessageLevel LogLevel { get; set; }
        public MessageType LogType { get; set; }
        public string LogFilePath { get; set; }
        public bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High)
        {
            throw new NotImplementedException();
        }
    }
}

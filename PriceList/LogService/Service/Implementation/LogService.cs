using System;
using DataService.Model;

namespace LogService.Service.Implementation
{
    public class LogService : ILogService
    {
        public LogService(string filePath, DBContext context) :
            this(filePath, context, MessageType.Info, MessageLevel.Low)
        { }

        public LogService(string filePath, DBContext context, MessageType type) :
            this(filePath, context, type, MessageLevel.Low) {}

        public LogService(string filePath, DBContext context, MessageType type, MessageLevel level)
        {
            LogFilePath = filePath;
            DBContext = context;
            LogType = type;
            LogLevel = level;
        }
        public MessageLevel LogLevel { get; set; }
        public MessageType LogType { get; set; }
        public bool SaveIntoDatabace { get; set; }
        public bool SaveIntoConsole { get; set; }
        public bool SaveIntoFile { get; set; }
        public string LogFilePath { get; set; }
        public DBContext DBContext { get; set; }
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

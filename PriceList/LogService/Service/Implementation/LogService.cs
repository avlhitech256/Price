using System;
using DataService.Model;

namespace LogService.Service.Implementation
{
    public class LogService : ILogService
    {
        private IConsoleLogService consoleLogService;
        private IFileLogService fileLogService;
        private IDatabaseLogService databaseLogService;
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
            SaveIntoConsole = true;
            SaveIntoFile = true;
            SaveIntoDatabace = true;
            consoleLogService = new ConsoleLogService();
            fileLogService = new FileLogService();
            databaseLogService = new DatabaseLogService();
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
            bool result = true;

            if (SaveIntoConsole)
            {
                result = consoleLogService.SendMessage(message, type, level);
            }

            if (SaveIntoFile && !string.IsNullOrWhiteSpace(LogFilePath))
            {
                result = fileLogService.SendMessage(message, type, level) && result;
            }

            if (SaveIntoDatabace && DBContext != null)
            {
                result = databaseLogService.SendMessage(message, type, level) && result;
            }

            return result;
        }

        public bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High)
        {
            bool result = true;

            if (SaveIntoConsole)
            {
                result = consoleLogService.SendMessage(e, type, level);
            }

            if (SaveIntoFile && !string.IsNullOrWhiteSpace(LogFilePath))
            {
                result = fileLogService.SendMessage(e, type, level) && result;
            }

            if (SaveIntoDatabace && DBContext != null)
            {
                result = databaseLogService.SendMessage(e, type, level) && result;
            }

            return result;
        }
    }
}

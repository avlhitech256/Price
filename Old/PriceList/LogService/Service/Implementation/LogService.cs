using System;
using Common.Domain;
using DataService.Model;
using DataService.Service;
using DataService = DataService.Service.Implementation.DataService;

namespace LogService.Service.Implementation
{
    public class LogService : ILogService
    {
        #region Members

        private IDataService dataService;
        private readonly IConsoleLogService consoleLogService;
        private readonly IFileLogService fileLogService;
        private readonly IDatabaseLogService databaseLogService;

        #endregion

        #region Constructors

        public LogService() : this(String.Empty, null) { }
        public LogService(string filePath, IDataService dataService) :
            this(filePath, dataService, MessageType.Info) { }

        public LogService(string filePath, IDataService dataService, MessageType type) :
            this(filePath, dataService, type, MessageLevel.Low) {}

        public LogService(string filePath, IDataService dataService, MessageType type, MessageLevel level)
        {
            LogFilePath = filePath;
            this.dataService = dataService?? new global::DataService.Service.Implementation.DataService((ILogService)this);
            LogType = type;
            LogLevel = level;
            SaveIntoConsole = true;
            SaveIntoFile = true;
            SaveIntoDatabace = true;
            consoleLogService = new ConsoleLogService();
            fileLogService = new FileLogService();
            databaseLogService = new DatabaseLogService();
        }

        #endregion

        #region Properties

        public MessageLevel LogLevel { get; set; }

        public MessageType LogType { get; set; }

        public bool SaveIntoDatabace { get; set; }

        public bool SaveIntoConsole { get; set; }

        public bool SaveIntoFile { get; set; }

        public string LogFilePath { get; set; }

        //public 

        private DBContext DBContext { get; set; }

        #endregion

        #region Methods

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

        #endregion
    }
}

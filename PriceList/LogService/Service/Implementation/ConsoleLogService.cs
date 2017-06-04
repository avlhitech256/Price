using System;

namespace LogService.Service.Implementation
{
    class ConsoleLogService : IConsoleLogService
    {
        #region Members
        
        
        #endregion

        #region Constructors

        public ConsoleLogService() : this(MessageType.Info) { }
        public ConsoleLogService(MessageType type) : this(type, MessageLevel.Low) { }
        public ConsoleLogService(MessageLevel logLevel, MessageType logType)
        {
            LogLevel = logLevel;
            LogType = logType;
        }

        public ConsoleLogService(MessageType type, MessageLevel level)
        {
            LogType = type;
            LogLevel = level;
        }
        
        #endregion

        #region Properties

        public MessageLevel LogLevel { get; set; }
        public MessageType LogType { get; set; }

        #endregion

        #region Methods

        public bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

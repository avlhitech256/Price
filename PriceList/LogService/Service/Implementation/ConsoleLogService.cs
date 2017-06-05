using System;
using System.Text;
using Common.Domain;

namespace LogService.Service.Implementation
{
    class ConsoleLogService : IConsoleLogService
    {
        #region Constructors

        public ConsoleLogService() : this(MessageType.Info) { }

        public ConsoleLogService(MessageType type) 
            : this(type, MessageLevel.Low) { }

        public ConsoleLogService(MessageType type, MessageLevel level) 
            : this(type, level, new ConsoleColorConfig()) { }

        public ConsoleLogService(ConsoleColorConfig messageColorConfig) 
            : this(MessageType.Info, messageColorConfig) { }

        public ConsoleLogService(MessageType type, ConsoleColorConfig messageColorConfig) 
            : this(type, MessageLevel.Low, messageColorConfig) { }

        public ConsoleLogService(MessageType type, MessageLevel level, ConsoleColorConfig messageColorConfig)
            : this(type, level, messageColorConfig, new ConsoleColorConfig()) { }

        public ConsoleLogService(MessageType logType, MessageLevel logLevel, ConsoleColorConfig messageColorConfig, ConsoleColorConfig headerColorConfig)
        {
            LogLevel = logLevel;
            LogType = logType;
            MessageColorConfig = messageColorConfig;
            HeaderColorConfig = headerColorConfig;
        }

        #endregion

        #region Properties

        public ConsoleColorConfig HeaderColorConfig { get; set; }

        public ConsoleColorConfig MessageColorConfig { get; set; }

        public MessageLevel LogLevel { get; set; }

        public MessageType LogType { get; set; }

        #endregion

        #region Methods

        public bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            string header = GetHeader(type);
            Console.ForegroundColor = HeaderColorConfig.GetColor(type, level); ;
            Console.Write(header);
            Console.ForegroundColor = MessageColorConfig.GetColor(type, level);
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
            return true;
        }

        public bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = MessageColorConfig.GetColor(type, level);
            StringBuilder message = new StringBuilder();

            Console.WriteLine(message.ToString());
            Console.ForegroundColor = oldColor;
            return true;
        }

        private string GetHeader(MessageType type, string formatDateTime = "O")
        {
            string header = DateTime.Now.ToString(formatDateTime) + " ";

            switch (type)
            {
                case MessageType.Error:
                    header += "[ Error ] - ";
                    break;
                case MessageType.Warning:
                    header += "[Warning] - ";
                    break;
                case MessageType.Info:
                    header += "[ Info  ] - ";
                    break;
                default:
                    header += "[Unknown] - ";
                    break;
            }

            return header;
        }

        #endregion
    }
}

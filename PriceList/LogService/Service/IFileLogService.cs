using System;

namespace LogService.Service
{
    interface IFileLogService
    {
        MessageLevel LogLevel { get; set; }
        MessageType LogType { get; set; }
        string LogFilePath { get; set; }
        bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low);
        bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High);
    }
}

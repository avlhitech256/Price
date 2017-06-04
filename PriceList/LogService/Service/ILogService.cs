using System;
using DataService.Model;

namespace LogService.Service
{
    public interface ILogService
    {
        MessageLevel LogLevel { get; set; }
        MessageType LogType { get; set; }
        bool SaveIntoDatabace { get; set; }
        bool SaveIntoConsole { get; set; }
        bool SaveIntoFile { get; set; }
        string LogFilePath { get; set; }
        DBContext DBContext { get; set; }
        bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low);
        bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High);
    }
}

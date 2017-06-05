using System;
using Common.Domain;
using DataService.Model;

namespace LogService.Service
{
    interface IDatabaseLogService
    {
        MessageLevel LogLevel { get; set; }
        MessageType LogType { get; set; }
        DBContext DBContext { get; set; }
        bool SendMessage(string message, MessageType type = MessageType.Info, MessageLevel level = MessageLevel.Low);
        bool SendMessage(Exception e, MessageType type = MessageType.Error, MessageLevel level = MessageLevel.High);
    }
}

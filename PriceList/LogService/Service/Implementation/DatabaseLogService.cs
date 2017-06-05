using System;
using Common.Domain;
using DataService.Model;

namespace LogService.Service.Implementation
{
    class DatabaseLogService : IDatabaseLogService
    {
        public MessageLevel LogLevel { get; set; }
        public MessageType LogType { get; set; }
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

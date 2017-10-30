using System;
using Common.Data.Enum;

namespace Domain.DomainContext
{
    public interface ILoadingContext
    {

        bool IsLoading { get; set; }

        bool IsWaiting { get; set; }

        Action<string> ShowWaitScreen { get; set; }

        Action<string> SetWaitScreenMessage { get; set; }

        Action HideWaitScreen { get; set; }

        void SetWaitMessage(AsyncOperationType type);
    }
}

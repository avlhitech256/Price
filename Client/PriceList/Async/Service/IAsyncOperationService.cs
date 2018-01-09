using System;
using Common.Data.Enum;
using Common.Event;

namespace Async.Service
{
    public interface IAsyncOperationService
    {
        void PerformAsyncOperation<T, TResult>(
            AsyncOperationType type,
            Func<T, TResult> func,
            T param,
            Action<Exception, TResult> completeAction);

        event EventHandler<PriceEventArgs<AsyncOperationType>> OperationStarted;
        event EventHandler<PriceEventArgs<AsyncOperationType>> OperationCompleted;
    }
}

using System;
using TasklistPresentation.Models;
using TasklistPresentation.Objects;

namespace TasklistPresentation.Services
{
    public interface IAsyncOperationService
    {
        void PerformAsyncOperation<T, TResult>(
            AsyncOperationType type,
            Func<T, TResult> func,
            T param,
            Action<Exception, TResult> completeAction);

        event EventHandler<GenericEventArgs<AsyncOperationType>> OperationStarted;
        event EventHandler<GenericEventArgs<AsyncOperationType>> OperationCompleted;
    }
}
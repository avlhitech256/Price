﻿using System.Threading;
using log4net;
using System;
using System.Threading.Tasks;
using TasklistPresentation.Models;
using TasklistPresentation.Objects;

namespace TasklistPresentation.Services.Implementation
{
    public class AsyncOperationService : IAsyncOperationService
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(AsyncOperationService));

        private readonly IUserDecisionsService userDecisionsService;
        private readonly TaskScheduler taskScheduler;

        public AsyncOperationService(IUserDecisionsService userDecisionsService, TaskScheduler taskScheduler)
        {
            this.userDecisionsService = userDecisionsService;
            this.taskScheduler = taskScheduler;
        }

        public void PerformAsyncOperation<T, TResult>(
            AsyncOperationType type,
            Func<T, TResult> func,
            T param,
            Action<Exception, TResult> completeAction)
        {
            OperationStarted(this, new GenericEventArgs<AsyncOperationType>(type));
            var task = new Task<TResult>(() => func(param));
            if (taskScheduler == null)
            {
                task.ContinueWith(t => ProcessTaskResult(t, completeAction, type));
            }
            else
            {
                task.ContinueWith(t => ProcessTaskResult(t, completeAction, type), taskScheduler);
            }

            task.Start(TaskScheduler.Default);
        }

        public event EventHandler<GenericEventArgs<AsyncOperationType>> OperationStarted = delegate { };
        public event EventHandler<GenericEventArgs<AsyncOperationType>> OperationCompleted = delegate { };

        private void ProcessTaskResult<TResult>(Task<TResult> task, Action<Exception, TResult> completeAction, AsyncOperationType type)
        {
            if (task.IsFaulted && task.Exception != null)
            {
                // TODO: move this code into OperationCompleted registered by a Form to isolate it from UTing.
                ShowExceptionCarefully(task.Exception);
            }

            try
            {
                OperationCompleted(this, new GenericEventArgs<AsyncOperationType>(type));
                if (completeAction != null)
                {
                    completeAction(task.Exception, task.Exception == null ? task.Result : default(TResult));
                }
            }
            catch (Exception e)
            {
                ShowExceptionCarefully(e);
            }
        }

        private void ShowExceptionCarefully(Exception exception)
        {
            try
            {
                userDecisionsService.ShowException(exception, exception.Message);
            }
            catch (Exception ex)
            {
                // This can occure in UT if mock for ShowException is not defined explicitly.
                // In production we do not expect any exception here.
                LOGGER.Error("Unexpected exception of userDecisionsService", ex);
            }
        }
    }
}
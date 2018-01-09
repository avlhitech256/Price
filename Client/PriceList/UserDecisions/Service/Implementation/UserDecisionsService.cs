using System;
using System.Windows;
using UserDecisions.View;
using UserDecisions.ViewModel;

namespace UserDecisions.Service.Implementation
{
    public class UserDecisionsService : IUserDecisionsService
    {
        public void ShowException(Exception e)
        {
            ErrorViewModel viewModel = new ErrorViewModel();
            Window window = new Window
            {
                Width = 800,
                Content = new ErrorControl(),
                DataContext = viewModel
            };

            viewModel.SetException(e);
            viewModel.CloseExceptionWindow = window.Close;
            window.ShowDialog();
        }

        //public void UnionClearRequest(string message, Action<Answer> action)
        //{
        //    Window window = new Window();
        //    {
        //        //window.
        //    };

        //}
    }
}

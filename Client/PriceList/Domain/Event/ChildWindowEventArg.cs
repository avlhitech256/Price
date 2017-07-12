using System;
using System.Windows.Controls;
using Domain.ViewModel;

namespace Domain.Event
{
    public class ChildWindowEventArg : EventArgs
    {
        #region Constructors

        public ChildWindowEventArg(UserControl view, IChildWindowViewModel viewModel)
        {
            View = view;
            ViewModel = viewModel;
        }

        #endregion

        #region Properties

        public UserControl View { get; }

        public IChildWindowViewModel ViewModel { get; }

        #endregion
    }
}

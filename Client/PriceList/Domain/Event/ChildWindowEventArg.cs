using System;
using System.Web.UI;

namespace Domain.Event
{
    public class ChildWindowEventArg : EventArgs
    {
        #region Constructors

        public ChildWindowEventArg(UserControl view, object viewModel)
        {
            View = view;
            ViewModel = viewModel;
        }

        #endregion

        #region Properties

        public UserControl View { get; set; }

        public object ViewModel { get; }

        #endregion
    }
}

using Common.ViewModel.Command;

namespace Photo.ViewModel.Command
{
    internal class CloseCommand : CommonCommand
    {
        #region Constructors

        public CloseCommand(object viewModel) : base(viewModel)
        {
            CanExecuteProperty = true;
        }

        #endregion

        #region Properties

        private PhotoViewModel PhotoViewModel => (PhotoViewModel)ViewModel;

        #endregion

        #region Methods

        public override void Execute(object parameter)
        {
            PhotoViewModel?.Close();
        }

        #endregion
    }
}

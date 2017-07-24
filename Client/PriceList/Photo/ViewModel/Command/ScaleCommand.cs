using System.ComponentModel;
using Domain.ViewModel.Command;

namespace Photo.ViewModel.Command
{
    internal class ScaleCommand : CommonCommand
    {
        #region Constructors

        public ScaleCommand(object viewModel) : base(viewModel)
        {
            if (PhotoViewModel != null)
            {
                PhotoViewModel.PropertyChanged += PhotoViewModel_PropertyChanged;
                CanExecuteProperty = CalculateCanExecute();
            }

        }

        #endregion

        #region Properties

        private PhotoViewModel PhotoViewModel => (PhotoViewModel)ViewModel;

        #endregion

        #region Methods

        private void PhotoViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PhotoViewModel.SelectedItem))
            {
                CanExecuteProperty = CalculateCanExecute();
            }
        }

        private bool CalculateCanExecute()
        {
            return PhotoViewModel?.SelectedItem != null;
        }

        public override void Execute(object parameter)
        {
            PhotoViewModel?.Scale();
        }

        #endregion
    }
}

using System.ComponentModel;
using System.Linq;
using Domain.ViewModel.Command;

namespace Photo.ViewModel.Command
{
    internal class PreviousCommand : CommonCommand
    {
        #region Constructors

        public PreviousCommand(object viewModel) : base(viewModel)
        {
            if (PhotoViewModel != null)
            {
                PhotoViewModel.PropertyChanged += PhotoViewModel_PropertyChanged;
                PhotoViewModel.Entities.CollectionChanged += Entities_CollectionChanged;
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
            if (e.PropertyName == nameof(PhotoViewModel.Entities) ||
                e.PropertyName == nameof(PhotoViewModel.SelectedItem))
            {
                CanExecuteProperty = CalculateCanExecute();
            }
        }

        private void Entities_CollectionChanged(object sender,
                                                System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CanExecuteProperty = CalculateCanExecute();
        }

        private bool CalculateCanExecute()
        {
            bool result = (PhotoViewModel?.Entities?.Any() ?? false) &&
                          !Equals(PhotoViewModel.Entities.FirstOrDefault(), PhotoViewModel?.SelectedItem);
            return result;
        }

        public override void Execute(object parameter)
        {
            PhotoViewModel?.Previous();
        }

        #endregion
    }

}

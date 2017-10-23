using System;
using Catalog.Model;
using Common.Data.Notifier;
using Common.ViewModel.Command;

namespace Catalog.ViewModel
{
    public class CatalogNavigateViewModel : Notifier
    {
        #region Members

        public Action<LoadingType> LoadData;

        #endregion

        #region Constructors

        public CatalogNavigateViewModel(CatalogModel model)
        {
            LoadData = delegate { };
            Model = model;
            CreateCommands();
            SubscribeEvetns();
        }

        #endregion

        #region Properties

        private CatalogModel Model { get; }

        public DelegateCommand FirstCommand { get; private set; }

        public DelegateCommand PreviousCommand { get; private set; }

        public DelegateCommand NextCommand { get; private set; }

        public DelegateCommand LastCommand { get; private set; }

        public DelegateCommand RefreshEntities { get; set; }

        private int StartRowIndex
        {
            get
            {
                return Model?.StartRowIndex ?? 0;
            }
            set
            {
                if (Model != null && Model.StartRowIndex != value)
                {
                    Model.StartRowIndex = value;
                }
            }
        }

        public int MaximumRows
        {
            get
            {
                return Model?.MaximumRows ?? 0;
            }
            set
            {
                if (Model != null && Model.MaximumRows != value)
                {
                    Model.MaximumRows = value > 100 ? 100 : value < 5 ? 5 : value;
                    OnPropertyChanged();
                }
            }
        }

        private int Count => Model?.Count ?? 0;

        public int CurrentPage
        {
            get
            {
                return (StartRowIndex / MaximumRows) + 1;
            }
            set
            {
                if (CurrentPage != value)
                {
                    StartRowIndex = (value - 1) * MaximumRows;
                    OnPropertyChanged();
                }
            }
        }

        public int CountOfPages => (int) Math.Ceiling(((decimal)Count)/((decimal)MaximumRows));

        public string StringCountOfPages => $"страница из {CountOfPages}";

        #endregion

        #region Methods

        private void SubscribeEvetns()
        {
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.MaximumRows) || 
                e.PropertyName == nameof(Model.StartRowIndex) || 
                e.PropertyName == nameof(Model.Count))
            {
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CountOfPages));
                OnPropertyChanged(nameof(StringCountOfPages));
                FirstCommand.RiseCanExecute();
                PreviousCommand.RiseCanExecute();
                NextCommand.RiseCanExecute();
                LastCommand.RiseCanExecute();
            }
        }

        private void CreateCommands()
        {
            FirstCommand = new DelegateCommand(GoToFirst, CanGoToFirst);
            PreviousCommand = new DelegateCommand(GoToPrevious, CanGoToPrevious);
            NextCommand = new DelegateCommand(GoToNext, CanGoToNext);
            LastCommand = new DelegateCommand(GoToLast, CanGoToLast);
        }

        private void GoToFirst(object parametr)
        {
            Model.StartRowIndex = 0;
            LoadData(LoadingType.ChangedSelectedPage);
            //Model.SelectEntities();
        }

        private bool CanGoToFirst(object parametr)
        {
            return Model.StartRowIndex != 0;
        }

        private void GoToPrevious(object parametr)
        {
            StartRowIndex = StartRowIndex >= MaximumRows ? StartRowIndex - MaximumRows : 0;
            LoadData(LoadingType.ChangedSelectedPage);
            //Model.SelectEntities();
        }

        private bool CanGoToPrevious(object parametr)
        {
            return StartRowIndex > 0;
        }

        private void GoToNext(object parametr)
        {
            StartRowIndex = StartRowIndex + MaximumRows;
            LoadData(LoadingType.ChangedSelectedPage);
            //Model.SelectEntities();
        }

        private bool CanGoToNext(object parametr)
        {
            return StartRowIndex < Count - MaximumRows;
        }

        private void GoToLast(object parametr)
        {
            Model.StartRowIndex = (CountOfPages - 1) * MaximumRows;
            LoadData(LoadingType.ChangedSelectedPage);
            //Model.SelectEntities();
        }

        private bool CanGoToLast(object parametr)
        {
            return StartRowIndex < Count - MaximumRows;
        }

        public void ValidateCurrentPage()
        {
            StartRowIndex = CurrentPage > CountOfPages
                ? (CountOfPages - 1) * MaximumRows
                : CurrentPage < 1 ? 0 : (CurrentPage - 1) * MaximumRows;
        }

        public void ValidateMaximumRows()
        {
            MaximumRows = MaximumRows > 100 ? 100 : MaximumRows < 5 ? 5 : MaximumRows;
        }

        public void LoadCurrentPage()
        {
            Model.SelectEntities();
        }

        #endregion
    }
}

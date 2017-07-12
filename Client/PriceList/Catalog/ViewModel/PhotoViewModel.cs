﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.ViewModel.Command;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.DomainContext;
using Domain.Event;
using Domain.ViewModel;

namespace Catalog.ViewModel
{
    public class PhotoViewModel : Notifier, IChildWindowViewModel
    {
        #region Members

        private PhotoModel model;
        private ICommand nextCommand;
        private ICommand previousCommand;
        private ICommand scaleCommand;
        private ICommand closeCommand;
        private bool fullScale;

        #endregion

        #region Constructors

        public PhotoViewModel(IDomainContext domainContext, PhotoModel model)
        {
            DomainContext = domainContext;
            Model = model;
            FullScale = false;
            NextCommand = new NextCommand(this);
            PreviousCommand = new PreviousCommand(this);
            ScaleCommand = new ScaleCommand(this);
            CloseCommand = new CloseCommand(this);
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;


        private PhotoModel Model
        {
            get
            {
                return model;
            }
            set
            {
                if (model != value)
                {
                    model = value;
                    SubscribeModel();
                }
            }
        }

        public BitmapSource SelectedItem => Model?.SelectedItem;

        public ObservableCollection<BitmapSource> Entities => Model?.Entities;

        public  bool FullScale
        {
            get
            {
                return fullScale;
            }
            set
            {
                if (fullScale != value)
                {
                    fullScale = value;
                    SubscribeModel();
                }
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return nextCommand;
            }
            private set
            {
                if (!Equals(nextCommand, value))
                {
                    nextCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                return previousCommand;
            }
            private set
            {
                if (!Equals(previousCommand, value))
                {
                    previousCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ScaleCommand
        {
            get
            {
                return scaleCommand;
            }
            private set
            {
                if (!Equals(scaleCommand, value))
                {
                    scaleCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return closeCommand;
            }
            private set
            {
                if (!Equals(closeCommand, value))
                {
                    closeCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void SubscribeModel()
        {
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
            }
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || e.PropertyName == nameof(Entities))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }

        public void Next()
        {
            Model.Next();
        }

        public void Previous()
        {
            Model.Previous();
        }

        public void Close()
        {
            HideWindow?.Invoke();
        }

        public void Scale()
        {
            FullScale = !FullScale;
            Messenger?.Send(CommandName.SetPhotoWindowState, new ChildWindowScaleEventArgs(FullScale));
        }

        #endregion

        #region Events

        public HideWindow HideWindow { get; set; }

        #endregion
    }
}

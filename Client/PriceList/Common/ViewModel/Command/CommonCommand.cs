﻿using System;
using System.Windows.Input;

namespace Common.ViewModel.Command
{
    abstract public class CommonCommand : ICommand
    {
        #region Members

        private bool canExecute;

        #endregion

        #region Constructors

        protected CommonCommand(object viewModel)
        {
            ViewModel = viewModel;
            CanExecuteProperty = false;
        }

        #endregion

        #region Properties

        protected object ViewModel { get; }

        protected bool CanExecuteProperty
        {
            get
            {
                return canExecute;
            }

            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    OnCanExecuteChanged();
                }
            }

        }

        #endregion

        #region Methods

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }


        public virtual bool CanExecute(object parameter)
        {
            return CanExecuteProperty;
        }

        public abstract void Execute(object parameter);

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged = delegate { };

        #endregion
    }
}

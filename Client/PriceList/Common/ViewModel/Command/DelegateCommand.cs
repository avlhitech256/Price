using System;
using System.Windows.Input;

namespace Common.ViewModel.Command
{
    public class DelegateCommand : ICommand
    {
        #region Members

        private Func<object, bool> canExecute;
        private object value;
        private bool canExecuteProperty;

        #endregion

        #region Constructors

        public DelegateCommand() : this(null) { }

        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute) 
            : this(execute, canExecute, null) { }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute, object value)
        {
            this.ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute;
            this.value = value;
            CanExecuteProperty = CanExecute(value);
        }

        #endregion

        #region Properties

        protected object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    RiseCanExecute(value);
                }
            }
        }

        protected Action<object> ExecuteDelegate { get; set; }

        protected Func<object, bool> CanExecuteDelegate
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
                    RiseCanExecute();
                }
            }
        } 

        protected bool CanExecuteProperty
        {
            get
            {
                return canExecuteProperty;
            }

            set
            {
                if (canExecuteProperty != value)
                {
                    canExecuteProperty = value;
                    OnCanExecuteChanged();
                }
            }

        }

        #endregion

        #region Methods

        public void RiseCanExecute(object parameter = null)
        {
            CanExecuteProperty = CanExecute(parameter ?? value);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter ?? value) ?? true;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter ?? value))
            {
                ExecuteDelegate?.Invoke(parameter ?? value);
            }
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}

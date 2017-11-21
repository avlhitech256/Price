using System;

namespace Load.Service.Implementation
{
    public class LoadingThreadInfo
    {
        #region Members

        private bool isLoading;

        #endregion

        #region Constructors

        public LoadingThreadInfo()
        {
            isLoading = false;
        }

        #endregion
        
        #region Properties

        public bool IsLoading 
        {
            get
            {
                return isLoading;
            }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;

                    if (value)
                    {
                        OnStart();
                    }
                    else
                    {
                        OnEnd();
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void OnStart()
        {
            Start?.Invoke(this, new EventArgs());
        }

        private void OnEnd()
        {
            End?.Invoke(this, new EventArgs());
        }

        #endregion
        
        #region Events

        public event EventHandler Start;

        public event EventHandler End;

        #endregion
    }
}

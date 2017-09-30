using System;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;

namespace Domain.Data.Object
{
    public class BrandItem : Notifier
    {
        #region Members

        private long position;

        #endregion

        #region Constructors

        public BrandItem(BrandItemEntity entity)
        {
            Entity = entity;
        }

        #endregion

        #region Properties

        public long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged();
                }
            }
        }

        public BrandItemEntity Entity { get; }

        public long Id => Entity.Id;

        public Guid Code => Entity.Code;

        public string Name => Entity.Name;

        #endregion
    }
}

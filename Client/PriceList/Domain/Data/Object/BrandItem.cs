using System;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;

namespace Domain.Data.Object
{
    public class BrandItem : Notifier
    {
        #region Members

        private BrandItemEntity entity;

        #endregion

        #region Constructors

        public BrandItem(BrandItemEntity entity)
        {
            Entity = entity;
        }

        #endregion

        #region Properties

        public BrandItemEntity Entity
        {
            get
            {
                return entity;
            }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    OnPropertyChanged();
                }
            }
        }

        public long Id
        {
            get
            {
                return entity.Id;
            }
            set
            {
                if (entity.Id != value)
                {
                    entity.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public Guid Code
        {
            get
            {
                return entity.Code;
            }
            set
            {
                if (entity.Code != value)
                {
                    entity.Code = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return entity.Name;
            }
            set
            {
                if (entity.Name != value)
                {
                    entity.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}

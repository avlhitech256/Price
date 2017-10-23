using System;
using System.Collections.Generic;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;

namespace Domain.Data.Object
{
    public class BrandItem : Notifier
    {
        #region Members

        private long position;
        private bool selected;

        #endregion

        #region Constructors

        public BrandItem(BrandItemEntity entity)
        {
            Entity = entity;
            Selected = false;
            Subbrands = new List<BrandItem>();
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

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<BrandItem> Subbrands { get; }

        #endregion
    }
}

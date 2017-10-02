using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Media.Image;

namespace Domain.Data.Object
{
    public class DirectoryItem : Notifier
    {
        #region Members

        private DirectoryEntity entity;
        private bool selected;
        private List<DirectoryItem> subdirectories;

        #endregion

        #region Constructors

        public DirectoryItem(DirectoryEntity entity)
        {
            Entity = entity;
            Selected = false;
            Subdirectories = new List<DirectoryItem>();
        }

        #endregion

        #region Properties

        public DirectoryEntity Entity
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
                return Entity.Id;
            }
            set
            {
                if (Entity.Id != value)
                {
                    Entity.Id = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public Guid Code
        {
            get
            {
                return Entity.Code;
            }
            set
            {
                if (Entity.Code != value)
                {
                    Entity.Code = value;
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

        public List<DirectoryItem> Subdirectories
        {
            get
            {
                return subdirectories;
            }
            set
            {
                if (subdirectories != value)
                {
                    subdirectories = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}

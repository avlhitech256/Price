using System;
using System.Collections.Generic;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;

namespace Domain.Data.Object
{
    public class DirectoryItem : Notifier
    {
        #region Members

        private DirectoryEntity entity;
        private bool selected;
        private DirectoryItem parent;
        private readonly IDataService dataService;

        #endregion

        #region Constructors

        public DirectoryItem(IDataService dataService, DirectoryEntity entity)
        {
            this.dataService = dataService;
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

        public DirectoryItem Parent
        {
            get
            {
                return parent;
            }
            set
            {
                DirectoryEntity parentValue = LoadParent();

                if (parent != value && (value == null || parentValue == null || value.Id == parentValue.Id))
                {
                    parent = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<DirectoryItem> Subdirectories { get; }

        #endregion

        #region Methods

        private DirectoryEntity LoadParent()
        {
            dataService.LoadParent(Entity);
            return Entity.Parent;
        }

        #endregion
    }
}

﻿using System.ComponentModel;
using Common.Annotations;
using Common.Messenger;
using Domain.DomainContext;

namespace Domain.ViewModel
{
    public interface IControlViewModel : INotifyPropertyChanged
    {
        [CanBeNull]
        IDomainContext DomainContext { get; }

        [CanBeNull]
        IMessenger Messenger { get; }

        bool ReadOnly { get; set; }

        bool Enabled { get; set; }

        bool IsEditControl { get; set; }

        bool HasChanges { get; }

        void ApplySearchCriteria();

        void Clear();

        void Add();

        void View();

        void Edit();

        bool Save();

        void Delete();
    }
}

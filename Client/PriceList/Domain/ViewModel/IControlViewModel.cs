using System;
using System.ComponentModel;
using Common.Annotations;
using Common.Messenger;
using Domain.DomainContext;

namespace Domain.ViewModel
{
    public interface IControlViewModel : INotifyPropertyChanged//, ILoadingContext
    {
        [CanBeNull]
        IDomainContext DomainContext { get; }

        [CanBeNull]
        IMessenger Messenger { get; }

        bool ReadOnly { get; set; }

        bool Enabled { get; set; }

        bool IsEditControl { get; set; }

        bool HasChanges { get; }

        Action RefreshView { get; set; } 

        Action<bool> SetEnabled { get; set; }

        Func<bool> HasResultGridErrors { get; set; }

        void ApplySearchCriteria();

        void Clear();

        void Add();

        void View();

        void Edit();

        bool Save();

        void Delete();

        void Init();
    }
}

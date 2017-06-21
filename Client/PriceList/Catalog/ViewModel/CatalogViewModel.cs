using System.ComponentModel;
using Common.Messenger;
using Domain.DomainContext;
using Domain.ViewModel;

namespace Catalog.ViewModel
{
    public class CatalogViewModel : IControlViewModel
    {
        public CatalogViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            HasChanges = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public IDomainContext DomainContext { get; }
        public IMessenger Messenger => DomainContext?.Messenger;
        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }
        public bool IsEditControl { get; set; }
        public bool HasChanges { get; }
        public void ApplySearchCriteria()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void View()
        {
            throw new System.NotImplementedException();
        }

        public void Edit()
        {
            throw new System.NotImplementedException();
        }

        public bool Save()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}

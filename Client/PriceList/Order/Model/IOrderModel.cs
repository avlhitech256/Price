using System.Collections.ObjectModel;
using System.ComponentModel;
using Domain.Data.Object;
using Order.SearchCriteria;

namespace Order.Model
{
    public interface IOrderModel : INotifyPropertyChanged
    {
        OrderSearchCriteria SearchCriteria { get; }

        OrderItem SelectedItem { get; set; }

        ObservableCollection<OrderItem> Entities { get; set; }

        void SelectEntities();

        void Clear();
    }
}

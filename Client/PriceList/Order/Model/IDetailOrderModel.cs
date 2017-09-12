using System.Collections.ObjectModel;
using System.ComponentModel;
using Domain.Data.Object;

namespace Order.Model
{
    public interface IDetailOrderModel : INotifyPropertyChanged
    {
        OrderItem CurrentOrder { get; set; }

        string Capture { get; }

        BasketItem SelectedItem { get; set; }

        ObservableCollection<BasketItem> Entities { get; set; }

        void SelectEntities();

        void Clear();
    }

}

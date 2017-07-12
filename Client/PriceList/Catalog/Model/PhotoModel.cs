using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using Common.Data.Notifier;

namespace Catalog.Model
{
    public class PhotoModel : Notifier
    {
        #region Members

        private BitmapSource selectedItem;
        private ObservableCollection<BitmapSource> entities;
        private int currentItemIndex;

        #endregion

        #region Constructors

        public PhotoModel(ObservableCollection<BitmapSource> entities)
        {
            currentItemIndex = -2;
            Entities = entities;
        }

        #endregion

        #region Properties

        public BitmapSource SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (!Equals(selectedItem, value))
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<BitmapSource> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                if (entities != value)
                {
                    entities = value ?? new ObservableCollection<BitmapSource>();
                    CurrentItemIndex = Entities.Any() ? 0 : -1;
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentItemIndex
        {
            get
            {
                return currentItemIndex;
            }
            private set
            {
                int newValue = value < 0 && Entities.Any() 
                    ? 0 
                    : value >= Entities.Count 
                        ? Entities.Count - 1 
                        : value;

                if (currentItemIndex != newValue)
                {
                    currentItemIndex = newValue;
                    int index = newValue < 0 ? 0 : newValue;
                    SelectedItem = Entities.ElementAtOrDefault(index);
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public void Next()
        {
            CurrentItemIndex++;
        }

        public void Previous()
        {
            CurrentItemIndex--;
        }

        #endregion
    }
}

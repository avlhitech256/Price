using System.Windows.Media;
using Common.Data.Notifier;
using Domain.Data.Enum;
using Domain.DomainContext;
using Media.Color;

namespace PriceList.ViewModel.TopMenu
{
    public class MenuItemStyle : Notifier
    {
        #region Members

        private MenuItemName name;
        private LinearGradientBrush backgroundColorBrush;
        private LinearGradientBrush notSelectedAndMouseIsNotOverColorBrush;
        private LinearGradientBrush notSelectedAndMouseIsOverColorBrush;
        private LinearGradientBrush selectedAndMouseIsNotOverColorBrush;
        private LinearGradientBrush selectedAndMouseIsOverColorBrush;

        private bool selected;
        private bool isMouseOver;

        #endregion

        #region Constructors

        public MenuItemStyle(IDomainContext domainContext) : this(MenuItemName.PriceList, domainContext, null, null, null, null)
        {
        }

        public MenuItemStyle(MenuItemName name, IDomainContext domainContext,
                             LinearGradientBrush notSelectedAndMouseIsNotOverColorBrush,
                             LinearGradientBrush notSelectedAndMouseIsOverColorBrush,
                             LinearGradientBrush selectedAndMouseIsNotOverColorBrush,
                             LinearGradientBrush selectedAndMouseIsOverColorBrush)
        {
            this.name = name;
            DomainContext = domainContext;
            this.notSelectedAndMouseIsNotOverColorBrush = notSelectedAndMouseIsNotOverColorBrush;
            this.notSelectedAndMouseIsOverColorBrush = notSelectedAndMouseIsOverColorBrush;
            this.selectedAndMouseIsNotOverColorBrush = selectedAndMouseIsNotOverColorBrush;
            this.selectedAndMouseIsOverColorBrush = selectedAndMouseIsOverColorBrush;
            SetDefaultBackgroundColors();

            backgroundColorBrush = notSelectedAndMouseIsNotOverColorBrush;
            selected = false;
            isMouseOver = false;
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IColorService ColorService => DomainContext?.ColorService;

        public MenuItemName Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }

            }

        }

        public LinearGradientBrush NotSelectedAndMouseIsNotOverBackgroundColor
        {
            get
            {
                return notSelectedAndMouseIsNotOverColorBrush;
            }

            set
            {
                if (notSelectedAndMouseIsNotOverColorBrush != value)
                {
                    notSelectedAndMouseIsNotOverColorBrush = value;
                    OnPropertyChanged();
                    ChangeBackgroundBrush();
                }
            }
        }

        public LinearGradientBrush NotSelectedAndMouseIsOverBackgroundColor
        {
            get
            {
                return notSelectedAndMouseIsOverColorBrush;
            }

            set
            {
                if (notSelectedAndMouseIsOverColorBrush != value)
                {
                    notSelectedAndMouseIsOverColorBrush = value;
                    OnPropertyChanged();
                    ChangeBackgroundBrush();
                }

            }

        }

        public LinearGradientBrush SelectedAndMouseIsNotOverBackgroundColor
        {
            get
            {
                return selectedAndMouseIsNotOverColorBrush;
            }

            set
            {
                if (selectedAndMouseIsNotOverColorBrush != value)
                {
                    selectedAndMouseIsNotOverColorBrush = value;
                    OnPropertyChanged();
                    ChangeBackgroundBrush();
                }

            }

        }

        public LinearGradientBrush SelectedAndMouseIsOverBackgroundColor
        {
            get
            {
                return selectedAndMouseIsOverColorBrush;
            }

            set
            {
                if (selectedAndMouseIsOverColorBrush != value)
                {
                    selectedAndMouseIsOverColorBrush = value;
                    OnPropertyChanged();
                    ChangeBackgroundBrush();
                }

            }

        }

        public bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }

            set
            {
                if (isMouseOver != value)
                {
                    isMouseOver = value;
                    OnPropertyChanged();
                    ChangeBackgroundBrush();
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
                    ChangeBackgroundBrush();
                }

            }

        }

        public LinearGradientBrush Background
        {
            get
            {
                return backgroundColorBrush;
            }

            set
            {
                if (backgroundColorBrush != value)
                {
                    backgroundColorBrush = value;
                    OnPropertyChanged();
                }

            }

        }

        #endregion

        #region Methods

        private void SetDefaultBackgroundColors()
        {
            if (NotSelectedAndMouseIsNotOverBackgroundColor == null)
            {
                NotSelectedAndMouseIsNotOverBackgroundColor = ColorService.CreateBrush(0x80, 0x80, 0x80);
            }

            if (NotSelectedAndMouseIsOverBackgroundColor == null)
            {
                NotSelectedAndMouseIsOverBackgroundColor = ColorService.CreateBrush(0x64, 0x64, 0x64);
            }

            if (SelectedAndMouseIsNotOverBackgroundColor == null)
            {
                SelectedAndMouseIsNotOverBackgroundColor = ColorService.CreateBrush(0x47, 0x47, 0xB8);
            }

            if (SelectedAndMouseIsOverBackgroundColor == null)
            {
                SelectedAndMouseIsOverBackgroundColor = ColorService.CreateBrush(0x67, 0x67, 0xD8);
            }

        }

        private void ChangeBackgroundBrush()
        {
            Background = Selected
                ? (IsMouseOver ? SelectedAndMouseIsOverBackgroundColor : SelectedAndMouseIsNotOverBackgroundColor)
                : (IsMouseOver ? NotSelectedAndMouseIsOverBackgroundColor : NotSelectedAndMouseIsNotOverBackgroundColor);
        }

        public bool Equals(MenuItemStyle other)
        {
            bool result = Selected == other.Selected &&
                          IsMouseOver == other.IsMouseOver &&
                          Background == other.Background &&
                          NotSelectedAndMouseIsNotOverBackgroundColor == other.NotSelectedAndMouseIsNotOverBackgroundColor &&
                          NotSelectedAndMouseIsOverBackgroundColor == other.NotSelectedAndMouseIsOverBackgroundColor &&
                          SelectedAndMouseIsNotOverBackgroundColor == other.SelectedAndMouseIsNotOverBackgroundColor &&
                          SelectedAndMouseIsOverBackgroundColor == other.SelectedAndMouseIsOverBackgroundColor;

            return result;
        }

        #endregion
    }
}

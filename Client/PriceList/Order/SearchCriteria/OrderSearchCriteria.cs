using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.Annotations;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Service;

namespace Order.SearchCriteria
{
    public class OrderSearchCriteria : Notifier
    {
        #region Members

        private OrderStatus orderStatus;
        private string orderStatusName;
        private DateTimeOffset? fromDateTime;
        private DateTimeOffset? toDateTime;
        private bool isModified;
        private bool isEmpty;
        private OrderStatus oldOrderStatus;
        private DateTimeOffset? oldFromDateTime;
        private DateTimeOffset? oldToDateTime;
        private Dictionary<OrderStatus, string> statusList;
        private IConvertService convertService;

        #endregion

        #region Constructors

        public OrderSearchCriteria(IConvertService convertService)
        {
            this.convertService = convertService;
            Clear();
            CopyValueToOld();
            InitProperties();
        }

        #endregion

        #region Properties

        public Dictionary<OrderStatus, string> StatusList
        {
            get
            {
                return statusList;
            }
            set
            {
                if (statusList != value)
                {
                    statusList = value;
                    var a = StatusList.ElementAt(1);
                    OnPropertyChanged();
                }
            }
        }

        public OrderStatus OrderStatus
        {
            get
            {
                return orderStatus;
            }
            set
            {
                if (orderStatus != value)
                {
                    orderStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OrderStatusName
        {
            get
            {
                return orderStatusName;
            }
            set
            {
                if (orderStatusName != value)
                {
                    orderStatusName = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTimeOffset? FromDateTime
        {
            get
            {
                return fromDateTime;
            }
            set
            {
                if (fromDateTime != value)
                {
                    fromDateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTimeOffset? ToDateTime
        {
            get
            {
                return toDateTime;
            }
            set
            {
                if (toDateTime != value)
                {
                    toDateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsModified
        {
            get
            {
                return isModified;
            }
            private set
            {
                if (isModified != value)
                {
                    isModified = value;
                    OnPropertyChanged();
                    OnSearchCriteriaChanged();
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged();
                    OnSearchCriteriaCleared();
                }
            }
        }


        #endregion

        #region Methods

        private void InitProperties()
        {
            InitStatusList();
            FromDateTime = null;
            ToDateTime = null;
        }

        private void InitStatusList()
        {
            StatusList = new Dictionary<OrderStatus, string>
            {
                {OrderStatus.All, convertService.Convert(OrderStatus.All)},
                {OrderStatus.New, convertService.Convert(OrderStatus.New)},
                {OrderStatus.SentOut, convertService.Convert(OrderStatus.SentOut)},
                {OrderStatus.Adopted, convertService.Convert(OrderStatus.Adopted)},
                {OrderStatus.Approved, convertService.Convert(OrderStatus.Approved)},
                {OrderStatus.Cancel, convertService.Convert(OrderStatus.Cancel)},
                {OrderStatus.InWork, convertService.Convert(OrderStatus.InWork)},
                {OrderStatus.Shipped, convertService.Convert(OrderStatus.Shipped)},
                {OrderStatus.InTransit, convertService.Convert(OrderStatus.InTransit)},
                {OrderStatus.Fulfilled, convertService.Convert(OrderStatus.Fulfilled)},
            };

            OrderStatus = OrderStatus.All;
            OrderStatusName = convertService.Convert(OrderStatus);
        }

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(IsModified) &&
                propertyName != nameof(IsEmpty))
            {
                IsModified = GetModifyStatus();
            }

            IsEmpty = SearchCriteriaIsEmpty();
        }

        public void SearchComplited()
        {
            CopyValueToOld();
            IsModified = false;
        }

        private void CopyValueToOld()
        {
            oldOrderStatus = OrderStatus;
            oldFromDateTime = FromDateTime;
            oldToDateTime = ToDateTime;
        }

        private bool GetModifyStatus()
        {
            return OrderStatus != oldOrderStatus ||
                   FromDateTime.HasValue != oldFromDateTime.HasValue || 
                   (FromDateTime.HasValue && oldFromDateTime.HasValue && FromDateTime != oldFromDateTime) ||
                   ToDateTime.HasValue != oldToDateTime.HasValue ||
                   (ToDateTime.HasValue && oldToDateTime.HasValue && ToDateTime != oldToDateTime);
        }

        public void Clear()
        {
            OrderStatus = OrderStatus.All;
            FromDateTime = null;
            ToDateTime = null;
        }

        private void OnSearchCriteriaChanged()
        {
            SearchCriteriaChanged?.Invoke(this, new EventArgs());
        }

        private void OnSearchCriteriaCleared()
        {
            SearchCriteriaCleared?.Invoke(this, new EventArgs());
        }

        private bool SearchCriteriaIsEmpty()
        {
            return OrderStatus == OrderStatus.All &&
                   !FromDateTime.HasValue &&
                   !ToDateTime.HasValue;
        }

        #endregion

        #region

        public event EventHandler SearchCriteriaChanged;

        public event EventHandler SearchCriteriaCleared;

        #endregion
    }
}

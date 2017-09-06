using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Annotations;
using Common.Data.Notifier;
using DatabaseService.Objects.Enum;

namespace Order.SearchCriteria
{
    public class OrderSearchCriteria : Notifier
    {
        #region Members

        private OrderStatus orderStatus;
        private DateTime? fromDateTime;
        private DateTime? toDateTime;
        private bool isModified;
        private bool isEmpty;
        private OrderStatus oldOrderStatus;
        private DateTime? oldFromDateTime;
        private DateTime? oldToDateTime;
        private Dictionary<OrderStatus, string> statusList; 

        #endregion

        #region Constructors

        public OrderSearchCriteria()
        {
            Clear();
            CopyValueToOld();
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

        public DateTime? FromDateTime
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

        public DateTime? ToDateTime
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

using System;
using System.Collections.Generic;
using Basket.ViewModel;
using Catalog.ViewModel;
using Common.Data.Enum;
using Domain.DomainContext;
using Synchronize.ViewModel;

namespace PriceList.ViewModel.MainWindow
{
    public class ViewModelFactory
    {
        #region Members

        private readonly Dictionary<MenuItemName, Func<IDomainContext, object>> mapCreators;
        private readonly IDomainContext context;

        #endregion

        #region Constructors

        public ViewModelFactory(IDomainContext context)
        {
            mapCreators =
                new Dictionary<MenuItemName, Func<IDomainContext, object>>
                {
                    {MenuItemName.PriceList, (x) => new CatalogViewModel(x)},
                    {MenuItemName.Basket,    (x) => new BasketViewModel(x)},
                    {MenuItemName.Orders,    (x) => new Order.ViewModel.OrderViewModel(x)},
                    {MenuItemName.Documents, (x) => null /*new SpecialtyViewModel(x)*/},
                    {MenuItemName.Sync,      (x) => new SynchronizeViewModel(x)},
                    {MenuItemName.Settings,  (x) => null /*new SpecializationViewModel(x)*/}
                };
            this.context = context;
        }

        #endregion

        #region Methods

        public object Create(MenuItemName menuItemName)
        {
            object result = null;

            if (mapCreators.ContainsKey(menuItemName))
            {
                Func<IDomainContext, object> creator = mapCreators[menuItemName];
                result = creator(context);
            }

            return result;
        }

        #endregion
    }
}

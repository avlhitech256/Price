using System;
using System.Collections.Generic;
using Catalog.ViewModel;
using Domain.Data.Enum;
using Domain.DomainContext;

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
                    {MenuItemName.Basket, (x) => null },
                    {MenuItemName.Orders,    (x) => null /*new FacultyViewModel(x)*/},
                    {MenuItemName.Documents, (x) => null /*new SpecialtyViewModel(x)*/},
                    {MenuItemName.Sync,      (x) => null /*new ChairViewModel(x)*/},
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

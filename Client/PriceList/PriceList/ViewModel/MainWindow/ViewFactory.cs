using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommonControl.EditControl;
using CommonControl.SearchControl;
using Domain.Data.Enum;
using Domain.DomainContext;
using Domain.ViewModel;
using CatalogControl = Catalog.View.CatalogControl;

namespace PriceList.ViewModel.MainWindow
{
    public class ViewFactory
    {
        #region Members

        private readonly Dictionary<MenuItemName, Func<object>> mapSearchControlFactories;
        private readonly Dictionary<MenuItemName, Func<object>> mapEditControlFactories;
        private readonly IDomainContext domainContext;
        private readonly ViewModelRouter viewModelRouter;

        #endregion

        #region Constructors

        public ViewFactory(IDomainContext domainContext, ViewModelRouter viewModelRouter)
        {
            this.domainContext = domainContext;
            this.viewModelRouter = viewModelRouter;

            mapSearchControlFactories =
                new Dictionary<MenuItemName, Func<object>>
                {
                    {MenuItemName.PriceList, () => new CatalogControl()},
                    {MenuItemName.Orders,    () => null /*new FacultySearchControl()*/},
                    {MenuItemName.Documents, () => null /*new SpecialtySearchControl()*/},
                    {MenuItemName.Sync,      () => null /*new ChairSearchControl()*/},
                    {MenuItemName.Settings,  () => null /*new SpecializationSearchControl()*/}
                };

            mapEditControlFactories =
                new Dictionary<MenuItemName, Func<object>>
                {
                    {MenuItemName.PriceList, () => null /*new HighSchoolEditControl()*/},
                    {MenuItemName.Orders,    () => null /*new FacultyEditControl()*/},
                    {MenuItemName.Documents, () => null /*new SpecialtyEditControl()*/},
                    {MenuItemName.Sync,      () => null /*new ChairEditControl()*/},
                    {MenuItemName.Settings,  () => null /*new SpecializationEditControl()*/}
                };

        }

        #endregion

        #region Methods

        public object GetView(MenuItemName menuItemName, object oldView)
        {
            object view = null;
            object viewModel = viewModelRouter.GetViewModel(menuItemName);
            IControlViewModel viewModelWithInterface = viewModel as IControlViewModel;
            Func<object> factory = null;

            if (viewModelWithInterface != null)
            {
                if ((domainContext.ViewModel != viewModelWithInterface) ||
                    (domainContext.IsEditControl != viewModelWithInterface.IsEditControl))
                {
                    domainContext.ViewModel = viewModelWithInterface;
                    domainContext.IsEditControl = viewModelWithInterface.IsEditControl;

                    if (viewModelWithInterface.IsEditControl)
                    {
                        if (mapEditControlFactories != null && mapEditControlFactories.ContainsKey(menuItemName))
                        {
                            factory = mapEditControlFactories[menuItemName];
                        }

                    }
                    else
                    {
                        if (mapSearchControlFactories != null && mapSearchControlFactories.ContainsKey(menuItemName))
                        {
                            factory = mapSearchControlFactories[menuItemName];
                        }

                    }

                    if (factory != null)
                    {
                        view = factory.Invoke();
                        UserControl viewWithInterface = view as UserControl;

                        if (viewWithInterface != null)
                        {
                            viewWithInterface.DataContext = viewModel;
                        }

                        if (viewModelWithInterface.IsEditControl)
                        {
                            EditControl editView = view as EditControl;

                            if (editView != null)
                            {
                                editView.DomainContext = domainContext;
                            }

                        }
                        else
                        {
                            SearchControl searchView = view as SearchControl;

                            if (searchView != null)
                            {
                                searchView.DomainContext = domainContext;
                            }

                        }

                    }

                }
                else
                {
                    view = oldView;
                }

            }
            else
            {
                domainContext.ViewModel = null;
                domainContext.IsEditControl = false;
            }

            return view;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    public class PageServiceBuilder
    {
        private Func<INavigation> _navigationGetter;
        private PageServiceZero _pageService;
        private Func<Type, object> _typeFactory;
        private Dictionary<Type, Func<ViewMapperParameters, IView>> _viewMap;
        private readonly Func<INavigation> _defaultNavigationGetter;
        private readonly Func<Type, object> _defaultTypeFactory;

        internal PageServiceBuilder(Func<INavigation> defaultNavigationGetter, Func<Type, object> defaultTypeFactory) : this()
        {
            _defaultNavigationGetter = defaultNavigationGetter;
            _defaultTypeFactory = defaultTypeFactory;
        }

        internal PageServiceBuilder()
        {
            _viewMap = new();
        }

        public PageServiceBuilder SetNavigationGetter(Func<INavigation> navigationGetter)
        {
            if (_navigationGetter != null)
                throw new InvalidOperationException("SetNavigationGetter can be called once only!");
            _navigationGetter = navigationGetter;

            return this;
        }
        public PageServiceBuilder SetTypeFactory(Func<Type, object> typeFactory)
        {
            if (_typeFactory != null)
                throw new InvalidOperationException("SetNavigationGetter can be called once only!");
            _typeFactory = typeFactory;

            return this;
        }

        public PageServiceBuilder MapVmToPage<TViewModel>(Func<ViewMapperParameters, IView> viewFactory)
        {
            _viewMap.Add(typeof(TViewModel), viewFactory);
            return this;
        }

        public PageServiceBuilder MapVmToPage<TViewModel, TPage>(bool wrapInNavigationPage = false) where TPage : Page
        {
            Func<ViewMapperParameters, Page> getter;

            if (wrapInNavigationPage)
                getter = (ViewMapperParameters p) =>
                {
                    var page = (Page)p.PageService.GetPage<TPage>();
                    return new NavigationPage(page) { Title = page.Title };
                };
            else
                getter = (ViewMapperParameters p) => p.PageService.GetPage<TPage>();

            _viewMap.Add(typeof(TViewModel), getter);

            return this;
        }

        public PageServiceBuilder MapVmToView<TViewModel, TView>() where TView : IView
        {
            Func<ViewMapperParameters, IView> getter;

            getter = (ViewMapperParameters p) => p.PageService.GetView<TView>();

            _viewMap.Add(typeof(TViewModel), getter);

            return this;
        }

        public IPageServiceZero Build()
        {
            _typeFactory = _typeFactory ?? _defaultTypeFactory;
            _navigationGetter = _navigationGetter ?? _defaultNavigationGetter;
            _pageService = new PageServiceZero(_navigationGetter, _typeFactory, ViewMapper);
            return _pageService;
        }

        private IView ViewMapper(Type vmType, object hint)
        {

            try
            {
                var parameters = new ViewMapperParameters(vmType, this._pageService, hint);
                return _viewMap[vmType](parameters);
            }
            catch (KeyNotFoundException kex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. You must register them in UsePageServiceZero. TODO: More details.", kex);
            }
            catch (NullReferenceException nrex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. If you see this can you raise a bug please? TODO: More details.", nrex);

            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. The mapping has been registered in UsePageServiceZero but your container cannot provide a suitable view. TODO: More details.", ex);
            }
        }
    }
}

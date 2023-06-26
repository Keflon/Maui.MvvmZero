using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    public class PageServiceBuilder
    {
        private Func<INavigation> _navigationFinder;
        private PageServiceZero _pageService;
        private Func<Type, object> _typeFactory;
        private Dictionary<Type, Func<ViewMapperParameters, IView>> _viewMap;
        private readonly Func<INavigation> _defaultNavigationFinder;
        private readonly Func<MultiPage<Page>> _defaultMultiPageFinder;
        private Func<FlyoutPage> _flyoutFactory;
        private Func<MultiPage<Page>> _multiPageFinder;

        internal PageServiceBuilder(Func<INavigation> defaultNavigationFinder, Func<MultiPage<Page>> defaultMultiPageFinder/*, Func<Type, object> defaultTypeFactory*/) : this()
        {
            _defaultNavigationFinder = defaultNavigationFinder;
            _defaultMultiPageFinder = defaultMultiPageFinder;
        }

        internal PageServiceBuilder()
        {
            _viewMap = new();
        }

        public PageServiceBuilder SetNavigationFinder(Func<INavigation> navigationFinder)
        {
            if (_navigationFinder != null)
                throw new InvalidOperationException("SetNavigationFinder can be called once only!");
            _navigationFinder = navigationFinder;

            return this;
        }

        public PageServiceBuilder SetMultiPageFinder(Func<MultiPage<Page>> multiPageFinder)
        {
            if (_multiPageFinder != null)
                throw new InvalidOperationException("SetMultiPageFinder can be called once only!");
            _multiPageFinder = multiPageFinder;

            return this;
        }

        public bool HasTypeFactory => _typeFactory != null;
        public PageServiceBuilder SetTypeFactory(Func<Type, object> typeFactory)
        {
            if (_typeFactory != null)
                throw new InvalidOperationException("SetTypeFactory can be called once only!");
            _typeFactory = typeFactory;

            return this;
        }
        public PageServiceBuilder SetFlyoutFactory(Func<FlyoutPage> flyoutFactory)
        {
            if (_flyoutFactory != null)
                throw new InvalidOperationException("SetFlyoutFactory can be called once only!");
            _flyoutFactory = flyoutFactory;

            return this;
        }

        //public PageServiceBuilder MapVmToPage<TViewModel>(Func<ViewMapperParameters, IView> viewFactory)
        //{
        //    _viewMap.Add(typeof(TViewModel), viewFactory);
        //    return this;
        //}

        //public PageServiceBuilder MapVmToPage<TViewModel, TPage>() where TPage : Page
        //{
        //    var getter = (ViewMapperParameters p) => p.PageService.GetPage<TPage>();

        //    _viewMap.Add(typeof(TViewModel), getter);

        //    return this;
        //}

        public PageServiceBuilder MapVmToView<TViewModel>(Func<ViewMapperParameters, IView> viewFactory)
        {
            _viewMap.Add(typeof(TViewModel), viewFactory);
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
            //_typeFactory = _typeFactory ?? _defaultTypeFactory;
            _navigationFinder = _navigationFinder ?? _defaultNavigationFinder;
            _multiPageFinder = _multiPageFinder ?? _defaultMultiPageFinder;
            _flyoutFactory = _flyoutFactory ?? GetDefaultFlyout;
            _pageService = new PageServiceZero(_typeFactory, _flyoutFactory, _navigationFinder, _multiPageFinder,  ViewMapper);

            return _pageService;
        }

        private FlyoutPage GetDefaultFlyout()
        {
            return (FlyoutPage)_typeFactory(typeof(FlyoutPage));
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
                string shortType = Path.GetExtension(vmType.ToString()).Substring(1);
                string message = $"ERROR: Cannot resolve the View for type {shortType}\r\n";
                message += "You must register a View for a ViewModel in UsePageServiceZero in the CreateMauiApp method.\r\n";
                message += "\r\n";
                message += "Like this:\r\n";
                message += "\r\n";
                message += $".UsePageServiceZero(config =>\r\n{{\r\n    config.MapVmToPage<{shortType}, SomePage>();\r\n    ...\r\n}})";
                throw new ViewMapperException(message, vmType, kex);
            }
            catch (NullReferenceException nrex)
            {
                throw new ViewMapperException($"Cannot resolve the View for type {vmType}. If you see this can you raise a bug please?", vmType, nrex);
            }
            catch (TypeFactoryException ex)
            {
                throw new ViewMapperException($"ERROR: Cannot resolve the View for type {vmType}. The mapping has been registered in UsePageServiceZero but your container cannot provide a suitable view instance.", vmType, ex);
            }
            catch
            {
                throw;
            }
        }
    }
}

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
        private Dictionary<Type, Func<ViewFinderParameters, IView>> _viewFinder;
        private readonly Func<INavigation> _defaultNavigationGetter;
        private readonly Func<Type, object> _defaultTypeFactory;

        internal PageServiceBuilder(Func<INavigation> defaultNavigationGetter, Func<Type, object> defaultTypeFactory) : this()
        {
            _defaultNavigationGetter = defaultNavigationGetter;
            _defaultTypeFactory = defaultTypeFactory;
        }

        internal PageServiceBuilder()
        {
            _viewFinder = new();
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

        public PageServiceBuilder AddViewFinder<TViewModel>(Func<ViewFinderParameters, IView> viewFactory)
        {
            _viewFinder.Add(typeof(TViewModel), viewFactory);
            return this;
        }

        public PageServiceBuilder AddViewFinder<TView, TViewModel>(bool wrapInNavigationPage = false) where TView : Page
        {
            Func<ViewFinderParameters, Page> getter;

            if (wrapInNavigationPage)
                getter = (ViewFinderParameters p) =>
                {
                    var page = (Page)p.pageService.GetPage<TView>();
                    return new NavigationPage(page) { Title = page.Title };
                };
            else
                getter = (ViewFinderParameters p) => p.pageService.GetPage<TView>();

            _viewFinder.Add(typeof(TViewModel), getter);

            return this;
        }

        public IPageServiceZero Build()
        {
            _typeFactory = _typeFactory ?? _defaultTypeFactory;
            _navigationGetter = _navigationGetter ?? _defaultNavigationGetter;
            _pageService = new PageServiceZero(_navigationGetter, _typeFactory, ViewFinder);
            return _pageService;
        }

        private IView ViewFinder(Type vmType, object hint)
        {

            try
            {
                var parameters = new ViewFinderParameters(vmType, this._pageService, hint);
                return _viewFinder[vmType](parameters);
            }
            catch(KeyNotFoundException kex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. You must register them in UsePageServiceZero. TODO: More details.", kex);
            }
            catch(NullReferenceException nrex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. Don't know why, look at the innter exception? TODO: More details.", nrex);

            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot resolve the View for type {vmType}. The mapping has been registered in UsePageServiceZero but your container cannot provide a suitable view. TODO: More details.", ex);
            }
        }
    }
}

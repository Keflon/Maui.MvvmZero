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
        public PageServiceBuilder AddViewFinder<TView, TViewModel>()
        {
            _viewFinder.Add(typeof(TViewModel), (parameters) => (IView)_typeFactory(typeof(TView)));
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
            var parameters = new ViewFinderParameters(vmType, this._pageService, hint);
            return _viewFinder[vmType].Invoke(parameters);
        }
    }
}

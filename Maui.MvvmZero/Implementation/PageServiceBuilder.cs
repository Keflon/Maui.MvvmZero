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
        private Func<Type, object> _typeFactory;
        private Dictionary<Type, Func<IView>> _viewFinder;

        public PageServiceBuilder()
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

        public PageServiceBuilder AddViewFinder<TViewModel>(Func<IView> viewFactory)
        {
            _viewFinder.Add(typeof(TViewModel), viewFactory);
            return this;
        }
        public PageServiceBuilder AddViewFinder<TView, TViewModel>()
        {
            _viewFinder.Add(typeof(TViewModel), ()=>(IView)_typeFactory(typeof(TView)));
            return this;
        }

        public IPageServiceZero Build()
        {
            //if (_viewFinder == null && _typeFactory == null)
            //    throw new InvalidOperationException("Must call at least one of SetTypeFactory or AddViewFinder");

            return new PageServiceZero(_navigationGetter, _typeFactory, ViewFinder);
        }

        private IView ViewFinder(object arg)
        {
            return _viewFinder[arg.GetType()].Invoke();
        }
    }
}

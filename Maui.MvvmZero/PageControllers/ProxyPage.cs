using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    public class ProxyPage : MultiPage<ContentPage>, IContentView
    {
        public ProxyPage()
        {
            //this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", source: this));
            this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", BindingMode.OneWay, source: this));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // TODO: Use a binding?
            if (CurrentPage != null)
                CurrentPage.BindingContext = BindingContext;
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            //SelectedItem = CurrentPage;
            OnPropertyChanged(nameof(Content));
            if (CurrentPage != null)
                CurrentPage.BindingContext = BindingContext;
        }

        public object Content => ((IContentView)CurrentPage)?.Content;

        public IView PresentedContent => ((IContentView)CurrentPage)?.PresentedContent;

        public Size CrossPlatformArrange(Rect bounds)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformArrange(bounds) ?? new Size(30, 30);
        }

        public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformMeasure(widthConstraint, heightConstraint) ?? new Size(30, 30);

        }

        protected override ContentPage CreateDefault(object item)
        {
            return new ContentPage() { BackgroundColor = Colors.AliceBlue };
        }

        #region bindable properties

        #region LookupProperty

        public static readonly BindableProperty LookupProperty = BindableProperty.Create(nameof(Lookup), typeof(IDictionary<string, Func<Page>>), typeof(ProxyPage), null, BindingMode.OneWay, null, LookupPropertyChanged);

        public IDictionary<string, Func<Page>> Lookup
        {
            get { return (IDictionary<string, Func<Page>>)GetValue(LookupProperty); }
            set { SetValue(LookupProperty, value); }
        }

        private static void LookupPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (ProxyPage)bindable;

            self.Update();
        }

        private void Update()
        {
            if (Idiom != null)
            {
                if (Lookup != null)
                {
                    if (Lookup.TryGetValue(Idiom, out var pageGetter))
                    {
                        var page = pageGetter();
                        // TODO: Should be Page, e.g. NavigationPage only for one screen orientation. No, that can't work!
                        CurrentPage = pageGetter() as ContentPage;
                    }
                }
            }
        }

        #endregion

        #region IdiomProperty

        public static readonly BindableProperty IdiomProperty = BindableProperty.Create(nameof(Idiom), typeof(string), typeof(ProxyPage), null, BindingMode.OneWay, null, IdiomPropertyChanged);

        public string Idiom
        {
            get { return (string)GetValue(IdiomProperty); }
            set { SetValue(IdiomProperty, value); }
        }

        private static void IdiomPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (ProxyPage)bindable;

            self.Update();
        }

        #endregion
        #endregion
    }
}

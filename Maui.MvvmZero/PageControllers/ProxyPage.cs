using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    public class ProxyPage : Page, IContentView, IPageContainer<Page>
    {
        public ProxyPage()
        {
            this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", BindingMode.OneWay, source: this));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // TODO: Use a binding?
            if (CurrentPage != null)
                CurrentPage.BindingContext = BindingContext;
        }

        public object Content => ((IContentView)CurrentPage);

        public IView PresentedContent => ((IContentView)CurrentPage).PresentedContent;

        public Size CrossPlatformArrange(Rect bounds)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformArrange(bounds) ?? new Size(30, 30);
        }

        public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformMeasure(widthConstraint, heightConstraint) ?? new Size(30, 30);
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
                        CurrentPage = pageGetter() as Page;
                        //InvalidateMeasure();
                        //UpdateChildrenLayout();
                        //ForceLayout();
                        //CurrentPage.ForceLayout();
                        //ForceLayout();
                    }
                }
            }
        }

        #endregion

        #region IdiomProperty

        public static readonly BindableProperty IdiomProperty = BindableProperty.Create(nameof(Idiom), typeof(string), typeof(ProxyPage), null, BindingMode.OneWay, null, IdiomPropertyChanged);
        private Page currentPage;

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

        #region CurrentPageProperty

        public static readonly BindableProperty CurrentPageProperty = BindableProperty.Create(nameof(CurrentPage), typeof(Page), typeof(ProxyPage), null, BindingMode.OneWay, null, CurrentPagePropertyChanged);

        public Page CurrentPage
        {
            get { return (Page)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        private static void CurrentPagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (ProxyPage)bindable;
            self.CurrentPage.BindingContext = self.BindingContext;
            /*
            if(oldValue is Page oldPage)
                oldPage.BindingContext = null;

            if(newValue is Page newPage)
                newPage.BindingContext= self.BindingContext;



            */

            //self.OnPropertyChanged(nameof(CurrentPage));
            self.OnPropertyChanged(nameof(Content));

            self.Title = self.CurrentPage.Title;
        }

        #endregion
        #endregion
    }
}

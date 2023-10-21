using FunctionZero.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SampleTabbedApp.Mvvm.Pages
{
    internal class TestControlTemplatePage : TemplatedPage, IContentView, IPageContainer<Page>
    {
        public TestControlTemplatePage()
        {
            AdaptedTabbedPage r;

            this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", BindingMode.OneWay, source: this));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
            // TODO: Use a binding?
            if (CurrentPage != null)
                CurrentPage.BindingContext = BindingContext;
        }

        //public object Content => ((IContentView)CurrentPage);
        public object Content {
            get
            {
                if(CurrentPage is IPageContainer<Page> pc)
                {
                    if(pc.CurrentPage is NavigationPage navigationPage)
                    {
                        return navigationPage.CurrentPage;
                    }
                    return ((ContentPage)pc.CurrentPage);
                }
                else if(CurrentPage is Page)
                {
                    return CurrentPage;
                }
                return null;
            }
        }

        //public IView PresentedContent => ((IContentView)CurrentPage).PresentedContent;
        public IView PresentedContent => ((IContentView)Content).PresentedContent;

        public Size CrossPlatformArrange(Rect bounds)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformArrange(bounds) ?? new Size(30, 30);
        }

        public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return ((IContentView)CurrentPage)?.CrossPlatformMeasure(widthConstraint, heightConstraint) ?? new Size(30, 30);
        }


        #region CurrentPageProperty

        public static readonly BindableProperty CurrentPageProperty = BindableProperty.Create(nameof(CurrentPage), typeof(Page), typeof(TestControlTemplatePage), null, BindingMode.OneWay, null, CurrentPagePropertyChanged);

        public Page CurrentPage
        {
            get { return (Page)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        private static void CurrentPagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (TestControlTemplatePage)bindable;

            if (oldValue is Page oldPage)
                oldPage.BindingContext = null;

            if (newValue is Page newPage)
                newPage.BindingContext = self.BindingContext;

            //self.CurrentPage.BindingContext = self.BindingContext;
            
            //self.OnPropertyChanged(nameof(CurrentPage));
            self.OnPropertyChanged(nameof(Content));

            self.Title = self.CurrentPage.Title;
        }

        #endregion
    }
}
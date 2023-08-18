using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    public class ProxyPage : MultiPage<ContentPage>, IContentView
    {
        public ProxyPage()
        {
            this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", source: this));
            this.SetBinding(TitleProperty, new Binding("CurrentPage.Title", source: this));
        }
        
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFlyoutApp.Mvvm.PageViewModels
{
    public class ListPlayPageVm : BasePageVm
    {
        public override void OnOwnerPageDisappearing()
        {
            base.OnOwnerPageDisappearing();
        }

        public override void OnOwnerPagePopped(bool isModal)
        {
            base.OnOwnerPagePopped(isModal);
        }

    }

}

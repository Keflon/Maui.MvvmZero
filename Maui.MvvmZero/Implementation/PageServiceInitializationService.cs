using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    internal class PageServiceInitializationService : IMauiInitializeService
    {
        public void Initialize(IServiceProvider services)
        {
            // Application.Current isn't set up yet. :(
            //var pageService = services.GetService<IPageServiceZero>();
            //pageService.Init(Application.Current);
            // TODO: Consider using this to call an initialisation that is set in the 
            // TODO: UsePageServiceZero extension method to get the first page presented.
            // TODO: Or not, because I quite like we currently have.
        }
    }
}

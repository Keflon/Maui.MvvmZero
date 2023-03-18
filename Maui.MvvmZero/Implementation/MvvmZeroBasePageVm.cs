/*
MIT License

Copyright(c) 2016 - 2023 Function Zero Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero.Services;

namespace FunctionZero.Maui.MvvmZero
{
    /// <summary>
    /// If you get a WinUI xaml compiler error after deriving from this class,
    /// reference this library directly in your WinUI project to resolve it.
    /// </summary>
    public abstract class MvvmZeroBasePageVm : MvvmZeroBaseVm, IHasOwnerPage
    {
        private bool _isownerPageVisible;
        private bool _isOnNavigationStack;

        public event EventHandler OwnerPageAppearing;
        public event EventHandler OwnerPageDisappearing;

        private readonly List<AutoPageTimer> _pageTimers;

        public MvvmZeroBasePageVm()
        {
            _pageTimers = new List<AutoPageTimer>();
        }

        protected void AddPageTimer(int millisecondInterval, Action<object> callback, Action<Exception> exceptionHandler, object state)
        {
            _pageTimers.Add(new AutoPageTimer(this, millisecondInterval, callback, exceptionHandler, state));
        }

        #region IHasOwnerPage

        public bool IsOwnerPageVisible
        {
            get => _isownerPageVisible;
            private set => SetProperty(ref _isownerPageVisible, value);
        }
        public bool IsOnNavigationStack
        {
            get => _isOnNavigationStack;
            private set => SetProperty(ref _isOnNavigationStack, value);
        }

        public virtual void OnOwnerPageAppearing()
        {
            Debug.WriteLine($"OnOwnerPageAppearing {this}");
            IsOwnerPageVisible = true;
            OwnerPageAppearing?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnOwnerPageDisappearing()
        {
            Debug.WriteLine($"OnOwnerPageDisappearing {this}");
            IsOwnerPageVisible = false;
            OwnerPageDisappearing?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnOwnerPagePushed(bool isModal)
        {
            Debug.WriteLine($"OnOwnerPagePushed {this} Modal={isModal}");
            IsOnNavigationStack = true;
        }

        public virtual void OnOwnerPagePopped(bool isModal)
        {
            Debug.WriteLine($"OnOwnerPagePopped {this} Modal={isModal}");
            IsOnNavigationStack = false;
        }

        public virtual void OnOwnerPageAddedToVisualTree()
        {
            Debug.WriteLine($"OnOwnerPageAddedToVisualTree {this}");

        }

        public virtual void OnOwnerPageRemovedFromVisualTree()
        {
            Debug.WriteLine($"OnOwnerPageRemovedFromVisualTree {this}");

        }



        #endregion
    }
}

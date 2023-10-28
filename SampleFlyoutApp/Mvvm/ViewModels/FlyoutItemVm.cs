using FunctionZero.TreeZero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SampleFlyoutApp.Mvvm.ViewModels
{
    public class FlyoutItemVm : Node<FlyoutItemVm>
    {
        /// <summary>
        /// Represents the data for a data-driven FlyoutFlyout.
        /// Add any useful properties for databinding to this class, e.g. isEnabled, isVisible, iconName, subText, colorScheme etc.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="action"></param>
        public FlyoutItemVm(string title, Action<object> action)
        {
            Title = title;
            Action = action ?? ((arg) => { });
        }

        public string Title { get; }
        public Action<FlyoutItemVm> Action { get; }

        private bool _isExpanded;
        public bool IsExpanded { get => _isExpanded; set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsExpanded))
            {
                Action?.Invoke(this);
            }
        }
    }
}

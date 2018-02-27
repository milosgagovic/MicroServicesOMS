using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace DispatcherApp.View
{
    public class NetworkModelControlExtended : NetworkModelControl
    {
        public static readonly DependencyProperty itemsSourceForCanvasProperty =
            DependencyProperty.Register("ItemsSourceForCanvas", typeof(ObservableCollection<UIElement>),
            typeof(NetworkModelControlExtended));

        public ObservableCollection<UIElement> ItemsSourceForCanvas
        {
            get
            {
                return (ObservableCollection<UIElement>)GetValue(itemsSourceForCanvasProperty);
            }
            set
            {
                SetValue(itemsSourceForCanvasProperty, value);
            }
        }
    }
}

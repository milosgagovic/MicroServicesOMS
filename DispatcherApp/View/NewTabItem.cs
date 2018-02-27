using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DispatcherApp.View
{
    public class NetworkModelControlExtended : NetworkModelControl
    {
        public NetworkModelControlExtended()
        {
        }

        public ObservableCollection<UIElement> ItemsSourceForCanvas
        {
            get { return base.GetValue(ItemsSourceForCanvasProperty) as ObservableCollection<UIElement>; }
            set { base.SetValue(ItemsSourceForCanvasProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceForCanvasProperty =
          DependencyProperty.Register("ItemsSourceForCanvas", typeof(ObservableCollection<UIElement>), typeof(NetworkModelControlExtended), null);
    }
}

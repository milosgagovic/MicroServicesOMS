using DispatcherApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DispatcherApp.View.CustomControls
{
    public class BorderTabItem : TabItem
    {
        public TextBlock Title { get; set; }
        public ScrollViewer Scroll { get; set; }

        public BorderTabItem()
        {
            //this.DataContext = new BorderTabControlsViewModel();
            this.Header = "Not Set";
            FrameworkElement frameworkElement = new FrameworkElement();

            Grid mainGrid = new Grid();
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(20);
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(20);
            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);
            mainGrid.RowDefinitions.Add(row3);
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(20);
            mainGrid.ColumnDefinitions.Add(column1);
            mainGrid.ColumnDefinitions.Add(column2);

            Grid headerGrid = new Grid();
            headerGrid.Style = (Style)FindResource("BorderTabTopLineGrid");
            Grid.SetColumnSpan(headerGrid, 2);
            mainGrid.Children.Add(headerGrid);

            TextBlock title = new TextBlock();
            mainGrid.Children.Add(title);
            Grid.SetRow(title, 0);
            Grid.SetColumn(title, 0);
            title.Style = (Style)FindResource("BorderTabTopLineTextBlock");
            title.Text = "Not Set";

            this.Title = title;

            Button closeButton = new Button();
            mainGrid.Children.Add(closeButton);
            Grid.SetRow(closeButton, 0);
            Grid.SetColumn(closeButton, 1);
            closeButton.Style = (Style)FindResource("BorderTabTopLineCloseButton");
            closeButton.SetBinding(Button.CommandProperty, new Binding() { Path = new PropertyPath("CloseControlCommand") });
            closeButton.SetBinding(Button.CommandParameterProperty, new Binding() { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(BorderTabItem), 1), Path = new PropertyPath("Header") });

            ScrollViewer scroll = new ScrollViewer();
            scroll.Style = (Style)frameworkElement.FindResource("FavsScrollViewer");
            //Grid.SetRow(scroll, 1);
            //Grid.SetColumn(scroll, 0);
            //Grid.SetRowSpan(scroll, 2);
            //Grid.SetColumnSpan(scroll, 2);
            //scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            //scroll.Background = (Brush)frameworkElement.FindResource("DarkColor");
            //scroll.BorderBrush = (Brush)frameworkElement.FindResource("DarkColor");

            this.Scroll = scroll;
            mainGrid.Children.Add(this.Scroll);
            this.AddChild(mainGrid);

            Rectangle corner = new Rectangle() { Width = 20, Height = 20 };
            corner.Fill = (Brush)frameworkElement.FindResource("DarkColor");
            Grid.SetRow(corner, 1);
            Grid.SetColumn(corner, 1);
            Canvas.SetZIndex(corner, 100);

            this.Content = mainGrid;
        }
    }
}

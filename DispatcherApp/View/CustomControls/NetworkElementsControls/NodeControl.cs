using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DispatcherApp.View.CustomControls.NetworkElementsControls
{
    class NodeControl : Button
    {
        public NodeControl(double width, double height /*double left, double top, int z*/)
        {
            this.Width = width;
            this.Height = height;

            //Canvas.SetLeft(this, left);
            //Canvas.SetTop(this, top);
            //Canvas.SetZIndex(this, z);

            FrameworkElement frameworkElement = new FrameworkElement();

            Style style = new Style();
            style.TargetType = typeof(Button);

            Setter setter = new Setter();
            setter.Property = Button.TemplateProperty;

            ControlTemplate template = new ControlTemplate(typeof(Button));
            FrameworkElementFactory elemFactory = new FrameworkElementFactory(typeof(Border));
            elemFactory.Name = "Border";
            elemFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(width/2));
            elemFactory.SetValue(Border.BackgroundProperty, Brushes.Black);
            template.VisualTree = elemFactory;

            Trigger trigger1 = new Trigger();
            trigger1.Property = Button.IsMouseOverProperty;
            trigger1.Value = true;

            Setter setter1 = new Setter();
            setter1.Property = Border.BackgroundProperty;
            setter1.Value = Brushes.Gray;
            setter1.TargetName = "Border";
            trigger1.Setters.Add(setter1);

            template.Triggers.Add(trigger1);

            Trigger trigger2 = new Trigger();
            trigger2.Property = Button.IsPressedProperty;
            trigger2.Value = true;

            Setter setter2 = new Setter();
            setter2.Property = Border.BackgroundProperty;
            setter2.Value = Brushes.DarkGray;
            setter2.TargetName = "Border";
            trigger2.Setters.Add(setter2);

            template.Triggers.Add(trigger2);

            setter.Value = template;

            style.Triggers.Clear();
            style.Setters.Add(setter);
            this.Style = style;
        }
    }
}

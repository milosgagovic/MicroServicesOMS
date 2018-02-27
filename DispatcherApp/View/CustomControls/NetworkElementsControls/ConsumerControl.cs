using DispatcherApp.Model.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DispatcherApp.View.CustomControls.NetworkElementsControls
{
    public class ConsumerControl : Grid
    {
        public Button Button { get; set; }
        public Canvas ButtonCanvas { get; set; }

        public ConsumerControl(object dataContext, double buttonSize)
        {
            this.Button = new Button();
            this.ButtonCanvas = new Canvas();

            //Canvas.SetLeft(this, left);
            //Canvas.SetTop(this, top);
            //Canvas.SetZIndex(this, z);

            this.ButtonCanvas.Height = buttonSize;
            this.ButtonCanvas.Width = buttonSize;
            this.Button.Height = buttonSize;
            this.Button.Width = buttonSize;

            this.DataContext = dataContext;
            FrameworkElement frameworkElement = new FrameworkElement();

            Style style = new Style();
            style.TargetType = typeof(Button);

            Setter setter = new Setter();
            setter.Property = Button.TemplateProperty;

            ControlTemplate template = new ControlTemplate(typeof(Button));
            FrameworkElementFactory elemFactory = new FrameworkElementFactory(typeof(Border));
            elemFactory.Name = "Border";
            elemFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(buttonSize / 2));
            elemFactory.SetValue(Border.BackgroundProperty, (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed"));
            template.VisualTree = elemFactory;

            Trigger trigger1 = new Trigger();
            trigger1.Property = Button.IsMouseOverProperty;
            trigger1.Value = true;

            Setter setter1 = new Setter();
            setter1.Property = Border.BackgroundProperty;
            setter1.Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed");
            setter1.TargetName = "Border";
            trigger1.Setters.Add(setter1);

            template.Triggers.Add(trigger1);

            Trigger trigger2 = new Trigger();
            trigger2.Property = Button.IsPressedProperty;
            trigger2.Value = true;

            Setter setter2 = new Setter();
            setter2.Property = Border.BackgroundProperty;
            setter2.Value = (SolidColorBrush)frameworkElement.FindResource("SwitchColorClosed");
            setter2.TargetName = "Border";
            trigger2.Setters.Add(setter2);

            template.Triggers.Add(trigger2);

            DataTrigger dataTrigger1 = new DataTrigger();
            dataTrigger1.Binding = new Binding("IsEnergized");
            dataTrigger1.Value = false;

            Setter dataSetter1 = new Setter();
            dataSetter1.Property = Border.BackgroundProperty;
            dataSetter1.Value = Brushes.Blue;
            dataSetter1.TargetName = "Border";
            dataTrigger1.Setters.Add(dataSetter1);

            template.Triggers.Add(dataTrigger1);

            setter.Value = template;

            style.Triggers.Clear();
            style.Setters.Add(setter);
            this.Button.Style = style;
            this.ButtonCanvas.Children.Add(Button);

            this.Children.Add(this.ButtonCanvas);

            EnergyConsumerProperties consumerProperties = (EnergyConsumerProperties)dataContext;

            if (consumerProperties.IsUnderScada)
            {
                Canvas scadaCanvas = new Canvas();
                Border scadaBorder = new Border()
                {
                    Height = buttonSize / 2,
                    Width = buttonSize / 2,
                    Background = Brushes.DarkOrange,
                    BorderBrush = Brushes.White,
                    ToolTip = "Under SCADA"
                };
                scadaBorder.CornerRadius = new CornerRadius(scadaBorder.Height / 2);
                scadaBorder.BorderThickness = new Thickness(scadaBorder.Height / 10);
                Canvas.SetLeft(scadaBorder, -(scadaBorder.Width / 4));
                Canvas.SetTop(scadaBorder, -(scadaBorder.Height / 4));
                scadaCanvas.Children.Add(scadaBorder);

                TextBlock scadaTextblock = new TextBlock()
                {
                    Text = "S",
                    Foreground = Brushes.White,
                    ToolTip = "Under SCADA",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 4 * scadaBorder.Height / 5,
                    Margin = new Thickness(0, 0, 0, scadaBorder.Height / 6)
                };
                scadaBorder.Child = scadaTextblock;

                this.ButtonCanvas.Children.Add(scadaCanvas);
            }
        }
    }
}

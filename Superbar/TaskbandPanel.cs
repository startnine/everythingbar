using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Superbar
{
    public class TaskbandPanel : WrapPanel
    {
        Window _jumpListWindow = new Window();

        public Style JumpListStyle
        {
            get => (Style)GetValue(JumpListStyleProperty);
            set => SetValue(JumpListStyleProperty, value);
        }

        public static readonly DependencyProperty JumpListStyleProperty =
            DependencyProperty.Register("JumpListStyle", typeof(Style), typeof(TaskbandPanel), new PropertyMetadata());

        Window _thumbWindow = new Window();

        public Style ThumbWindowStyle
        {
            get => (Style)GetValue(ThumbWindowStyleProperty);
            set => SetValue(ThumbWindowStyleProperty, value);
        }

        public static readonly DependencyProperty ThumbWindowStyleProperty =
            DependencyProperty.Register("ThumbWindowStyle", typeof(Style), typeof(TaskbandPanel), new PropertyMetadata());

        public TaskbandPanel()
        {
            Binding jumpListStyleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("JumpListStyle"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_jumpListWindow, Window.StyleProperty, jumpListStyleBinding);

            Binding thumbStyleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("ThumbStyle"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_thumbWindow, Window.StyleProperty, thumbStyleBinding);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (visualAdded is UIElement)
            {
                var element = (visualAdded as UIElement);
                element.MouseEnter += Element_MouseEnter;
                element.MouseRightButtonUp += Element_MouseRightButtonUp;
            }
        }

        private void Element_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Element_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
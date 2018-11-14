using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Superbar
{
    class TaskItemMiddleClickBehavior : Behavior<FrameworkElement>
    {
        bool _pressed = false;
        ListViewItem _item;
        protected override void OnAttached()
        {
            base.OnAttached();

            _item = (AssociatedObject.TemplatedParent as FrameworkElement).TemplatedParent as ListViewItem;
            if (_item != null)
            {
                _item.PreviewMouseDown += _item_PreviewMouseDown;
                _item.PreviewMouseUp += _item_PreviewMouseUp;
            }
        }

        private void _item_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                _pressed = false;
        }

        private void _item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((!_pressed) && (e.ChangedButton == MouseButton.Middle) && (e.MiddleButton == MouseButtonState.Pressed) && (Mouse.MiddleButton == MouseButtonState.Pressed))
            {
                Debug.WriteLine("Opening app...");
                Panel visualOwner = VisualTreeHelper.GetParent(_item) as Panel;
                ((Window.GetWindow(sender as DependencyObject) as MainWindow).OpenApplications[visualOwner.Children.IndexOf(_item)]).DiskApplication.Open();
                _pressed = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using WindowsSharp.Processes;

namespace Everythingbar
{
    public class SysMenuBehavior : Behavior<FrameworkElement>
    {
        public ProcessWindow TargetWindow
        {
            get => (ProcessWindow)GetValue(TargetWindowProperty);
            set => SetValue(TargetWindowProperty, value);
        }

        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register("TargetWindow", typeof(ProcessWindow), typeof(SysMenuBehavior), new PropertyMetadata());

        ListViewItem _item;
        protected override void OnAttached()
        {

            base.OnAttached();

            _item = AssociatedObject.TemplatedParent as ListViewItem;
            //FrameworkElement parent = AssociatedObject.Parent as FrameworkElement;
            //Debug.WriteLine("PARENT TYPE: " + (AssociatedObject.TemplatedParent as FrameworkElement).TemplatedParent.GetType().FullName);

            /*while (!(parent is ListViewItem) && (parent != null))
            {
                parent = parent.Parent as FrameworkElement;
            }

            if (parent == null)
                Debug.WriteLine("PARENT IS NULL");
            else
                Debug.WriteLine("PARENT TYPE: " + AssociatedObject.Parent.GetType().FullName);*/
            if (_item != null)
            {
                _item.PreviewMouseRightButtonUp += ListViewItem_PreviewMouseRightButtonUp;
            }
        }

        private void ListViewItem_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TargetWindow.ShowSystemMenu(new WindowInteropHelper(Window.GetWindow(_item)).EnsureHandle());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using WindowsSharp.Processes;

namespace Superbar
{
    public class PeekBehavior : Behavior<FrameworkElement>
    {
        public ProcessWindow PeekWindow
        {
            get => (ProcessWindow)GetValue(PeekWindowProperty);
            set => SetValue(PeekWindowProperty, value);
        }

        public static readonly DependencyProperty PeekWindowProperty =
            DependencyProperty.Register("PeekWindow", typeof(ProcessWindow), typeof(PeekBehavior), new PropertyMetadata(null));

        public static bool IsPeeking = false;

        IntPtr _handle = IntPtr.Zero;
        ListViewItem _item = null;
        Window _window = null;
        /*Panel _panel = null;
        ProcessWindow _processWindow = null;*/

        protected override void OnAttached()
        {
            base.OnAttached();
            SetFields();
        }

        private void SetFields()
        {
            _item = AssociatedObject.TemplatedParent as ListViewItem;
            _handle = new WindowInteropHelper(Window.GetWindow(_item)).Handle;
            /*var win = Window.GetWindow(_item);
            _window = win as ThumbnailsWindow;
            _panel = VisualTreeHelper.GetParent(_item) as Panel;

            _window.IsVisibleChanged += ThumbnailsWindow_IsVisibleChanged;
            _processWindow = _window.OpenWindows[_panel.Children.IndexOf(_item)];*/

            /*if (_item.ToolTip != null)
                _item.ToolTip.Opened += ListViewItem_ToolTip_Opened;*/
            _item.MouseEnter += ListViewItem_MouseEnter;
            _item.MouseLeave += ListViewItem_MouseLeave;
        }

        /*private void ThumbnailsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)(e.NewValue))
                SetFields();
        }*/

        /*private void ListViewItem_ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            if (!IsPeeking)
                BeginPeek();
        }*/

        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //SetFields();

            if (IsPeeking)
                BeginPeek();
            else
            {
                int interval = 0;
                Timer timer = new Timer(250);
                timer.Elapsed += (sneder, args) =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        interval++;
                        if (interval > 1)
                        {
                            //Debug.WriteLine("timer");
                            if (_item.IsMouseOver)
                                BeginPeek();

                            timer.Stop();
                        }
                    }));
                };
                timer.Start();
            }
        }

        private void BeginPeek()
        {
            Window.GetWindow(_item).IsVisibleChanged += Window_IsVisibleChanged;
            //SetFields();

            /*if (_processWindow != null)
                _processWindow.Peek();

            Debug.WriteLine("IsPeeking: " + IsPeeking.ToString());
            IsPeeking = true;*/
            if (PeekWindow != null)
                PeekWindow.Peek(_handle);

            IsPeeking = true;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EndPeek();
        }

        private void ListViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EndPeek();
        }

        private void EndPeek()
        {
            if (PeekWindow != null)
                PeekWindow.Unpeek();

            IsPeeking = false;
        }
    }
}
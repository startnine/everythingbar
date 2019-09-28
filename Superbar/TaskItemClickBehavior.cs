using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using WindowsSharp.DiskItems;

namespace Everythingbar
{
    public class TaskItemClickBehavior : Behavior<FrameworkElement>
    {
        bool _pressed = false;
        ListViewItem _item;
        new public ToggleButton Button
        {
            get => (ToggleButton)GetValue(ButtonProperty);
            set => SetValue(ButtonProperty, value);
        }

        new public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.Register("Button", typeof(ToggleButton), typeof(TaskItemClickBehavior), new PropertyMetadata(OnButtonPropertyChanged));

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

        public static void OnButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as ToggleButton).Click += (d as TaskItemClickBehavior).Button_Click;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Panel visualOwner = VisualTreeHelper.GetParent(_item) as Panel;
            var app = ((Window.GetWindow(sender as DependencyObject) as MainWindow).OpenApplications[visualOwner.Children.IndexOf(_item)]);

            if (app.OpenWindows.Count == 0)
                app.DiskApplication.Open();
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

    class ToolBarClickBehavior : Behavior<FrameworkElement>
    {
        public Button TargetButton
        {
            get => (Button)GetValue(TargetButtonProperty);
            set => SetValue(TargetButtonProperty, value);
        }

        public static readonly DependencyProperty TargetButtonProperty =
            DependencyProperty.Register("TargetButton", typeof(Button), typeof(ToolBarClickBehavior), new PropertyMetadata(null, OnTargetButtonPropertyChangedCallback));

        internal static void OnTargetButtonPropertyChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var sned = sender as ToolBarClickBehavior;
            sned.GetButton();

            if (e.OldValue != null)
                (e.OldValue as Button).Click -= sned.Button_Click;
        }

        public DiskItem DiskFile
        {
            get => (DiskItem)GetValue(DiskFileProperty);
            set => SetValue(DiskFileProperty, value);
        }

        public static readonly DependencyProperty DiskFileProperty =
            DependencyProperty.Register("DiskFile", typeof(DiskItem), typeof(ToolBarClickBehavior), new PropertyMetadata(null));

        Button _button;

        protected override void OnAttached()
        {
            base.OnAttached();
            //_button = (AssociatedObject.TemplatedParent as FrameworkElement).TemplatedParent as Button;
            GetButton();
        }

        void GetButton()
        {
            if (TargetButton != null)
                _button = TargetButton;
            else if (AssociatedObject.TemplatedParent != null)
            {
                if (AssociatedObject.TemplatedParent is FrameworkElement parent)
                {
                    if (parent.TemplatedParent is Button button)
                        _button = button;
                    else
                        _button = null;
                }
                else
                    _button = null;

                if (_button != null)
                    _button.Click += Button_Click;
            }
            else
                _button = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Opening file...");
            /*Panel visualOwner = /*VisualTreeHelper.GetParent(_item)* / _button.TemplatedParent as Panel;
            var toolBar = visualOwner.TemplatedParent as SizableToolBar;
            (toolBar.ItemsSource as List<DiskItem>)[visualOwner.Children.IndexOf(_button)].Open();*/
            if (DiskFile != null)
                DiskFile.Open();
        }
    }
}

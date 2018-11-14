using Start9.UI.Wpf.Statics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Superbar
{
    public class TaskItemDragBehavior : Behavior<Panel>
    {
        //public PinnedApplication App { get; set; }
        new public PinnedApplication Application
        {
            get => (PinnedApplication)GetValue(ApplicationProperty);
            set => SetValue(ApplicationProperty, value);
        }

        new public static readonly DependencyProperty ApplicationProperty =
            DependencyProperty.Register("Application", typeof(PinnedApplication), typeof(TaskItemDragBehavior), new PropertyMetadata());

        ListViewItem _item;
        protected override void OnAttached()
        {

            base.OnAttached();

            _item = (AssociatedObject.TemplatedParent as FrameworkElement).TemplatedParent as ListViewItem;
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
                _item.PreviewMouseLeftButtonDown += ListViewItem_PreviewMouseLeftButtonDown;
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Transform originalTransform = _item.RenderTransform;
            ListView list = ItemsControl.ItemsControlFromItemContainer(_item) as ListView;

            //Debug.WriteLine("PARENT TYPE NAME: " + .GetType().FullName);
            Panel visualOwner = VisualTreeHelper.GetParent(_item) as Panel;
            //Debug.WriteLine("PARENT TYPE: " + AssociatedObject.Parent.GetType().FullName);
            Timer timer = new Timer(1);
            Point curInitial = SystemScaling.CursorPosition;
            Point curCurrent = SystemScaling.CursorPosition;
            MainWindow window = Window.GetWindow(list) as MainWindow;

            TranslateTransform newTransform = new TranslateTransform();

            bool isDragging = false;

            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        curCurrent = SystemScaling.CursorPosition;

                        if (window.Orientation == Orientation.Vertical)
                            newTransform.Y = curCurrent.Y - curInitial.Y;
                        else
                            newTransform.X = curCurrent.X - curInitial.X;

                        if (window.Orientation == Orientation.Vertical)
                        {
                            if ((curCurrent.Y > (curInitial.Y + 10))
                            || (curCurrent.Y < (curInitial.Y - 10)))
                            {
                                isDragging = true;
                            }
                        }
                        else
                        {
                            if ((curCurrent.X > (curInitial.X + 10))
                            || (curCurrent.X < (curInitial.X - 10)))
                            {
                                isDragging = true;
                            }
                        }

                        if (isDragging)
                        {
                            Config.IsDraggingPinnedApplication = true;

                            foreach (PinnedApplication p in list.Items)
                            {
                                ListViewItem hoverItem = visualOwner.Children[list.Items.IndexOf(p)] as ListViewItem;
                                if (IsMouseWithin(hoverItem) && (Application != p))
                                {
                                    //int index = visualOwner.Children.IndexOf(_item);
                                    int hoverIndex = list.Items.IndexOf(p);
                                    (list.ItemsSource as ObservableCollection<PinnedApplication>).Remove(Application);
                                    (list.ItemsSource as ObservableCollection<PinnedApplication>).Insert(hoverIndex, Application);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Config.IsDraggingPinnedApplication = false;
                        timer.Stop();
                    }
                }));
            };
            timer.Start();
        }

        public static bool IsMouseWithin(FrameworkElement target)
        {
            var targetPoint = target.PointToScreen(new Point(0, 0));
            var cur = SystemScaling.CursorPosition;

            double width = target.ActualWidth;
            if (width == 0)
                try
                {
                    width = target.Width;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            double height = target.ActualHeight;
            if (height == 0)
                try
                {
                    height = target.Height;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            /*Debug.WriteLine(target.GetType().FullName + "\n"
                + cur.X + ", " + cur.Y + ", " + targetPoint.X + ", " + targetPoint.Y + ", " + width + ", " + height + "\n"
                + (cur.X >= targetPoint.X).ToString() + ", " + (cur.Y >= targetPoint.Y).ToString() + ", " + (cur.X < targetPoint.X + width).ToString() + ", " + (cur.Y < targetPoint.Y + height).ToString());*/
            return (cur.X >= targetPoint.X)
                && (cur.Y >= targetPoint.Y)
                && (cur.X < targetPoint.X + width)
                && (cur.Y < targetPoint.Y + height);
        }
    }
}

using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsSharp.Processes;

namespace Everythingbar
{
    /// <summary>
    /// Interaction logic for ThumbnailsWindow.xaml
    /// </summary>
    public partial class ThumbnailsWindow : ShadowedWindow, INotifyPropertyChanged
    {
        ObservableCollection<ProcessWindow> _openWindows = new ObservableCollection<ProcessWindow>();
        public ObservableCollection<ProcessWindow> OpenWindows
        {
            get => _openWindows;
            set
            {
                _openWindows = value;
                NotifyPropertyChanged("OpenWindows");
            }
        }

        private ProcessWindow _selectedWindow = null;
        public ProcessWindow SelectedWindow
        {
            get
            {
                return _selectedWindow;
            }
            set
            {
                _selectedWindow = value;
                NotifyPropertyChanged("SelectedWindow");
                if (IsWindowVisible && (_selectedWindow != null))
                {
                    Debug.WriteLine("ThumbnailsWindow.SelectedWindow " + _selectedWindow.Title);
                    _selectedWindow.Show();
                }
            }
        }

        public IEasingFunction RepositionEase
        {
            get => (IEasingFunction)GetValue(RepositionEaseProperty);
            set => SetValue(RepositionEaseProperty, value);
        }

        public static readonly DependencyProperty RepositionEaseProperty =
            DependencyProperty.Register("RepositionEase", typeof(IEasingFunction), typeof(ThumbnailsWindow), new PropertyMetadata(null));

        int _keepOpenInterval = 0;

        Timer _keepOpenTimer = new Timer
        {
            Interval = 100
        };

        public ThumbnailsWindow()
        {
            InitializeComponent();

            /*IntPtr handle = new WindowInteropHelper(this).EnsureHandle();
            int exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GwlExstyle).ToInt32();

            exStyle = NativeMethods.WsExToolwindow;

            NativeMethods.SetWindowLong(handle, NativeMethods.GwlExstyle, exStyle);*/

            ProcessWindow.ActiveWindowChanged += ProcessWindow_ActiveWindowChanged;


            _keepOpenTimer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ///*Application.Current.MainWindow.IsMouseOver*/
                    if (IsMouseWithin(App.Current.MainWindow as TaskbarWindow) | IsMouseWithin(this))
                    {
                        _keepOpenInterval = 0;
                    }
                    else
                    {
                        _keepOpenInterval++;
                        if (_keepOpenInterval > 3)
                        {
                            Hide();
                            _keepOpenInterval = 0;
                            _keepOpenTimer.Stop();
                        }
                    }
                }));
            };

            /*_openWindows.CollectionChanged += (sneder, args) =>
            {
                double oldWidth = ActualWidth;
                double newWidth = WindowsListView.ActualWidth;
                Debug.WriteLine("newWidth: " + newWidth);

                DoubleAnimation widthAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    From = oldWidth,
                    To = newWidth
                };

                if (RepositionEase != null)
                    widthAnimation.EasingFunction = RepositionEase;

                widthAnimation.Completed += (snader, ergs) =>
                {
                    //_animatingSize = false;
                    BeginAnimation(WidthProperty, null);
                    Width = newWidth;
                };

                BeginAnimation(WidthProperty, widthAnimation);
            };*/
        }

        new public void Show()
        {
            if (!IsVisible)
            {
                base.Show();
                UpdatePosition();
            }
            /*else
            {
                UpdatePosition();
                base.Show();
            }*/
        }

        //bool _animatingSize = false;

        private void WindowsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double difference = ActualWidth - e.PreviousSize.Width;
            double oldWidth = e.PreviousSize.Width;
            double newWidth = e.NewSize.Width;
            Debug.WriteLine("newWidth: " + newWidth);

            DoubleAnimation widthAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(500),
                From = oldWidth,
                To = newWidth
            };

            if (RepositionEase != null)
                widthAnimation.EasingFunction = RepositionEase;

            widthAnimation.Completed += (sneder, args) =>
            {
                //_animatingSize = false;
                BeginAnimation(WidthProperty, null);
                Width = newWidth;
            };

            BeginAnimation(WidthProperty, widthAnimation);
        }

        /*protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            UpdatePosition();
        }*/
        /*protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            double oldWidth = ActualWidth;
            double newWidth = (Content as UIElement).DesiredSize.Width;

            DoubleAnimation widthAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(500),
                From = oldWidth,
                To = newWidth
            };

            if (RepositionEase != null)
                widthAnimation.EasingFunction = RepositionEase;

            widthAnimation.Completed += (sneder, args) =>
            {
                //_animatingSize = false;
                BeginAnimation(WidthProperty, null);
                Width = newWidth;
            };

            BeginAnimation(WidthProperty, widthAnimation);
        }*/

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!double.IsNaN(Left))
                UpdatePosition();
        }
        /*protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _animatingSize = true;
            UpdatePosition();
            if (_animatingSize)
            {
                double oldWidth = sizeInfo.PreviousSize.Width;
                double newWidth = sizeInfo.NewSize.Width;

                DoubleAnimation widthAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    From = oldWidth,
                    To = newWidth
                };

                if (RepositionEase != null)
                    widthAnimation.EasingFunction = RepositionEase;

                widthAnimation.Completed += (sneder, args) =>
                {
                    base.OnRenderSizeChanged(sizeInfo);
                    _animatingSize = false;
                    BeginAnimation(WidthProperty, null);
                    Left = newWidth;
                };

                BeginAnimation(WidthProperty, widthAnimation);
            }
        }*/

        public void UpdatePosition()
        {
            //double oldWidth = ActualWidth;
            /*double newWidth = WindowsListView.ActualWidth;
            Debug.WriteLine("newWidth: " + newWidth);

            DoubleAnimation widthAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(500),
                //From = oldWidth,
                To = newWidth
            };

            if (RepositionEase != null)
                widthAnimation.EasingFunction = RepositionEase;

            widthAnimation.Completed += (snader, ergs) =>
            {
                BeginAnimation(WidthProperty, null);
                Width = newWidth;
            };

            BeginAnimation(WidthProperty, widthAnimation);*/

            double newLeft = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition.X - (ActualWidth / 2);
            if (!double.IsNaN(Width))
                newLeft = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition.X - (Width / 2);

            if (newLeft < 0)
                newLeft = 0;
            else if ((newLeft + ActualWidth) > SystemParameters.WorkArea.Width)
                newLeft = SystemParameters.WorkArea.Width - ActualWidth;

            if (Visibility == Visibility.Visible)
            {
                /*IEasingFunction ease = null;
                try
                {
                    ease = (IEasingFunction)App.Current.Resources["ThumbnailsWindowMovementEase"];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }*/

                DoubleAnimation leftAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(500),
                    To = newLeft
                };

                if (RepositionEase != null)
                    leftAnimation.EasingFunction = RepositionEase;

                leftAnimation.Completed += (sneder, args) =>
                {
                    BeginAnimation(LeftProperty, null);
                    Left = newLeft;
                };

                //BeginAnimation(LeftProperty, null);
                BeginAnimation(LeftProperty, leftAnimation);
                //}
            }
            else
                Left = newLeft;
        }

        /*protected override void OnSourceInitialized(EventArgs e)
        {
            //var hwnd = new WindowInteropHelper(this).EnsureHandle();
            //NativeMethods.SetWindowLong(hwnd, NativeMethods.GwlExstyle, NativeMethods.GetWindowLong(hwnd, NativeMethods.GwlExstyle).ToInt32() | NativeMethods.WsExToolwindow);

            IntPtr handle = new WindowInteropHelper(this).EnsureHandle();
            int exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GwlExstyle).ToInt32();

            exStyle |= NativeMethods.WsExToolwindow;

            NativeMethods.SetWindowLong(handle, NativeMethods.GwlExstyle, exStyle);

            base.OnSourceInitialized(e);
        }*/

        public static bool IsMouseWithin(FrameworkElement target)
        {
            var targetPoint = target.PointToScreen(new Point(0, 0));
            var cur = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition;

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

        private void ProcessWindow_ActiveWindowChanged(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProcessWindow window = null;
                foreach (ProcessWindow w in OpenWindows)
                    if (w.Handle == e.Window.Handle)
                    {
                        window = w;
                        break;
                    }

                SelectedWindow = window;

                /*if (Mouse.LeftButton == MouseButtonState.Pressed)
                {*/
                    _keepOpenTimer.Stop();
                    _keepOpenInterval = 0;
                    Hide();
                //}
            }));
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private bool _hideOnSelect = true;

        private void WindowsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //bool addedContainsActive = false;
            bool containsActive = false;
            //bool removedContainsActive = false;

            foreach (ProcessWindow w in OpenWindows)
                if (w.Handle == ProcessWindow.ActiveWindow.Handle)
                {
                    containsActive = true;
                    break;
                }

            /*foreach (ProcessWindow w in e.RemovedItems)
                if (w.Handle == ProcessWindow.ActiveWindow.Handle)
                {
                    removedContainsActive = true;
                    break;
                }

            foreach (ProcessWindow w in e.AddedItems)
                if (w.Handle == ProcessWindow.ActiveWindow.Handle)
                {
                    addedContainsActive = true;
                    break;
                }*/

            if (false) //containsActive && Mouse.LeftButton == MouseButtonState.Pressed)// (!containsActive) && removedContainsActive)
            {
                _keepOpenTimer.Stop();
                _keepOpenInterval = 0;
                Hide();
            }
        }

        /*internal void SetSelection(ProcessWindow window)
        {
            _hideOnSelect = false;
            SelectedWindow = window;
            _hideOnSelect = true;
        }*/

        private void ThumbnailsWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            //_keepOpenTimer.Start();
        }

        private void ThumbnailsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                _keepOpenTimer.Start();
            else
                _keepOpenTimer.Stop();
        }

        /*private void ThumbnailsWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!double.IsNaN(Left))
                UpdatePosition();
        }*/
    }
}

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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsSharp.Processes;

namespace Superbar
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
                if (_selectedWindow != null)
                    _selectedWindow.Show();
            }
        }

        int _keepOpenInterval = 0;

        Timer _keepOpenTimer = new Timer
        {
            Interval = 100
        };

        public ThumbnailsWindow()
        {
            InitializeComponent();

            IntPtr handle = new WindowInteropHelper(this).EnsureHandle();
            int exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GwlExstyle).ToInt32();

            exStyle = NativeMethods.WsExToolwindow;

            NativeMethods.SetWindowLong(handle, NativeMethods.GwlExstyle, exStyle);

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
    }
}

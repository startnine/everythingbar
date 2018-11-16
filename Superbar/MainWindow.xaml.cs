using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Media.Animation;
using Start9.UI.Wpf;
using Start9.UI.Wpf.Windows;
using WindowsSharp.Processes;
using System.Collections.ObjectModel;
using Start9.WCF;
using System.ServiceModel;
using WindowsSharp.DiskItems;
using System.ComponentModel;
using System.IO;

namespace Superbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : TaskbarWindow, INotifyPropertyChanged
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("dwmapi.dll")]
        static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        //public ObservableCollection<ProcessWindowSet> OpenWindows { get; set; } = new ObservableCollection<ProcessWindowSet>();
        public ObservableCollection<PinnedApplication> OpenApplications { get; set; } = new ObservableCollection<PinnedApplication>();
        //public List<int> AddedProcesses = new List<int>();
        //public Dictionary<Process, int> OpenProcesses { get; set; } = new Dictionary<Process, int>();

        /*private bool _useSmallIcons = false;
        public bool UseSmallIcons
        {
            get => _useSmallIcons;
            set
            {
                _useSmallIcons = value;
                NotifyPropertyChanged("UseSmallIcons");
            }
        }*/

        public bool UseSmallIcons
        {
            get => (bool)GetValue(UseSmallIconsProperty);
            set => SetValue(UseSmallIconsProperty, value);
        }

        public static readonly DependencyProperty UseSmallIconsProperty =
            DependencyProperty.Register("UseSmallIcons", typeof(bool), typeof(MainWindow), new PropertyMetadata(Config.UseSmallIcons));

        private Config.CombineMode _taskbarCombineMode = Config.TaskbarCombineMode;
        public Config.CombineMode TaskbarCombineMode
        {
            get => _taskbarCombineMode;
            set
            {
                _taskbarCombineMode = value;
                NotifyPropertyChanged("TaskbarCombineMode");
            }
        }

        //public Config.ClockDateMode TrayClockDateMode { get; set; } = Config.TrayClockDateMode;
        /*private Config.ClockDateMode _trayClockDateMode = Config.TrayClockDateMode;
        public Config.ClockDateMode TrayClockDateMode
        {
            get => _trayClockDateMode;
            set
            {
                _trayClockDateMode = value;
                NotifyPropertyChanged("TrayClockDateMode");
            }
        }*/

        /*public ProcessWindow SetActiveWindow
        {
            get => ProcessWindow.ActiveWindow;
            set => SelectWindow(value);
        }*/

        /*public double ResizeDistance
        {
            get => (double)GetValue(ResizeDistanceProperty);
            set => SetValue(ResizeDistanceProperty, value);
        }

        public static readonly DependencyProperty ResizeDistanceProperty =
            DependencyProperty.Register("ResizeDistance", typeof(double), typeof(MainWindow), new PropertyMetadata(40.0));*/

        [StructLayout(LayoutKind.Sequential)]
        struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;

            public DWM_BLURBEHIND(bool enabled)
            {
                fEnable = enabled ? true : false;
                hRgnBlur = IntPtr.Zero;
                fTransitionOnMaximized = false;
                dwFlags = DWM_BB.Enable;
            }

            public System.Drawing.Region Region
            {
                get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
            }

            public bool TransitionOnMaximized
            {
                get { return fTransitionOnMaximized != false; }
                set
                {
                    fTransitionOnMaximized = value ? true : false;
                    dwFlags |= DWM_BB.TransitionMaximized;
                }
            }

            public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
            {
                hRgnBlur = region.GetHrgn(graphics);
                dwFlags |= DWM_BB.BlurRegion;
            }
        }

        [Flags]
        enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionMaximized = 4
        }

        [Flags]
        public enum ThumbnailFlags : int
        {
            RectDetination = 1,
            RectSource = 2,
            Opacity = 4,
            Visible = 8,
            SourceClientAreaOnly = 16
        }

        public enum GetWindowCmd : uint
        {
            First = 0,
            Last = 1,
            Next = 2,
            Prev = 3,
            Owner = 4,
            Child = 5,
            EnabledPopup = 6
        }

        const int GclHiconsm = -34;
        const int GclHicon = -14;
        const int IconSmall = 0;
        const int IconBig = 1;
        const int IconSmall2 = 2;
        const int WmGeticon = 0x7F;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int TASKSTYLE = 0x10000000 | 0x00800000;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        public IntPtr current = IntPtr.Zero;
        public IntPtr handle = IntPtr.Zero;

        public List<IntPtr> thumbs = new List<IntPtr>();


        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }


        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);

        // 1. Change the function to call the Unicode variant, where applicable.
        // 2. Ask the marshaller to alert you to any errors that occur.
        // 3. Change the parameter types to make marshaling easier. 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(
                                                        int uiAction,
                                                        int uiParam,
                                                        ref RECT pvParam,
                                                        int fWinIni);

        private const Int32 SPIF_SENDWININICHANGE = 2;
        private const Int32 SPIF_UPDATEINIFILE = 1;
        private const Int32 SPIF_change = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;
        private const Int32 SPI_SETWORKAREA = 47;
        private const Int32 SPI_GETWORKAREA = 48;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 Left;
            public Int32 Top;   // top is before right in the native struct
            public Int32 Right;
            public Int32 Bottom;
        }

        //https://stackoverflow.com/questions/6267206/how-can-i-resize-the-desktop-work-area-using-the-spi-setworkarea-flag
        private static bool SetWorkspace(RECT rect)
        {
            // Since you've declared the P/Invoke function correctly, you don't need to
            // do the marshaling yourself manually. The .NET FW will take care of it.

            bool result = SystemParametersInfo(SPI_SETWORKAREA,
                                               0,
                                               ref rect,
                                               SPIF_change);
            if (!result)
            {
                // Find out the error code
                MessageBox.Show("The last error was: " +
                                Marshal.GetLastWin32Error().ToString());
            }

            return result;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        readonly Timer _activeWindowTimer = new Timer
        {
            Interval = 1
        };

        readonly Timer _clockTimer = new Timer
        {
            Interval = 1
        };

        public double ScrollAnimator
        {
            get => (double)GetValue(ScrollAnimatorProperty);
            set => SetValue(ScrollAnimatorProperty, value);
        }

        ThumbnailsWindow _thumbnailsWindow = new ThumbnailsWindow();
        JumpListWindow _jumpListWindow = new JumpListWindow();

        public static readonly DependencyProperty ScrollAnimatorProperty = DependencyProperty.Register("ScrollAnimator",
            typeof(double), typeof(MainWindow),
            new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        Shell32.Shell _shell = new Shell32.Shell();

        public List<ProcessWindow> Windows(Process process)
        {
            var list = new List<ProcessWindow>();
            var windows = ProcessWindow.ProcessWindows;

            foreach (ProcessWindow w in windows)
            {
                if (w.Process.Id == process.Id)
                    list.Add(w);
            }

            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*OpenApplications.CollectionChanged += (sneder, args) =>
            {
                TaskBandScrollBar.Visibility = TaskBandScrollViewer.ComputedVerticalScrollBarVisibility;
            };*/
            Populate();
            //int windowCounter = 0;

            /*foreach (KeyValuePair<Process, int> p in OpenProcesses)
            {
                foreach (ProcessWindow w in p.Key.Windows)
                {
                    OpenWindows.Add()
                }
            }*/

            /*ProcessWindow.WindowOpened += (sneder, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var win = (args as WindowEventArgs).Window;
                    if (!OpenProcesses.Keys.Contains(win.Process))
                    {

                        OpenWindows.Add((args as WindowEventArgs).Window);
                    }
                }));
            };*/

            ProcessWindow.WindowOpened += (sneder, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddWindow(args.Window);
                }));
            };

            ProcessWindow.ActiveWindowChanged += (sneder, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (PinnedApplication a in OpenApplications)
                    {
                        bool windowFound = false;

                        foreach (ProcessWindow w in a.OpenWindows)
                        {
                            if (w.Handle == args.Window.Handle)
                            {
                                TaskBandListView.SelectedItem = a;
                                windowFound = true;
                                break;
                            }
                        }

                        if (windowFound)
                            break;
                    }
                }));
            };

            ProcessWindow.WindowClosed += (sneder, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (PinnedApplication a in OpenApplications)
                    {
                        bool windowFound = false;

                        foreach (ProcessWindow w in a.OpenWindows)
                        {
                            if (w.Handle == args.Window.Handle)
                            {
                                //TaskBandListView.SelectedItem = a;
                                windowFound = true;
                                break;
                            }
                        }

                        /*if (windowFound)
                        {
                            if ((a.OpenWindows.Count == 0) && (!a.IsPinned))
                                OpenApplications.Remove(a);

                            break;
                        }*/
                    }
                }));
            };

            /*ProcessWindow.ActiveWindowChanged += (sneder, args) =>
                {
                    //Debug.WriteLine("ActiveWindowChanged");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (ProcessWindowSet l in OpenWindows)
                        {
                            foreach (ProcessWindow p in l.Windows)
                            {
                                if (p.Handle == (args as WindowEventArgs).Window.Handle)
                                {
                                    TaskBandListView.SelectedItem = p;
                                    //((TreeViewItem)TaskBandListView.ItemContainerGenerator.ContainerFromIndex(l.IndexOf(p))).IsSelected = true;
                                    break;
                                }
                            }
                        }
                    }));
                };*/

            /*ProcessWindow.WindowClosed += (sneder, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (ProcessWindowSet l in OpenWindows)
                    {
                        foreach (ProcessWindow p in l.Windows)
                        {
                            if (p.Handle == (args as WindowEventArgs).Window.Handle)
                            {
                                l.Windows.Remove(p);
                                break;
                            }
                        }
                    }
                }));
            };*/

            Config.ConfigUpdated += Config_ConfigUpdated;
            Config_ConfigUpdated(null, null);

            UpdateClockDateVisibility();

            _clockTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ClockTime.Text = DateTime.Now.ToShortTimeString();
                    if (ClockDate.IsVisible)
                        ClockDate.Text = DateTime.Now.ToShortDateString();
                }));
            };
            _clockTimer.Start();

            SizeChanged += (sneder, args) =>
            {
                UpdateClockDateVisibility();
            };
            //TrayClockDateMode
        }

        private void Config_ConfigUpdated(object sender, EventArgs e)
        {
            IsLocked = Config.IsLocked;
            DockMode = Config.DockMode;
            UseSmallIcons = Config.UseSmallIcons;
            TaskbarCombineMode = Config.TaskbarCombineMode;
            //TrayClockDateMode = Config.TrayClockDateMode;
            UpdateClockDateVisibility();
        }

        public void Populate()
        {
            OpenApplications.Clear();

            foreach (ProcessWindow w in ProcessWindow.ProcessWindows)// Process p in Process.GetProcesses())
            {
                AddWindow(w);
            }

            List<string> pinnedApps = File.ReadAllLines(Config.PinnedAppsPath).ToList();
            List<bool> areAppsAlreadyPresent = new List<bool>();
            int counter = 0;
            int insertCounter = 0;

            foreach (string s in pinnedApps)
            {
                bool isAppAlreadyPresent = false;
                foreach (PinnedApplication a in OpenApplications)
                {
                    if (a.DiskApplication.ItemPath.ToLowerInvariant() == s.ToLowerInvariant())
                    {
                        a.IsPinned = true;
                        //Debug.WriteLine("IsAlreadyPresent, IsPinned: " + a.IsPinned.ToString() + ", " + a.DiskApplication.ItemRealName);
                        isAppAlreadyPresent = true;

                        OpenApplications.Move(OpenApplications.IndexOf(a), counter);
                        //OpenApplications.Insert(counter, a);
                        counter++;

                        break;
                    }
                    /*OpenApplications.Add(new PinnedApplication(new DiskItem(Environment.ExpandEnvironmentVariables(s)))
                    {
                        IsPinned = true
                    });*/
                }

                if (!isAppAlreadyPresent)
                {
                    PinnedApplication app = new PinnedApplication(new DiskItem(s))
                    {
                        IsPinned = true
                    };
                    SetAppEventHandlers(app);
                    OpenApplications.Insert(insertCounter, app);
                    //Debug.WriteLine("3IsPinned: " + app.IsPinned.ToString() + ", " + app.DiskApplication.ItemRealName);
                    insertCounter++;
                }
                //areAppsAlreadyPresent.Add(isAppAlreadyPresent);
            }

            /*pinnedApps.Reverse();
                areAppsAlreadyPresent.Reverse();

                for (int i = 0; i < areAppsAlreadyPresent.Count; i++)
                {
                    if (!areAppsAlreadyPresent[i])
                    {
                        PinnedApplication app = new PinnedApplication(new DiskItem(Environment.ExpandEnvironmentVariables(pinnedApps[i])))
                        {
                            IsPinned = true
                        };
                        OpenApplications.Insert(insertCounter, app);
                        Debug.WriteLine("3IsPinned: " + app.IsPinned.ToString() + ", " + app.DiskApplication.ItemRealName);
                        insertCounter++;
                    }
                }

                //OpenApplications.Insert(0, new PinnedApplication(new DiskItem(Environment.ExpandEnvironmentVariables(@"%windir%\Explorer.exe"))));
                //OpenApplications.RemoveAt(0);*/
        }

        void UpdateClockDateVisibility()
        {
            if (Config.TrayClockDateMode == Config.ClockDateMode.AlwaysShow)
                ClockDate.Visibility = Visibility.Visible;
            else if (Config.TrayClockDateMode == Config.ClockDateMode.NeverShow)
                ClockDate.Visibility = Visibility.Collapsed;
            else
            {
                if (UseSmallIcons && (DockedWidthOrHeight <= ResizeIntervalDistance))
                    ClockDate.Visibility = Visibility.Collapsed;
                else
                    ClockDate.Visibility = Visibility.Visible;
            }
        }

        public void SetAppEventHandlers(PinnedApplication pinnedApp)
        {
            pinnedApp.LastWindowClosed += PinnedApp_LastWindowClosed;
            pinnedApp.ThumbnailsRequested += PinnedApp_ThumbnailsRequested;
            pinnedApp.JumpListRequested += PinnedApp_JumpListRequested;
            pinnedApp.IsPinnedChanged += PinnedApp_LastWindowClosed;
        }

        public void AddWindow(ProcessWindow window)
        {
            bool processAlreadyAdded = false;
            foreach (PinnedApplication a in OpenApplications)
            {
                var item = GetApplicationByWindow(window);
                if ((item != null) && (a.DiskApplication.ItemPath.ToLowerInvariant() == item.DiskApplication.ItemPath.ToLowerInvariant()))
                {
                    processAlreadyAdded = true;
                    break;
                }
            }

            if (!processAlreadyAdded)
            {
                try
                {
                    var pinnedApp = new PinnedApplication(new DiskItem(Config.GetExecutablePath(window.Process)));
                    SetAppEventHandlers(pinnedApp);
                    //Debug.WriteLine("2IsPinned: " + pinnedApp.IsPinned.ToString() + ", " + pinnedApp.DiskApplication.ItemRealName);
                    OpenApplications.Add(pinnedApp);
                    //Debug.WriteLine("PROCESS: " + w.Process.MainModule.FileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            /*try
            {
                if (!AddedProcesses.Contains(p.Id))
                {
                    if (Windows(p).Count > 0)
                    {
                        Debug.WriteLine(p.ToString());
                        ProcessWindowSet windows = new ProcessWindowSet(Windows(p), p);
                        OpenWindows.Add(windows);
                        Debug.WriteLine(windows.Windows.Count);
                        AddedProcesses.Add(p.Id);
                    }
                }
            }
            catch (Exception ex)
            {

            }*/
            _thumbnailsWindow.SizeChanged += FlyoutWindow_SizeChanged;
            _thumbnailsWindow.LocationChanged += FlyoutWindow_LocationChanged;

            _jumpListWindow.SizeChanged += FlyoutWindow_SizeChanged;
            _jumpListWindow.LocationChanged += FlyoutWindow_LocationChanged;
        }

        private void PinnedApp_LastWindowClosed(object sender, EventArgs e)
        {
            var sned = (sender as PinnedApplication);
            if ((!sned.IsPinned) && (sned.OpenWindows.Count == 0))
            {
                //Debug.WriteLine("IsPinned: " + sned.IsPinned.ToString() + "\nName: " + sned.DiskApplication.ItemRealName + "\nOpenWindows.Count: " + sned.OpenWindows.Count.ToString() + "\nRemoving app");
                OpenApplications.Remove(sned);
            }
        }

        private void FlyoutWindow_LocationChanged(object sender, EventArgs e)
        {
            FlyoutWindow_BoundsChanged(sender as Window);
        }

        private void FlyoutWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FlyoutWindow_BoundsChanged(sender as Window);
        }

        private void FlyoutWindow_BoundsChanged(Window win)
        {
            if (DockMode == AppBarDockMode.Left)
                win.Left = Left + ActualWidth;
            else if (DockMode == AppBarDockMode.Top)
                win.Top = Top + ActualHeight;
            else if (DockMode == AppBarDockMode.Right)
                win.Left = Left - win.ActualWidth;
            else
                win.Top = Top - win.ActualHeight;
        }

        private void PinnedApp_ThumbnailsRequested(object sender, WindowEventArgs e)
        {
            if (!_jumpListWindow.IsVisible)
            {
                var app = sender as PinnedApplication;

                if (app.OpenWindows.Count > 0)
                {

                    /*if (app.AreThumbnailsShown)
                    {*/
                    //_thumbnailsWindow.SetSelection(e.Window);
                    _thumbnailsWindow.SelectedWindow = e.Window;
                    _thumbnailsWindow.OpenWindows = app.OpenWindows;
                    _thumbnailsWindow.Show();

                    //var item = TaskBandListView.ItemContainerGenerator.ContainerFromItem(app);
                    /*var hitResult = System.Windows.Media.VisualTreeHelper.HitTest(TaskBandListView, Start9.UI.Wpf.Statics.SystemScaling.CursorPosition);

                    if (hitResult != null)
                    {
                        var visualHit = hitResult.VisualHit;
                        while ((visualHit != null) && (VisualTreeHelper.GetParent(visualHit) != null) && (!(visualHit is ListViewItem)))
                        {
                            visualHit = VisualTreeHelper.GetParent(visualHit);
                        }

                        /*var item = TaskBandListView.ItemContainerGenerator.ContainerFromItem(app);
                        var hitResult = (System.Windows.Media.VisualTreeHelper.HitTest(item, Start9.UI.Wpf.Statics.SystemScaling.CursorPosition)).VisualHit;
                        hitResult.*
                        //_thumbnailsWindow.Left = (visualHit as ListViewItem).PointToScreen(new System.Windows.Point(0, 0)).X - (_thumbnailsWindow.ActualWidth / 2);
                        _thumbnailsWindow.Left = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition.X - (_thumbnailsWindow.ActualWidth / 2);
                        _thumbnailsWindow.Top = Top - _thumbnailsWindow.ActualHeight;
                    }*/

                    double newLeft = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition.X - (_thumbnailsWindow.ActualWidth / 2);

                    IEasingFunction ease = null;
                    try
                    {
                        ease = (IEasingFunction)App.Current.Resources["ThumbnailsWindowMovementEase"];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    DoubleAnimation leftAnimation = new DoubleAnimation()
                    {
                        Duration = TimeSpan.FromMilliseconds(1000),
                        To = newLeft
                    };

                    if (ease != null)
                        leftAnimation.EasingFunction = ease;

                    leftAnimation.Completed += (sneder, args) =>
                    {
                        _thumbnailsWindow.Left = newLeft;
                        //_thumbnailsWindow.BeginAnimation(LeftProperty, null);
                    };

                    _thumbnailsWindow.BeginAnimation(LeftProperty, null);
                    _thumbnailsWindow.BeginAnimation(LeftProperty, leftAnimation);
                    //}
                }
            }
        }

        private void PinnedApp_JumpListRequested(object sender, WindowEventArgs e)
        {
            if (_thumbnailsWindow.IsVisible)
                _thumbnailsWindow.Hide();

            _jumpListWindow.Left = Start9.UI.Wpf.Statics.SystemScaling.CursorPosition.X - (_jumpListWindow.ActualWidth / 2);
            _jumpListWindow.ShowWindow(sender as PinnedApplication, e.Window);
        }

        public PinnedApplication GetApplicationByWindow(ProcessWindow window)
        {
            PinnedApplication app = null;
            foreach (PinnedApplication a in OpenApplications)
            {
                foreach (ProcessWindow w in a.OpenWindows)
                {
                    if (w.Handle == window.Handle)
                    {
                        app = a;
                        break;
                    }
                }
            }
            return app;
        }


        private void TaskItemButton_Click(object sender, RoutedEventArgs e)
        {
            var programWindow = ((sender as Button).Tag as ProcessWindow);
            programWindow.Show();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset - ResizeIntervalDistance);
            else
                TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset + ResizeIntervalDistance);
        }

        bool _isChecked = false;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Debug.WriteLine("StartButton_Click");

                int param1 = 0;
                int param2 = 0;

                if (DockMode == AppBarDockMode.Bottom)
                    param2 = (int)SystemParameters.WorkArea.Bottom;
                if (DockMode == AppBarDockMode.Right)
                    param2 = (int)SystemParameters.WorkArea.Right;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    (Application.Current as App).service.SendMessage(new Message(MessageCommand.Open)
                    {
                        Param1 = param1,
                        Param2 = param2
                    });
                }));

                if (!_isChecked)
                {
                    UpdateStartButtonState(true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void UpdateStartButtonState(bool value)
        {
            _isChecked = value;
            StartButton.IsChecked = _isChecked;
        }

        private void TaskBandListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = TaskBandListView.SelectedItem;
            if (item != null)
                TaskBandListView.ScrollIntoView(item);
        }

        private void QuickLaunchMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            (QuickLaunch.Items[0] as ListView).ItemsSource = new DiskItem(Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar")).SubItems;
            QuickLaunch.Visibility = Visibility.Visible;
        }

        private void QuickLaunchMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            (QuickLaunch.Items[0] as ListView).ItemsSource = null;
            QuickLaunch.Visibility = Visibility.Collapsed;
        }

        private void ToolbarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                (e.AddedItems[0] as DiskItem).Open();
                (sender as ListView).SelectedItem = null;
            }
        }

        private void TaskManagerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("taskmgr");
        }

        private void ShowDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            _shell.ToggleDesktop();
        }

        private void CascadeWindowsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _shell.CascadeWindows();
        }

        private void StackWindowsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _shell.TileVertically();
        }

        private void ShowWindowsSideBySideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _shell.TileHorizontally();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Config.SettingsWindow.Show();
            Config.SettingsWindow.Focus();
            Config.SettingsWindow.Activate();
        }

        private void TaskBandUpButton_Click(object sender, RoutedEventArgs e)
        {
            TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset - ResizeIntervalDistance);
        }

        private void TaskBandDownButton_Click(object sender, RoutedEventArgs e)
        {
            TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset + ResizeIntervalDistance);
        }

        bool _scrollAttachmentDirection = true;

        private void TaskBandScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_scrollAttachmentDirection)
            {
                if (e.NewValue > 0)
                    TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset + ResizeIntervalDistance);
                else
                    TaskBandScrollViewer.ScrollToVerticalOffset(TaskBandScrollViewer.VerticalOffset - ResizeIntervalDistance);

                _scrollAttachmentDirection = false;
            }

            //TaskBandScrollBar.Value = TaskBandScrollViewer.VerticalOffset;
            _scrollAttachmentDirection = true;
        }

        private void TaskBandListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = null;
            PinnedApplication app = null;

            foreach (PinnedApplication a in OpenApplications)
            {
                var listItem = TaskBandListView.ContainerFromElement(a) as ListViewItem;
                if (listItem.IsMouseOver)
                {
                    item = listItem;
                    app = a;
                    break;
                }
            }

            if (item != null)
            {
                var point = _thumbnailsWindow.PointToScreen(new System.Windows.Point(0, 0));
                _thumbnailsWindow.Left = (point.X + (item.ActualWidth / 2)) - (_thumbnailsWindow.ActualWidth / 2);
                _thumbnailsWindow.OpenWindows = app.OpenWindows;
                _thumbnailsWindow.Show();
            }
            else if (!_thumbnailsWindow.IsMouseOver)
                _thumbnailsWindow.Hide();
        }

        private void TaskViewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ActionCenterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SearchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem m in SearchMenuItem.Items)
            {
                if (m == sender)
                    m.IsChecked = true;
                else
                    m.IsChecked = false;
            }

            if (sender == SearchHiddenMenuItem)
            {
                SearchBox.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Collapsed;
            }
            else if (sender == SearchBoxMenuItem)
            {
                SearchBox.Visibility = Visibility.Visible;
                SearchButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchBox.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
            }
        }

        private void TaskViewMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                TaskViewButton.Visibility = Visibility.Visible;
        }

        private void TaskViewMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                TaskViewButton.Visibility = Visibility.Collapsed;
        }

        private void ActionCenterMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                ActionCenterButton.Visibility = Visibility.Visible;
        }

        private void ActionCenterMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                ActionCenterButton.Visibility = Visibility.Collapsed;
        }
    }
}

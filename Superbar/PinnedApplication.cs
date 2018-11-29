using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsSharp.DiskItems;
using WindowsSharp.Processes;

namespace Superbar
{
    public class PinnedApplication : DependencyObject, INotifyPropertyChanged
    {
        public DiskItem DiskApplication { get; set; }
        public ObservableCollection<ProcessWindow> OpenWindows { get; set; } = new ObservableCollection<ProcessWindow>();

        bool _isPinned = false;
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                _isPinned = value;
                if (value == true)
                    Config.AddPinnedApp(DiskApplication.ItemPath);
                else// if (Config.PinnedApps.Contains(DiskApplication.ItemPath))
                    Config.RemovePinnedApp(DiskApplication.ItemPath);

                NotifyPropertyChanged("IsPinned");
                //Debug.WriteLine("IsPinned updated: " + _isPinned.ToString());
                IsPinnedChanged?.Invoke(this, new EventArgs());
            }
        }

        /*FrameworkElement _listViewControl;
        public FrameworkElement ListViewControl
        {
            get => _listViewControl;
            set
            {
                _listViewControl = value;
                NotifyPropertyChanged("ListViewControl");
                if ((App.Current.MainWindow as MainWindow).Orientation == System.Windows.Controls.Orientation.Vertical)
                    ListSize = _listViewControl.ActualHeight;
                else
                    ListSize = _listViewControl.ActualWidth;
            }
        }*/

        double _listSize;
        public double ListSize
        {
            get => _listSize;
            set
            {
                _listSize = value;
                NotifyPropertyChanged("ListSize");
            }
        }

        double _itemSize = 160;
        public double ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                NotifyPropertyChanged("ItemSize");
                EvaluateItemCombining(false);
            }
        }

        Config.CombineMode _taskItemCombineMode = Config.CombineMode.Always;
        public Config.CombineMode TaskItemCombineMode
        {
            get => _taskItemCombineMode;
            set
            {
                //var oldMode = _taskItemCombineMode;

                _taskItemCombineMode = value;
                NotifyPropertyChanged("TaskItemCombineMode");

                /*if (oldMode != _taskItemCombineMode)
                    EvaluateItemCombining();*/
            }
        }

        public void EvaluateItemCombining()
        {
            EvaluateItemCombining(true);
        }



        public void EvaluateItemCombining(bool calculateItemSizes)
        {
            double max = ((App.Current.MainWindow as MainWindow).TaskBandScrollViewer.ActualWidth / 2);
            double width = (ItemSize * OpenWindows.Count);

            double size = 160;// (OpenWindows.Count / 160) / (max / ListSize); //(OpenWindows.Count * 0.25);
                              /*if (size > 160)
                                  size = 160;*/

            if (calculateItemSizes)
            {
                if (Config.TaskbarCombineMode != Config.CombineMode.Always)
                {
                    double minMax = max * 1.25;
                    //double totalSize = size * OpenWindows.Count;

                    if (false) //width > minMax)
                    {
                        ////double reducedSize = size;// + (size / 160);
                        /*while ((reducedSize * OpenWindows.Count) > minMax)
                        {
                            reducedSize -= 64;// (size / OpenWindows.Count); //((size / OpenWindows.Count) / 2);
                        }*/



                        size = minMax / OpenWindows.Count; //size - ((width - minMax) * OpenWindows.Count);//reducedSize;// 128; // 160 * (4 / 5);// reducedSize;
                    }
                    //if (size * OpenWindows.Count )
                    /*if (size < 0)
                        size = size * -1;*/

                    ItemSize = size;
                }
            }

            if (Config.TaskbarCombineMode == Config.CombineMode.WhenFull)
            {
                //Debug.WriteLine("Size: " + ListSize.ToString() + ", " + DiskApplication.ItemPath);
                if (width <= max)
                    TaskItemCombineMode = Config.CombineMode.Never;
                else
                    TaskItemCombineMode = Config.CombineMode.Always;
            }
            else
                TaskItemCombineMode = Config.TaskbarCombineMode;


            /*if (DiskApplication.ItemPath.Contains("moon"))
                Debug.WriteLine("new ItemSize: " + ItemSize + ", new TaskItemCombineMode: " + TaskItemCombineMode.ToString() + ", new ListSize: " + ListSize.ToString() + ", " + DiskApplication.ItemPath);*/
        }

        bool _areThumbnailsShown = false;
        public bool AreThumbnailsShown
        {
            get => _areThumbnailsShown;
            set
            {
                _areThumbnailsShown = value;
                NotifyPropertyChanged("AreThumbnailsShown");
                //Debug.WriteLine("AreThumbnailsShown updated:" + value.ToString());
                if (value == true)
                    ThumbnailsRequested?.Invoke(this, new WindowEventArgs(SelectedWindow));
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
                //Debug.WriteLine("Selected Window changed");
                if (_selectedWindow != null)
                    ShowWindow(_selectedWindow);
                /*else
                    Debug.WriteLine("_selectedWindow is null");*/
            }
        }

        private async void ShowWindow(ProcessWindow window)
        {
            await Task.Run(() =>
            {
                if (window != null)
                    window.Show();
            });
        }

        bool _isApplicationActive = false;

        public bool IsApplicationActive
        {
            get
            {
                //_isApplicationActive = GetContainsActiveWindow();
                return _isApplicationActive;
            }
            set
            {
                if (!Config.IsDraggingPinnedApplication)
                {
                    var oldValue = _isApplicationActive;
                    _isApplicationActive = value;
                    if ((value == true) && (oldValue != value))
                    {

                        if (OpenWindows.Count == 0)
                            DiskApplication.Open();
                        else if (OpenWindows.Count == 1)
                            ShowWindow(OpenWindows[0]);
                        else if (AreThumbnailsShown == true)
                            ThumbnailsRequested?.Invoke(this, new WindowEventArgs(SelectedWindow));
                    }
                }
                NotifyPropertyChanged("IsApplicationActive");
            }
        }

        bool _isJumpListOpen = false;
        public bool IsJumpListOpen
        {
            get => _isJumpListOpen;
            set
            {
                _isJumpListOpen = value;
                NotifyPropertyChanged("IsJumpListOpen");

                if (_isJumpListOpen == true)
                {
                    InvokeJumpList();
                }
            }
        }

        private void InvokeJumpList()
        {
            JumpListRequested?.Invoke(this, new WindowEventArgs(SelectedWindow));
        }

        bool GetContainsActiveWindow()
        {
            bool returnValue = false;

            foreach (ProcessWindow w in OpenWindows)
                if (ProcessWindow.ActiveWindow.Handle == w.Handle)
                    returnValue = true;

            return returnValue;
        }

        public PinnedApplication(DiskItem item)
        {
            DiskApplication = item;

            foreach (ProcessWindow win in ProcessWindow.ProcessWindows)
            {
                string path = win.Process.GetExecutablePath();
                /*if (Environment.OSVersion.Version >= new Version(6, 2, 8400, 0))
                {
                    var appx = AppxMethods.AppxPackage.FromProcess(win.Process);
                    if (appx != null)
                    {
                        MessageBox.Show("DisplayName: " + appx.DisplayName + "\nApplicationUserModelId: " + appx.ApplicationUserModelId + "\nFullName: " + appx.FullName, "Package information");
                    }
                }*/
                /*try
                {*/
                //Debug.WriteLine(win.Process.MainModule.FileName + "    " + DiskApplication.ItemRealName);
                if (path.ToLowerInvariant() == DiskApplication.ItemPath.ToLowerInvariant())
                    {
                        OpenWindows.Add(win);
                        //Debug.WriteLine("WINDOW: " + win.Title);
                    }
                /*}
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }*/
            }

            ProcessWindow.WindowOpened += ProcessWindow_WindowOpened;
            ProcessWindow.ActiveWindowChanged += ProcessWindow_ActiveWindowChanged;
            ProcessWindow.WindowClosed += ProcessWindow_WindowClosed;

            IsApplicationActive = GetContainsActiveWindow();

            //IsJumpListOpen = true;
            //IsJumpListOpen = false;

            EvaluateItemCombining();

            Config.ConfigUpdated += Config_ConfigUpdated;
        }

        private void Config_ConfigUpdated(object sender, EventArgs e)
        {
            EvaluateItemCombining();
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<WindowEventArgs> ThumbnailsRequested;

        public event EventHandler<WindowEventArgs> JumpListRequested;

        public event EventHandler<EventArgs> LastWindowClosed;

        public event EventHandler<EventArgs> IsPinnedChanged;

        private void ProcessWindow_WindowOpened(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                /*try
                {*/
                if (e.Window.Process.BelongsToExecutable(DiskApplication))
                {
                    bool proceed = true;
                    foreach (ProcessWindow w in OpenWindows)
                        if (w.Handle == e.Window.Handle)
                        {
                            proceed = false;
                            break;
                        }

                    if (proceed)
                    {
                        if (ProcessWindow.IsWindowUserAccessible(e.Window.Handle))
                        {
                            OpenWindows.Add(e.Window);
                            //ExamineActiveWindow(e.Window);
                            if ((e.Window != null) && (ProcessWindow.ActiveWindow.Handle == e.Window.Handle))
                            {
                                SelectedWindow = e.Window;
                                IsApplicationActive = true;
                            }
                        }
                    }
                }
                EvaluateItemCombining();
                /*}
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }*/
            }));
        }

        private void ProcessWindow_ActiveWindowChanged(object sender, WindowEventArgs e)
        {
            //Debug.WriteLine("Active Window changed");
            Dispatcher.Invoke(new Action(() =>
            {
                //bool found = false;
                ProcessWindow window = null;
                foreach (ProcessWindow w in OpenWindows)
                    if (w.Handle == e.Window.Handle)
                    {
                        window = w;
                        IsApplicationActive = true;
                        break;
                    }

                SelectedWindow = window;

                if (window == null)
                {
                    IsApplicationActive = false;
                }
            }));
        }

        private void ProcessWindow_WindowClosed(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                foreach (ProcessWindow w in OpenWindows)
                {
                    if (w.Handle == e.Window.Handle)
                    {
                        OpenWindows.Remove(w);

                        if ((OpenWindows.Count == 0) & (!this.IsPinned))
                        {
                            //Debug.WriteLine("IsPinned: " + this.IsPinned.ToString() + ", invoking LastWindowClosed");
                            LastWindowClosed?.Invoke(this, new EventArgs());
                        }

                        break;
                    }
                }
                EvaluateItemCombining();
            }));
        }
    }
}

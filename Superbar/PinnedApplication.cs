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
        public bool IsPinned { get; set; } = false;
        public DiskItem DiskApplication { get; set; }
        public ObservableCollection<ProcessWindow> OpenWindows { get; set; } = new ObservableCollection<ProcessWindow>();

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
                    _selectedWindow.Show();
            }
        }

        bool _isApplicationActive = false;

        public bool IsApplicationActive
        {
            get
            {
                _isApplicationActive = GetContainsActiveWindow();
                return _isApplicationActive;
            }
            set
            {
                if (value == true)
                {
                    if (OpenWindows.Count == 1)
                    {
                        OpenWindows[0].Show();
                    }
                    _isApplicationActive = true;
                }
                NotifyPropertyChanged("IsApplicationActive");
            }
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
                //Debug.WriteLine(win.Process.MainModule.FileName + "    " + DiskApplication.ItemRealName);
                if (win.Process.MainModule.FileName == DiskApplication.ItemPath)
                {
                    OpenWindows.Add(win);
                    //Debug.WriteLine("WINDOW: " + win.Title);
                }
            }

            ProcessWindow.WindowOpened += ProcessWindow_WindowOpened;
            ProcessWindow.ActiveWindowChanged += ProcessWindow_ActiveWindowChanged;
            ProcessWindow.WindowClosed += ProcessWindow_WindowClosed;

            _isApplicationActive = GetContainsActiveWindow();
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<WindowEventArgs> ThumbnailsRequested;

        public event EventHandler<EventArgs> LastWindowClosed;

        private void ProcessWindow_WindowOpened(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (e.Window.Process.MainModule.FileName == DiskApplication.ItemPath)
                    {
                        bool proceed = true;
                        foreach (ProcessWindow w in OpenWindows)
                            if (w.Handle == e.Window.Handle)
                            {
                                proceed = false;
                                break;
                            }

                        if (proceed)
                            OpenWindows.Add(e.Window);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }));
        }

        private void ProcessWindow_ActiveWindowChanged(object sender, WindowEventArgs e)
        {
            //Debug.WriteLine("Active Window changed");
            Dispatcher.Invoke(new Action(() =>
            {
                ProcessWindow window = null;
                foreach (ProcessWindow w in OpenWindows)
                    if (w.Handle == e.Window.Handle)
                    {
                        window = w;
                        IsApplicationActive = IsApplicationActive;
                        break;
                    }

                SelectedWindow = window;
                if (window == null)
                    IsApplicationActive = false;
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
                        if ((OpenWindows.Count == 0) && (!IsPinned))
                            LastWindowClosed?.Invoke(this, new EventArgs());
                        break;
                    }
                }
            }));
        }
    }
}

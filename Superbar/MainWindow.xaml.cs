using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.IO;

using Start9.Api;
using Start9.Api.Objects;
using Start9.Api.Programs;

using Timer = System.Timers.Timer;
using System.Windows.Controls.Primitives;
using Start9.Api.Tools;

namespace Superbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : QuadContentWindow
    {
        Timer activeWindowTimer = new Timer(1);

        ObservableCollection<TaskItemGroup> openWindows = new ObservableCollection<TaskItemGroup>();

        public ObservableCollection<TaskItemGroup> OpenWindows
        {
            get => openWindows;
            set => openWindows = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            Left = 0;
            Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Bottom - Height;
            InitialPopulate();
            ProgramWindow.WindowOpened += InsertCreatedWindow;
            //ProgramWindow.WindowClosed += RemoveClosedWindow;
            activeWindowTimer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                    {
                        IntPtr active = WinApi.GetForegroundWindow();
                        foreach (TaskItemGroup grp in OpenWindows)
                        {
                            bool containsActive = false;
                            foreach (ProgramWindow win in grp.Windows)
                            {
                                if (win.Hwnd == active)
                                {
                                    Debug.WriteLine("ACTIVE WINDOW FOUND" + active);
                                    containsActive = true;
                                    grp.ActiveWindowItem = win;
                                    grp.IsActiveWindow = true;
                                }
                            }
                            if (!containsActive)
                            {
                                grp.ActiveWindowItem = null;
                                grp.IsActiveWindow = false;
                            }
                        }
                    }));
            };
        }

        private void QuadContentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            activeWindowTimer.Start();
        }

        public void InitialPopulate()
        {
            ObservableCollection<TaskItemGroup> groups = new ObservableCollection<TaskItemGroup>();
            List<string> processNames = new List<string>();
            //1
            foreach (var wind in ProgramWindow.UserPerceivedProgramWindows)
            {
                try
                {
                    if (!processNames.Contains(wind.Process.MainModule.FileName))
                    {
                        processNames.Add(wind.Process.MainModule.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Process not added to list\n" + ex);
                }
            }

            foreach (string s in processNames)
            {
                TaskItemGroup group = new TaskItemGroup(s);
                groups.Add(group);
            }

            foreach (var wind in ProgramWindow.ProgramWindows)
            {
                foreach (TaskItemGroup t in groups)
                {
                    try
                    {
                        if (wind.Process.MainModule.FileName == t.ExecutableName)
                        {
                            Debug.WriteLine(wind.Process.MainModule.FileName + "    " + t.ExecutableName);
                            t.Windows.Add(wind);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            OpenWindows = groups;
        }

        public void InsertCreatedWindow(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                {
                    bool addedToExistingGroup = false;
                    foreach (TaskItemGroup t in OpenWindows)
                    {
                        try
                        {
                            Debug.WriteLine(t.ExecutableName);
                            if (e.Window.Process.MainModule.FileName == t.ExecutableName)
                            {
                                t.Windows.Add(e.Window);
                                addedToExistingGroup = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }

                    if (!addedToExistingGroup)
                    {
                        TaskItemGroup group = new TaskItemGroup(e.Window.Process.MainModule.FileName);
                        /*{
                            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x01, 0x0, 0x0, 0x0)),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Margin = new Thickness(2, 0, 2, 0),
                            Tag = e.Window.Process.MainModule.FileName
                        };*/

                        group.Windows.Add(e.Window);// ProgramStackPanel.ProgramWindowsList.Add(e.Window);
                    }
                }
            }));
        }

        public void RemoveClosedWindow(object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                {
                    foreach (TaskItemGroup t in OpenWindows)
                    {
                        try
                        {
                            foreach (ProgramWindow p in t.Windows)
                            {
                                if (p.Hwnd == e.Window.Hwnd)
                                {
                                    t.Windows.Remove(p);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
            }));
        }

        private void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            var clockTimer = new Timer(1);
            clockTimer.Elapsed += (o, args) => Dispatcher.Invoke(new Action(
                () => { (sender as TextBlock).Text = DateTime.Now.ToShortTimeString() + "\n" + DateTime.Now.ToShortDateString(); }
            ));
            clockTimer.Start();
        }

        private void ShowDesktopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClockButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrayFlyoutToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }/*

        double oldValue = 0;

        private void TaskbandScrollViewer_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            double offset = TaskbandScrollViewer.VerticalOffset;
            if (e.NewValue > oldValue)
            {
                TaskbandScrollViewer.ScrollToVerticalOffset(Math.Round(oldValue + 40, 0, MidpointRounding.AwayFromZero));
            }
            else if (offset < oldValue)
            {
                TaskbandScrollViewer.ScrollToVerticalOffset(Math.Round(oldValue - 40, 0, MidpointRounding.AwayFromZero));
            }
            oldValue = offset;
        }

        private void TaskbandScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //(sender as ScrollViewer).
        }

        private void TaskbandScrollBar_Loaded(object sender, RoutedEventArgs e)
        {
            TaskbandScrollBar.ValueChanged += TaskbandScrollBar_ValueChanged;
        }

        private void TaskbandScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Double finalValue = TaskBandScrollViewer.HorizontalOffset - (e.Delta * 2);
        }*/

        private void TaskbandScrollBar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                TaskbandScrollBar.Value = TaskbandScrollBar.Value - 1;
            }
            else if (e.Delta < 0)
            {
                TaskbandScrollBar.Value = TaskbandScrollBar.Value + 1;
            }
        }

        double offset = 0;
        private void TaskbandScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var pad = TaskbandListView.Margin;
            if ((e.NewValue > e.OldValue) & (pad.Top <= 0))
            {
                TaskbandListView.Margin = new Thickness(pad.Left, pad.Top - 40, pad.Right, pad.Bottom);
                offset = offset + 40;
            }
            else if (e.NewValue < e.OldValue)//((e.NewValue < e.OldValue) & ((pad.Top * -1) <= (TaskbandListView.ActualHeight - offset)))
            {
                TaskbandListView.Margin = new Thickness(pad.Left, pad.Top + 40, pad.Right, pad.Bottom);
                offset = offset - 40;
            }
            offset = offset - 1;
            Debug.WriteLine(TaskbandListView.Margin.Top + "    " + (TaskbandListView.ActualHeight - offset));
        }

        private void TaskItemButton_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine((sender as ToggleButton).Tag.GetType().ToString());
            new ProgramWindow((IntPtr)(sender as ToggleButton).Tag).Show();
        }
    }
}

//please
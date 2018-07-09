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

        public ObservableCollection<TaskItemGroup> OpenWindows { get; set; } = new ObservableCollection<TaskItemGroup>();

        public MainWindow()
        {
            InitializeComponent();
            DockMode = Start9.Api.AppBar.AppBarDockMode.Bottom;
            DockedWidthOrHeight = 40;
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
                            var containsActive = false;
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

        private void QuadContentWindow_Loaded(Object sender, RoutedEventArgs e)
        {
            activeWindowTimer.Start();
        }

        public void InitialPopulate()
        {
            ObservableCollection<TaskItemGroup> groups = new ObservableCollection<TaskItemGroup>();
            List<String> processNames = new List<String>();
            //1
            foreach (var wind in ProgramWindow.ProgramWindows)
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

            foreach (var s in processNames)
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

        public void InsertCreatedWindow(Object sender, WindowEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                {
                    var addedToExistingGroup = false;
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

        public void RemoveClosedWindow(Object sender, WindowEventArgs e)
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

        private void Clock_Loaded(Object sender, RoutedEventArgs e)
        {
            var clockTimer = new Timer(1);
            clockTimer.Elapsed += (o, args) => Dispatcher.Invoke(new Action(
                () => { (sender as TextBlock).Text = DateTime.Now.ToShortTimeString() + "\n" + DateTime.Now.ToShortDateString(); }
            ));
            clockTimer.Start();
        }

        public static RoutedCommand SubmittedCommand = new RoutedCommand();
        private void ShowDesktopButton_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void ClockButton_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void TrayFlyoutToggleButton_Click(Object sender, RoutedEventArgs e)
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

        private void TaskbandScrollBar_MouseWheel(Object sender, MouseWheelEventArgs e)
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

        Double offset = 0;
        private void TaskbandScrollBar_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
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

        private void TaskItemButton_Click(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine((sender as ToggleButton).Tag.GetType().ToString());
            new ProgramWindow((IntPtr)(sender as ToggleButton).Tag).Show();
            Debug.WriteLine("");
        }

        private void SearchButton_Click(Object sender, RoutedEventArgs e)
        {
            SuperbarAddIn.Instance.Host.SendMessage(new Message(DBNull.Value, ((SuperbarMessageContract) SuperbarAddIn.Instance.MessageContract).SearchClickedEntry));
        }

        private void Start_Click(Object sender, RoutedEventArgs e)
        {
            SuperbarAddIn.Instance.Host.SendMessage(new Message(DBNull.Value, ((SuperbarMessageContract) SuperbarAddIn.Instance.MessageContract).StartClickedEntry));

        }

        private void TaskView_Click(Object sender, RoutedEventArgs e)
        {
            SuperbarAddIn.Instance.Host.SendMessage(new Message(DBNull.Value, ((SuperbarMessageContract) SuperbarAddIn.Instance.MessageContract).TaskViewClickedEntry));
        }

        private void CommandBinding_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            SuperbarAddIn.Instance.Host.SendMessage(new Message(SearchBox.Text, ((SuperbarMessageContract)SuperbarAddIn.Instance.MessageContract).SearchInvokedEntry));
        }
    }
    //pls
}
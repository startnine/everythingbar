using Start9.UI.Wpf.Converters;
using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsSharp.Processes;

namespace Superbar
{
    /// <summary>
    /// Interaction logic for JumpListWindow.xaml
    /// </summary>
    public partial class JumpListWindow : ShadowedWindow
    {
        IconToImageBrushConverter iconConverter = new IconToImageBrushConverter();

        string _closeOneWindowText = "Close window";
        string _closeAllWindowsText = "Close all windows";

        string _pinAppText = "Pin to Superbar";
        string _unpinAppText = "Unpin from Superbar";

        PinnedApplication _app = null;
        ProcessWindow _window = null;

        public bool IsPinned { get; set; } = false;

        public JumpListWindow()
        {
            InitializeComponent();
        }

        public void ShowWindow(PinnedApplication app, ProcessWindow window)
        {
            _app = app;

            if (_app != null)
            {
                _window = window;

                if (!IsVisible)
                    Show();

                Focus();
                Activate();

                Resources["JumpListSmallIconImageBrush"] = iconConverter.Convert(_app.DiskApplication.ItemSmallIcon, null, "16", null);
                Resources["JumpListLargeIconImageBrush"] = iconConverter.Convert(_app.DiskApplication.ItemLargeIcon, null, "32", null);
                Resources["JumpListExtraLargeIconImageBrush"] = iconConverter.Convert(_app.DiskApplication.ItemExtraLargeIcon, null, "48", null);
                Resources["JumpListJumboIconImageBrush"] = iconConverter.Convert(_app.DiskApplication.ItemJumboIcon, null, "256", null);

                Resources["JumpListApplicationNameText"] = _app.DiskApplication.ItemDisplayName;
                Resources["JumpListApplicationIcon"] = _app.DiskApplication.ItemSmallIcon;

                IsPinned = _app.IsPinned;

                if (_app.IsPinned)
                    Resources["JumpListPinApplicationText"] = _unpinAppText;
                else
                    Resources["JumpListPinApplicationText"] = _pinAppText;

                if (_app.OpenWindows.Count > 0)
                    CloseWindowsListViewItem.Visibility = Visibility.Visible;
                else
                    CloseWindowsListViewItem.Visibility = Visibility.Collapsed;

                if (Config.ShowKillProcessesInJumpLists)
                    KillProcessesListViewItem.Visibility = Visibility.Visible;
                else
                    KillProcessesListViewItem.Visibility = Visibility.Collapsed;

                if (CloseWindowsListViewItem.IsVisible)
                {
                    if (_app.OpenWindows.Count > 1)
                        Resources["JumpListCloseWindowsText"] = _closeAllWindowsText;
                    else
                        Resources["JumpListCloseWindowsText"] = _closeOneWindowText;
                }
            }
        }

        private void ApplicationListViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
                _app.DiskApplication.Open();

            //Debug.WriteLine("Application");
            ResetSelection();
        }

        private void PinOrUnpinListViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
            {
                bool pinned = _app.IsPinned;
                if (pinned)
                    _app.IsPinned = false;
                else
                    _app.IsPinned = true;
            }
            ResetSelection();
        }

        private void CloseWindowsListViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (Config.TaskbarCombineMode == Config.CombineMode.Always)
            {
                if (_app != null)
                    foreach (ProcessWindow w in _app.OpenWindows)
                        w.Close();
            }
            else
            {
                if (_window != null)
                    _window.Close();
            }
            ResetSelection();
        }

        private void KillProcesses()
        {
            if (_app != null)
            {
                Process.Start(new ProcessStartInfo("taskkill", @"/f /im " + _app.DiskApplication.ItemRealName)
                {
                    CreateNoWindow = true
                });
            }
            ResetSelection();
        }

        private void ResetSelection()
        {
            BottomSegmentListView.SelectedItem = null; //DummyListViewItem;
            Hide();
        }

        private void JumpListWindow_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void HideJumpListButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void BottomSegmentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BottomSegmentListView.SelectedItem == ApplicationListViewItem)
            {
                ApplicationListViewItem_Click(sender, null);
            }
            else if (BottomSegmentListView.SelectedItem == PinOrUnpinListViewItem)
            {
                PinOrUnpinListViewItem_Click(sender, null);
            }
            else if (BottomSegmentListView.SelectedItem == KillProcessesListViewItem)
            {
                KillProcesses();
            }
            else if (BottomSegmentListView.SelectedItem == CloseWindowsListViewItem)
            {
                CloseWindowsListViewItem_Click(sender, null);
            }
        }

        new public void Hide()
        {
            _app.IsJumpListOpen = false;
            base.Hide();
        }

        private void RunAsAdminMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
                _app.DiskApplication.Open(WindowsSharp.DiskItems.DiskItem.OpenVerbs.Admin);
        }

        private void PropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_app != null)
                _app.DiskApplication.ShowProperties();
        }

        private void PinMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _app.IsPinned = true;
        }
    }
}

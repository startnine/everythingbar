using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Superbar
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : DecoratableWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            //Hide UI controls for NYI features
            AutoHideCheckBox.IsEnabled = false;
            /*NotificationAreaStackPanel*/ ManageTrayIconsButton.IsEnabled = false;
            RecentItemsMaxInJumpListDockPanel.IsEnabled = false;
            ShowMRUProgramsToggleSwitch.IsEnabled = false;
            ShowMRUFilesInJumpListsToggleSwitch.IsEnabled = false;
            ToolbarsTabItem.IsEnabled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.OldValue == false) && ((bool)e.NewValue == true))
            {
                LockBarCheckBox.IsChecked = Config.IsLocked;

                AutoHideCheckBox.IsChecked = Config.AutoHide;

                SmallButtonsCheckBox.IsChecked = Config.UseSmallIcons;

                AllowPeekCheckBox.IsChecked = Config.AllowPeekDesktop;

                if (Config.DockMode == AppBarWindow.AppBarDockMode.Left)
                    SetToggleButtons(BarPositionButtonsGrid, 1); //BarPositionComboBox.SelectedIndex = 1;
                else if (Config.DockMode == AppBarWindow.AppBarDockMode.Right)
                    SetToggleButtons(BarPositionButtonsGrid, 2); //BarPositionComboBox.SelectedIndex = 2;
                else if (Config.DockMode == AppBarWindow.AppBarDockMode.Top)
                    SetToggleButtons(BarPositionButtonsGrid, 3); //BarPositionComboBox.SelectedIndex = 3;
                else
                    SetToggleButtons(BarPositionButtonsGrid, 0); //BarPositionComboBox.SelectedIndex = 0;

                if (Config.TaskbarCombineMode == Config.CombineMode.WhenFull)
                    SetToggleButtons(TaskbarCombineModeGrid, 2); //TaskbarCombineModeComboBox.SelectedIndex = 2;
                else if (Config.TaskbarCombineMode == Config.CombineMode.Never)
                    SetToggleButtons(TaskbarCombineModeGrid, 3); //TaskbarCombineModeComboBox.SelectedIndex = 3;
                else
                {
                    if (Config.ShowCombinedLabels)
                        SetToggleButtons(TaskbarCombineModeGrid, 0); //TaskbarCombineModeComboBox.SelectedIndex = 0;
                    else
                        SetToggleButtons(TaskbarCombineModeGrid, 1); //TaskbarCombineModeComboBox.SelectedIndex = 1;
                }

                if (Config.TrayClockDateMode == Config.ClockDateMode.AlwaysShow)
                    SetToggleButtons(TaskbarClockDateModeGrid, 0); //TaskbarClockDateModeComboBox.SelectedIndex = 0;
                else if (Config.TrayClockDateMode == Config.ClockDateMode.NeverShow)
                    SetToggleButtons(TaskbarClockDateModeGrid, 2); //TaskbarClockDateModeComboBox.SelectedIndex = 2;
                else
                    SetToggleButtons(TaskbarClockDateModeGrid, 1); //TaskbarClockDateModeComboBox.SelectedIndex = 1;

                ShowKillProcessesEntryToggleSwitch.IsChecked = Config.ShowKillProcessesInJumpLists;
            }
        }

        private int GetSelectedToggleButton(Panel parent)
        {
            int returnIndex = -1;
            foreach (ToggleButton t in parent.Children)
            {
                if (t.IsChecked == true)
                {
                    returnIndex = parent.Children.IndexOf(t);
                    break;
                }
            }
            return returnIndex;
        }

        private void SetToggleButtons(Panel parent, int newSelectedIndex)
        {
            foreach (ToggleButton t in parent.Children)
            {
                if (parent.Children.IndexOf(t) == newSelectedIndex)
                    t.IsChecked = true;
                else
                    t.IsChecked = false;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Config.IsLocked = LockBarCheckBox.IsChecked.Value;

            Config.AutoHide = AutoHideCheckBox.IsChecked.Value;

            Config.UseSmallIcons = SmallButtonsCheckBox.IsChecked.Value;

            Config.AllowPeekDesktop = AllowPeekCheckBox.IsChecked.Value;

            Config.ShowKillProcessesInJumpLists = ShowKillProcessesEntryToggleSwitch.IsChecked.Value;

            if (GetSelectedToggleButton(BarPositionButtonsGrid) == 1) //BarPositionComboBox.SelectedIndex == 1)
                Config.DockMode = AppBarWindow.AppBarDockMode.Left;
            else if (GetSelectedToggleButton(BarPositionButtonsGrid) == 2) //BarPositionComboBox.SelectedIndex == 2)
                Config.DockMode = AppBarWindow.AppBarDockMode.Right;
            else if (GetSelectedToggleButton(BarPositionButtonsGrid) == 3) //BarPositionComboBox.SelectedIndex == 3)
                Config.DockMode = AppBarWindow.AppBarDockMode.Top;
            else
                Config.DockMode = AppBarWindow.AppBarDockMode.Bottom;

            if (GetSelectedToggleButton(TaskbarCombineModeGrid) == 2) //TaskbarCombineModeComboBox.SelectedIndex == 2)
                Config.TaskbarCombineMode = Config.CombineMode.WhenFull;
            else if (GetSelectedToggleButton(TaskbarCombineModeGrid) == 3) //TaskbarCombineModeComboBox.SelectedIndex == 3)
                Config.TaskbarCombineMode = Config.CombineMode.Never;
            else
            {
                Config.TaskbarCombineMode = Config.CombineMode.Always;

                if (GetSelectedToggleButton(TaskbarCombineModeGrid) == 1) //TaskbarCombineModeComboBox.SelectedIndex == 1)
                    Config.ShowCombinedLabels = false;
                else
                    Config.ShowCombinedLabels = true;
            }

            if (GetSelectedToggleButton(TaskbarClockDateModeGrid) == 1) //TaskbarClockDateModeComboBox.SelectedIndex == 1)
                Config.TrayClockDateMode = Config.ClockDateMode.Auto;
            else if (GetSelectedToggleButton(TaskbarClockDateModeGrid) == 2) //TaskbarClockDateModeComboBox.SelectedIndex == 2)
                Config.TrayClockDateMode = Config.ClockDateMode.NeverShow;
            else
                Config.TrayClockDateMode = Config.ClockDateMode.AlwaysShow;

            Config.InvokeConfigUpdated(this, new EventArgs());

            if (sender == OkButton)
                Close();
        }

        private void Start9SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (sender as ToggleButton);
            Panel parent = button.Parent as Panel;
            if (button.IsChecked == true)
            {
                foreach (ToggleButton t in parent.Children)
                {
                    if (t != button)
                        t.IsChecked = false;
                }
            }
        }
    }
}

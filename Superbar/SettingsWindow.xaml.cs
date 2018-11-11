using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

                if (Config.DockMode == AppBarWindow.AppBarDockMode.Left)
                    BarPositionComboBox.SelectedIndex = 1;
                else if (Config.DockMode == AppBarWindow.AppBarDockMode.Right)
                    BarPositionComboBox.SelectedIndex = 2;
                else if (Config.DockMode == AppBarWindow.AppBarDockMode.Top)
                    BarPositionComboBox.SelectedIndex = 3;
                else
                    BarPositionComboBox.SelectedIndex = 0;

                if (Config.TaskbarCombineMode == Config.CombineMode.WhenFull)
                    TaskbarCombineModeComboBox.SelectedIndex = 1;
                else if (Config.TaskbarCombineMode == Config.CombineMode.Never)
                    TaskbarCombineModeComboBox.SelectedIndex = 2;
                else
                    TaskbarCombineModeComboBox.SelectedIndex = 0;

                if (Config.TrayClockDateMode == Config.ClockDateMode.AlwaysShow)
                    TaskbarClockDateModeComboBox.SelectedIndex = 0;
                else if (Config.TrayClockDateMode == Config.ClockDateMode.NeverShow)
                    TaskbarClockDateModeComboBox.SelectedIndex = 2;
                else
                    TaskbarClockDateModeComboBox.SelectedIndex = 1;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Config.IsLocked = LockBarCheckBox.IsChecked.Value;

            Config.AutoHide = AutoHideCheckBox.IsChecked.Value;

            Config.UseSmallIcons = SmallButtonsCheckBox.IsChecked.Value;

            if (BarPositionComboBox.SelectedIndex == 1)
                Config.DockMode = AppBarWindow.AppBarDockMode.Left;
            else if (BarPositionComboBox.SelectedIndex == 2)
                Config.DockMode = AppBarWindow.AppBarDockMode.Right;
            else if (BarPositionComboBox.SelectedIndex == 3)
                Config.DockMode = AppBarWindow.AppBarDockMode.Top;
            else
                Config.DockMode = AppBarWindow.AppBarDockMode.Bottom;

            if (TaskbarCombineModeComboBox.SelectedIndex == 1)
                Config.TaskbarCombineMode = Config.CombineMode.WhenFull;
            else if (TaskbarCombineModeComboBox.SelectedIndex == 2)
                Config.TaskbarCombineMode = Config.CombineMode.Never;
            else
                Config.TaskbarCombineMode = Config.CombineMode.Always;

            if (TaskbarClockDateModeComboBox.SelectedIndex == 1)
                Config.TrayClockDateMode = Config.ClockDateMode.Auto;
            else if (TaskbarClockDateModeComboBox.SelectedIndex == 2)
                Config.TrayClockDateMode = Config.ClockDateMode.NeverShow;
            else
                Config.TrayClockDateMode = Config.ClockDateMode.AlwaysShow;

            Config.InvokeConfigUpdated(this, new EventArgs());

            if (sender == OkButton)
                Close();
        }
    }
}

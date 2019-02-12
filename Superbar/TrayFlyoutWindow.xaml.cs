using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Start9.UI.Wpf.Statics;
using WindowsSharp;

namespace Everythingbar
{
    /// <summary>
    /// Interaction logic for TrayFlyoutWindow.xaml
    /// </summary>
    public partial class TrayFlyoutWindow : ShadowedWindow
    {
        MainWindow _mainWindow
        {
            get => (Application.Current.MainWindow as MainWindow);
        }


        public ObservableCollection<SystemTrayItem> ExpandedTrayItems { get; set; } = new ObservableCollection<SystemTrayItem>();

        public TrayFlyoutWindow()
        {
            InitializeComponent();
        }

        private void TrayFlyoutWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                _mainWindow.ShowTrayFlyoutButton.IsChecked = true;
                UpdatePosition();
            }
            else
                _mainWindow.ShowTrayFlyoutButton.IsChecked = false;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdatePosition();
        }

        void UpdatePosition()
        {
            var point = _mainWindow.ShowTrayFlyoutButton.PointToScreen(new Point(0, 0));
            point.X = SystemScaling.RealPixelsToWpfUnits(point.X);
            point.Y = SystemScaling.RealPixelsToWpfUnits(point.Y);

            if (_mainWindow.Orientation == Orientation.Vertical)
                Top = (point.Y + (_mainWindow.ShowTrayFlyoutButton.ActualHeight / 2)) - (ActualHeight / 2);
            else
                Left = (point.X + (_mainWindow.ShowTrayFlyoutButton.ActualWidth / 2)) - (ActualWidth / 2);

            if (_mainWindow.DockMode == AppBarWindow.AppBarDockMode.Left)
                Left = _mainWindow.Left + _mainWindow.DockedWidthOrHeight;
            else if (_mainWindow.DockMode == AppBarWindow.AppBarDockMode.Top)
                Top = _mainWindow.Top + _mainWindow.DockedWidthOrHeight;
            else if (_mainWindow.DockMode == AppBarWindow.AppBarDockMode.Right)
                Left = _mainWindow.Left - ActualWidth;
            else
                Top = _mainWindow.Top - ActualHeight;
        }

        private void TrayFlyoutWindow_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }
    }
}

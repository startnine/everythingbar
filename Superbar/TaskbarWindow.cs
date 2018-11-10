using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Start9.UI.Wpf.Windows;

namespace Superbar
{
    public partial class TaskbarWindow : AppBarWindow
    {
        public object QuickLaunchArea
        {
            get => GetValue(QuickLaunchAreaProperty);
            set => SetValue(QuickLaunchAreaProperty, value);
        }

        public static readonly DependencyProperty QuickLaunchAreaProperty =
            DependencyProperty.Register("QuickLaunchArea", typeof(object), typeof(TaskbarWindow), new PropertyMetadata());

        public object TrayArea
        {
            get => GetValue(TrayAreaProperty);
            set => SetValue(TrayAreaProperty, value);
        }

        public static readonly DependencyProperty TrayAreaProperty =
            DependencyProperty.Register("TrayArea", typeof(object), typeof(TaskbarWindow), new PropertyMetadata());

        public TaskbarWindow()
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using WindowsSharp.Processes;

namespace Everythingbar
{
    public class CloseWindowButtonBehavior : Behavior<Button>
    {
        public ProcessWindow TargetWindow
        {
            get => (ProcessWindow)GetValue(TargetWindowProperty);
            set => SetValue(TargetWindowProperty, value);
        }

        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register("TargetWindow", typeof(ProcessWindow), typeof(CloseWindowButtonBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (TargetWindow != null)
                TargetWindow.Close();
        }
    }
}

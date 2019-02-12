using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Everythingbar
{
    public class ActualizerBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((AssociatedObject.Tag != null) && e.WidthChanged)
            {
                var tag = AssociatedObject.Tag as ListView;
                /*if ((!(AssociatedObject.Tag is double))
                    ||
                    (AssociatedObject.Tag is double) && ((double)AssociatedObject.Tag != AssociatedObject.ActualWidth)
                    )
                {*/
                if (tag.IsVisible)
                    AssociatedObject.Opacity = tag.ActualWidth;
                //AssociatedObject.Height = e.NewSize.Height;// AssociatedObject.ActualHeight;
                //}
            }
        }
    }
}

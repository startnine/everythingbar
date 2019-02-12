using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WindowsSharp.DiskItems;

namespace Superbar
{
    [TemplatePart(Name = PartResizeThumb, Type = typeof(Thumb))]
    public class SizableToolBar : ToolBar
    {
        const String PartResizeThumb = "PART_ResizeThumb";

        Thumb _resizeThumb;

        public override void OnApplyTemplate()
        {
            Dispatcher.BeginInvoke(new Action(() => InvalidateMeasure()));
            base.OnApplyTemplate();
            _resizeThumb = GetTemplateChild(PartResizeThumb) as Thumb;
            _resizeThumb.DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            /*Dispatcher.BeginInvoke(new Action(() =>
            {
                InvalidateMeasure();
            }));*/
            if (double.IsInfinity(Width) || double.IsNaN(Width))
                Width = ActualWidth;

            double newWidth = Width + e.HorizontalChange;
            if ((newWidth >= 0) && (newWidth >= MinWidth)/* && (newWidth <= DesiredSize.Width)*/)
                Width = newWidth;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Dispatcher.BeginInvoke(new Action(() => InvalidateMeasure()));
        }
    }

    public static class FolderToolBars
    {
        public static SizableToolBar CreateToolBar(DiskItem item)
        {
            var bar = new SizableToolBar();
            //bar.DataContext = item;
            Binding itemsBinding = new Binding()
            {
                Source = item,
                Path = new PropertyPath("SubItems"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(bar, ToolBar.ItemsSourceProperty, itemsBinding);
            return bar;

            /*var list = new ListView()
            {
                ItemsSource = item.SubItems
            };
            list.SelectionChanged += ToolbarListView_SelectionChanged;
            bar.Items.Add(list);*/
            /*bar.Tag = item.ItemPath;

            bool containsThis = false;
            foreach (DiskItem d in Config.FolderToolBars)
            {
                if (d.ItemPath.ToLowerInvariant() == item.ItemPath.ToLowerInvariant())
                    containsThis = true;
            }
            if (!containsThis)
            {
                Config.FolderToolBars.Add(item);
                return bar;
            }
            else
                return null;*/
        }

        /*private static void ToolbarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                (e.AddedItems[0] as DiskItem).Open();
                (sender as ListView).SelectedItem = null;
            }
        }*/
    }
}

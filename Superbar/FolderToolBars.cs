using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WindowsSharp.DiskItems;

namespace Superbar
{
    public static class FolderToolBars
    {
        public static ToolBar CreateToolBar(DiskItem item)
        {
            var bar = new ToolBar();
            var list = new ListView()
            {
                ItemsSource = item.SubItems
            };
            list.SelectionChanged += ToolbarListView_SelectionChanged;
            bar.Items.Add(list);
            bar.Tag = item.ItemPath;

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
                return null;
        }

        private static void ToolbarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                (e.AddedItems[0] as DiskItem).Open();
                (sender as ListView).SelectedItem = null;
            }
        }
    }
}

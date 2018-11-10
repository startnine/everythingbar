using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WindowsSharp.DiskItems;
using WindowsSharp.Processes;

namespace Superbar
{
    public class ProcessWindowSet
    {
        Process process;

        public List<ProcessWindow> Windows { get; set; } = new List<ProcessWindow>();

        public bool IsRunning
        {
            get => Windows.Count > 0;
        }

        public Icon SmallIcon
        {
            get => new DiskItem(process.MainModule.FileName).ItemSmallIcon;
        }

        public Icon LargeIcon
        {
            get => new DiskItem(process.MainModule.FileName).ItemLargeIcon;
        }

        public Icon ExtraLargeIcon
        {
            get => new DiskItem(process.MainModule.FileName).ItemExtraLargeIcon;
        }

        public Icon JumboIcon
        {
            get => new DiskItem(process.MainModule.FileName).ItemJumboIcon;
        }

        public int SelectedIndex { get; set; } = -1;

        public ProcessWindowSet(List<ProcessWindow> win, Process p)
        {
            process = p;
            Windows = win;

            //Windows[0].ico
            ProcessWindow.WindowOpened += (sneder, args) =>
            {
                if ((args as WindowEventArgs).Window.Process.Id == p.Id)
                    Windows.Add((args as WindowEventArgs).Window);
            };


            ProcessWindow.ActiveWindowChanged += (sneder, args) =>
            {
                SelectedIndex = -1;

                foreach (ProcessWindow w in Windows)
                {
                    if (w.Handle == (args as WindowEventArgs).Window.Handle)
                    {
                        SelectedIndex = Windows.IndexOf(w);
                        break;
                    }
                }
            };

            ProcessWindow.WindowClosed += (sneder, args) =>
            {

                foreach (ProcessWindow w in Windows)
                {
                    if (w.Handle == (args as WindowEventArgs).Window.Handle)
                    {
                        Windows.Remove(w);
                        break;
                    }
                }
            };
        }
    }

    /*public static class ExtraListProperties
    {
        private static readonly ConditionalWeakTable<List<ProcessWindow>, object> _tags = new ConditionalWeakTable<List<ProcessWindow>, object>();

        public static void SetTag(this List<ProcessWindow> @this, object tag)
        {
            _tags.Remove(@this);
            _tags.Add(@this, tag);
        }

        public static object GetTag(this List<ProcessWindow> @this)
        {
            object color;
            if (_tags.TryGetValue(@this, out color)) return color;
            return default(object);
        }
    }*/

    public class ProcessToWindowsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Process).Windows();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

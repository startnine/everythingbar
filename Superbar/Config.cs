using Start9.UI.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsSharp.Processes;
//using static WindowsSharp.Processes.AppxMethods;

namespace Superbar
{
    public static class Config
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int flags, bool inherit, int dwProcessId);

        public static string GetExecutablePath(Process process)
        {
            string returnValue = string.Empty;
            StringBuilder stringBuilder = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(0x1000, false, process.Id);

            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = stringBuilder.Capacity;

                    if (QueryFullProcessImageName(hprocess, 0, stringBuilder, out size))
                        returnValue = stringBuilder.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    try
                    {
                        returnValue = process.MainModule.FileName;
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc);
                    }
                }
            }
            /*var package = AppxPackage.FromProcess(process);
            if (package != null)
            {
                Debug.WriteLine("PACKAGE.APPLICATIONUSERMODELID: " + package.ApplicationUserModelId);
                returnValue = package.ApplicationUserModelId;
            }*/
            /*else
                Debug.WriteLine("PACKAGE IS NULL");*/

            //Debug.WriteLine("returnValue: " + returnValue);
            return returnValue;
        }

        static Config()
        {
            if (!File.Exists(PinnedAppsPath))
            {
                string dir = Path.GetDirectoryName(PinnedAppsPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(PinnedAppsPath, new List<string>()
                {
                }.ToArray());
            }
        }

        public static bool IsDraggingPinnedApplication = false;

        public static bool IsLocked { get; set; } = true;

        public static bool AutoHide { get; set; } = false;

        public static bool UseSmallIcons { get; set; } = false;

        public static AppBarWindow.AppBarDockMode DockMode { get; set; } = AppBarWindow.AppBarDockMode.Bottom;

        public static string PinnedAppsPath = Environment.ExpandEnvironmentVariables(@"%appdata%\Start9\TempData\Superbar_PinnedApps.txt");

        public static bool AddPinnedApp(string path)
        {
            var origStrings = PinnedApps;


            bool add = true;

            foreach (string t in origStrings)
            {
                if (Environment.ExpandEnvironmentVariables(path.ToLowerInvariant()) == Environment.ExpandEnvironmentVariables(t.ToLowerInvariant()))
                {
                    add = false;
                    break;
                }
            }

            if (add)
            {
                //Debug.WriteLine("string: " + path);
                origStrings.Add(path);
            }

            File.WriteAllLines(PinnedAppsPath, origStrings.ToArray());
            return add;
        }

        public static bool RemovePinnedApp(string path)
        {
            var origStrings = PinnedApps;


            bool remove = false;

            foreach (string t in origStrings)
            {
                if (Environment.ExpandEnvironmentVariables(path.ToLowerInvariant()) == Environment.ExpandEnvironmentVariables(t.ToLowerInvariant()))
                {
                    origStrings.Remove(t);
                    remove = false;
                    break;
                }
            }

            File.WriteAllLines(PinnedAppsPath, origStrings.ToArray());
            return remove;
        }

        public static List<string> PinnedApps
        {
            get => File.ReadAllLines(PinnedAppsPath).ToList();
            /*set
            {
                var origStrings = value;
                var strings = new List<string>();

                foreach (string s in origStrings)
                {
                    bool add = true;
                    foreach (string t in origStrings)
                    {
                        if (Environment.ExpandEnvironmentVariables(t.ToLowerInvariant()) == Environment.ExpandEnvironmentVariables(s.ToLowerInvariant()))
                        {
                            add = false;
                            break;
                        }
                    }

                    if (add)
                    {
                        Debug.WriteLine("string: " + s);
                        strings.Add(s);
                    }
                }

                File.WriteAllLines(PinnedAppsPath, strings.ToArray());
            }*/
        }

        /*public static ObservableCollection<PinnedApplication> PinnedApps
        {
            get
            {
                ObservableCollection<PinnedApplication> apps = new ObservableCollection<PinnedApplication>();

                foreach (string s in File.ReadAllLines(_pinnedAppsPath))
                {
                    if (File.Exists(Environment.ExpandEnvironmentVariables(s)))
                        apps.Add(new PinnedApplication(new WindowsSharp.DiskItems.DiskItem(Environment.ExpandEnvironmentVariables(s)))
                        {
                            IsPinned = true
                        });
                }

                return apps;
            }
            set
            {
                List<string> strings = new List<string>();
                foreach (PinnedApplication a in value)
                {
                    strings.Add(a.DiskApplication.ItemPath);
                }

                File.WriteAllLines(_pinnedAppsPath, strings.ToArray());
            }
        }*/

        public enum CombineMode
        {
            Always,
            WhenFull,
            Never
        }

        public static CombineMode TaskbarCombineMode = CombineMode.Always;

        public static bool ShowCombinedLabels { get; set; } = false;


        public enum ClockDateMode
        {
            Auto,
            AlwaysShow,
            NeverShow
        }

        public static ClockDateMode TrayClockDateMode = ClockDateMode.Auto;


        public static SettingsWindow SettingsWindow = new SettingsWindow();

        public static event EventHandler<EventArgs> ConfigUpdated;

        internal static void InvokeConfigUpdated(object sender, EventArgs e)
        {
            ConfigUpdated?.Invoke(sender, e);
        }
    }
}

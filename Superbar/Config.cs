using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superbar
{
    public static class Config
    {
        static string _pinnedAppsPath = Environment.ExpandEnvironmentVariables(@"%appdata%\Start9\TempData\Superbar_PinnedApps.txt");

        public static ObservableCollection<PinnedApplication> PinnedApps
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
        }

        static Config()
        {
            if (!File.Exists(_pinnedAppsPath))
            {
                string dir = Path.GetDirectoryName(_pinnedAppsPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(_pinnedAppsPath, new List<string>()
                    {
                    }.ToArray());
            }
        }

        public static SettingsWindow SettingsWindow = new SettingsWindow();

        public enum CombineMode
        {
            Always,
            WhenFull,
            Never
        }

        public static CombineMode TaskbarCombineMode = CombineMode.Always;


        public enum ClockDateMode
        {
            Auto,
            AlwaysShow,
            NeverShow
        }

        public static ClockDateMode TrayClockDateMode = ClockDateMode.Auto;
    }
}

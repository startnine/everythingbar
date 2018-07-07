using Start9.Api.AppBar;
using Superbar.Views;
using System;
using System.AddIn;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Superbar
{
    [AddIn("Superbar", Description = "The Windows 7 taskbar you know and love", Version = "1.0.0.0", Publisher = "Start9")]
    public class SuperbarAddIn : IModule
    {
        public static SuperbarAddIn Instance { get; private set; }

        public IConfiguration Configuration { get; set; } = new SuperbarConfiguration();

        public IMessageContract MessageContract { get; } = new SuperbarMessageContract();

        public IReceiverContract ReceiverContract => null;

        public IHost Host { get; private set; }

        public void Initialize(IHost host)
        {   
            void Start()
            {
                Instance = this;
                Host = host;
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
                App.Main();
            }

            var t = new Thread(Start);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {

                MessageBox.Show(e.ExceptionObject.ToString(), "Uh oh E R R O R E");
            };
        }
    }

    public class SuperbarMessageContract : IMessageContract
    {
        public IList<IMessageEntry> Entries => new[] { StartClickedEntry, SearchClickedEntry, SearchInvokedEntry, TaskViewClickedEntry };

        public IMessageEntry StartClickedEntry { get; } = new MessageEntry(typeof(DBNull), "Start button clicked");

        public IMessageEntry SearchClickedEntry { get; } = new MessageEntry(typeof(DBNull), "Search button clicked");

        public IMessageEntry SearchInvokedEntry { get; } = new MessageEntry(typeof(String), "Search box text submitted");

        public IMessageEntry TaskViewClickedEntry { get; } = new MessageEntry(typeof(DBNull), "Task view button clicked");

    }

    public class SuperbarConfiguration : IConfiguration
    {
        public IList<IConfigurationEntry> Entries => new[] { new ConfigurationEntry(GroupItems, "Group Items") };

        public TaskbarGroupMode GroupItems { get; set; } = TaskbarGroupMode.GroupAlways;

        public Boolean ShowSearchBar { get; set; } = false;

        public Boolean Lock { get; set; } = false;

        public Boolean AutoHide { get; set; } = false;

        public AppBarDockMode DockPosition { get; set; } = AppBarDockMode.Bottom;

        public Boolean ShowStoreApps { get; set; } = true;

        public Boolean ShowOnAllDisplays { get; set; } = true;

        public Boolean ReplaceCmdWithPowershell { get; set; } = true;

        public List<Toolbar> Toolbars { get; set; } = new List<Toolbar>();

    }

    [Serializable]
    public class Toolbar
    {
        public String Name { get; set; }

    }

    public enum TaskbarGroupMode
    {
        GroupAlways,
        DontGroup,
        GroupWhenFull    
    }

}

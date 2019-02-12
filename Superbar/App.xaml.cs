//using Start9.WCF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace Everythingbar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public IModuleService service;

        public App()
        {
            /*try
            {
                var callback = new ModuleCallback();
                var context = new InstanceContext(callback);
                var pipeFactory =
                     new DuplexChannelFactory<IModuleService>(context,
                     new NetNamedPipeBinding(),
                     new EndpointAddress("net.pipe://localhost/Start9"));

                service = pipeFactory.CreateChannel();

                service.Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }*/
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Module.StartModule();
            //Module.SendMessage("")
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using MeteorInkjet;

namespace MeteorSwathTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e) {
            // We must sort out where the runtime Meteor components will be
            // loaded from before the main window loads.
            MeteorPath.LocatePrinterInterface();
        }
    }
}

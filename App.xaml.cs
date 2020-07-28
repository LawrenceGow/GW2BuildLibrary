using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Reference to the <see cref="GW2BuildLibrary.BuildLibrary"/> instance.
        /// </summary>
        public static BuildLibrary BuildLibrary
        { get; private set; } = null;

        /// <summary>
        /// Raises the System.Windows.Application.Startup event.
        /// </summary>
        /// <param name="e">A System.Windows.StartupEventArgs that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            BuildLibrary = new BuildLibrary();
            BuildLibrary.Load();
            base.OnStartup(e);
        }

        /// <summary>
        /// Raises the System.Windows.Application.Exit event.
        /// </summary>
        /// <param name="e">An System.Windows.ExitEventArgs that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            BuildLibrary.Save();
            base.OnExit(e);
        }
    }
}

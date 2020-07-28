using Mono.Options;
using System;
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
            bool overlayMode = false;
            OptionSet options = new OptionSet()
            {
                { "o|overlay", "Overlay Mode", s => overlayMode = true }
            };
            try
            {
                options.Parse(e.Args);
            }
            catch (OptionException ex)
            {
                Console.Write("Error: ");
                Console.WriteLine(ex.Message);
                return;
            }
            BuildLibrary = new BuildLibrary();
            Console.WriteLine(overlayMode.ToString());
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

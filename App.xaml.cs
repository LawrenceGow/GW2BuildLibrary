using Mono.Options;
using System;
using System.IO;
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

        private OptionSet options;

        /// <summary>
        /// Raises the System.Windows.Application.Startup event.
        /// </summary>
        /// <param name="e">A System.Windows.StartupEventArgs that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            options = new OptionSet()
            {
                // Overlay Mode
                {
                    "o|overlay",
                    "Overlay Mode",
                    v => OverlayMode = v != null
                },

                // Quick Mode
                {
                    "q|quick",
                    "Quick Mode",
                    v => QuickMode = v != null
                },

                //{ "x|export", "Export", v => { } },
                //{ "i|import", "Import", v => { } },

                // Profession Filter
                {
                    "p=|profession=",
                    "Profession Filter",
                    v => SetProfessionFilter(v)
                },

                // Help
                {
                    "h|?|help",
                    "Help",
                    v => ShowHelp = v != null
                },
            };

            try
            {
                options.Parse(e.Args);
#if DEBUG
                using (StringWriter text = new StringWriter())
                {
                    text.WriteLine($"Overlay Mode: {OverlayMode}");
                    text.WriteLine($"Quick Mode: {QuickMode}");
                    text.WriteLine($"Profession Filter: {ProfessionFilter}");
                    text.WriteLine($"Help: {ShowHelp}");

                    MessageBox.Show(text.ToString(),
                                    "DEBUG Options",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
#endif
            }
            catch (OptionException ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                                "Option Parsing Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }

            if (ShowHelp)
            {
                // Show the help message and exit
                ShowHelpMessage();
                Shutdown(0);
                return;
            }

            BuildLibrary = new BuildLibrary();
            BuildLibrary.Load();
            base.OnStartup(e);
        }

        /// <summary>
        /// Whether or not the application is in overlay mode.
        /// </summary>
        public static bool OverlayMode { get; private set; } = false;

        /// <summary>
        /// Whether or not he application is in quick mode.
        /// </summary>
        public static bool QuickMode { get; private set; } = false;

        /// <summary>
        /// The application profession filter.
        /// </summary>
        /// <remarks>
        /// If set then only the applicable build will be shown and the filter buttons will be hidden.
        /// </remarks>
        public static Profession ProfessionFilter { get; private set; } = Profession.None;

        /// <summary>
        /// Sets the profession filter to what was passed into the application.
        /// </summary>
        /// <param name="value">The value that was passed into the application.</param>
        private void SetProfessionFilter(string value)
        {
            if (Enum.TryParse(value, out Profession profession))
                ProfessionFilter = profession;
        }

        private bool ShowHelp { get; set; } = false;

        /// <summary>
        /// Shows the help message and provides the user with the option to open the README file.
        /// </summary>
        private void ShowHelpMessage()
        {
            string optionText;
            using (StringWriter writer = new StringWriter())
            {
                options.WriteOptionDescriptions(writer);
                optionText = writer.ToString();
            }

            MessageBox.Show($"USAGE\n{optionText}\n\n" +
                            $"Would you like to open the README file?",
                            "Help",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

            // TODO Open the README file
        }

        /// <summary>
        /// Raises the System.Windows.Application.Exit event.
        /// </summary>
        /// <param name="e">An System.Windows.ExitEventArgs that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            BuildLibrary?.Save();
            base.OnExit(e);
        }
    }
}

using Mono.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

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
            BuildLibrarySettings settings = new BuildLibrarySettings();
            string exportLocation = null,
                   importLocation = null;

            options = new OptionSet()
            {
                // Overlay Mode
                {
                    "o|overlay",
                    "Overlay Mode",
                    v => settings.OverlayMode = v != null
                },

                // Full Screen Mode
                {
                    "f|full-screen",
                    "Full Screen Mode",
                    v => settings.FullScreenMode = v != null
                },

                // Quick Mode
                {
                    "q|quick",
                    "Quick Mode",
                    v => settings.QuickMode = v != null
                },

                // No Save Window State
                {
                    "no-save-window-state",
                    "No Save Window State",
                    v => settings.SaveWindowState = v == null
                },

                // Profession Filter
                {
                    "profession=",
                    "Profession Filter",
                    v => settings.ProfessionFilter = ParseProfessionFilter(v)
                },

                // Export Builds
                {
                    "export=",
                    "Export",
                    v => exportLocation = v
                },

                // Import Builds
                {
                    "import=",
                    "Import",
                    v => importLocation = v
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

            BuildLibrary = new BuildLibrary(settings);

            bool shutdown = false;
            if (!string.IsNullOrEmpty(importLocation))
            {
                // Load the additional templates
                BuildLibrary.Load(importLocation, loadWindowState: false);
            }

            if (!string.IsNullOrEmpty(exportLocation))
            {
                // Save the library without window data to the specified location
                // then shutdown the application
                BuildLibrary.Save(exportLocation, saveWindowState: false);
                shutdown = true;
            }

            if (shutdown)
                Shutdown(0);
            else
                base.OnStartup(e);
        }

        /// <summary>
        /// Parses the profession filter from what was passed into the application.
        /// </summary>
        /// <param name="value">The value that was passed into the application.</param>
        private Profession ParseProfessionFilter(string value)
        {
            if (Enum.TryParse(value, out Profession profession))
                return profession;

            return Profession.None;
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

            if (MessageBox.Show($"USAGE\n{optionText}\n\n" +
                            $"Would you like to open the README file?",
                            "Help",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                // Open the README file
                Process.Start(Path.Combine(BaseDirectory, "README.pdf"));
            }
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

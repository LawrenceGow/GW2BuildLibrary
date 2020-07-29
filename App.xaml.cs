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
            bool overlayMode = false;
            bool quickMode = false;
            string exportLocation = null;
            Profession professionFilter = Profession.None;

            options = new OptionSet()
            {
                // Overlay Mode
                {
                    "o|overlay",
                    "Overlay Mode",
                    v => overlayMode = v != null
                },

                // Quick Mode
                {
                    "q|quick",
                    "Quick Mode",
                    v => quickMode = v != null
                },

                // Export Builds
                {
                    "export=",
                    "Export",
                    v => exportLocation = v
                },

                //{ "import", "Import", v => { } },

                // Profession Filter
                {
                    "profession=",
                    "Profession Filter",
                    v => professionFilter = ParseProfessionFilter(v)
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
                    text.WriteLine($"Overlay Mode: {overlayMode}");
                    text.WriteLine($"Quick Mode: {quickMode}");
                    text.WriteLine($"Profession Filter: {professionFilter}");
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

            BuildLibrary = new BuildLibrary(overlayMode,
                                            quickMode,
                                            professionFilter);

            if (!string.IsNullOrEmpty(exportLocation))
            {
                // Save the library without window data to the specified location
                // then shutdown the application
                BuildLibrary.Save(exportLocation, false);
                Shutdown(0);
                return;
            }

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

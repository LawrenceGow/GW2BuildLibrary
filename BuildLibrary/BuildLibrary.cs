using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.XPath;

namespace GW2BuildLibrary
{
    /// <summary>
    /// The build library. Holds information on all templates in the library and provides methods to interface with them.
    /// </summary>
    public class BuildLibrary
    {
        #region Fields

        /// <summary>
        /// The current version of the XML file.
        /// </summary>
        private const int XmlFileVersion = 1;

        /// <summary>
        /// The default save location.
        /// </summary>
        private readonly string DefaultSaveLocation = Path.Combine(App.BaseDirectory, "BuildLibrary.xml");

        /// <summary>
        /// The state of the window containing the build library
        /// </summary>
        private WindowState windowState = WindowState.Normal;

        /// <summary>
        /// The settings for the <see cref="BuildLibrary"/>.
        /// </summary>
        public readonly BuildLibrarySettings Settings;

        /// <summary>
        /// The height of the window containing the build library.
        /// </summary>
        public double Height = 0;

        /// <summary>
        /// The left offset of the window containing the build library.
        /// </summary>
        public double Left = 0;

        /// <summary>
        /// The top offset of the window containing the build library.
        /// </summary>
        public double Top = 0;

        /// <summary>
        /// The width of the window containing the build library.
        /// </summary>
        public double Width = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialises a new <see cref="BuildLibrary"/> instance.
        /// </summary>
        /// <param name="settings">The settings for this <see cref="BuildLibrary"/> instance.</param>
        public BuildLibrary(BuildLibrarySettings settings)
        {
            Settings = settings;

            // Load the library
            Load();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Map of all the build templates in the library.
        /// </summary>
        private Dictionary<int, BuildTemplate> BuildTemplates
        { get; set; } = new Dictionary<int, BuildTemplate>();

        /// <summary>
        /// The window state.
        /// </summary>
        public WindowState WindowState
        {
            get => Settings.FullScreenMode ? WindowState.Maximized : windowState;
            set => windowState = value;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new build template.
        /// </summary>
        /// <param name="index">The new templates index.</param>
        /// <param name="data">The build template data.</param>
        public void CreateBuildTemplate(in int index, in string data)
        {
            BuildTemplate build = new BuildTemplate();
            if (build.SetBuildData(index, data))
            {
                BuildTemplates[build.Index] = build;
            }
        }

        /// <summary>
        /// Deletes the build template.
        /// </summary>
        /// <param name="index">The index.</param>
        public void DeleteBuildTemplate(in int index) =>
            BuildTemplates.Remove(index);

        /// <summary>
        /// Gets all the build templates for the provided profession.
        /// </summary>
        /// <param name="professionFilter">The profession.</param>
        /// <returns></returns>
        public IEnumerable<BuildTemplate> GetAllBuildTemplates(Profession professionFilter)
        {
            return BuildTemplates.Values
                .Where(b => b.Profession.IsBasedOn(professionFilter)
                            || b.Profession == professionFilter);
        }

        /// <summary>
        /// Gets the build template at the specified location.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The build template that was at the index, or null.</returns>
        public BuildTemplate GetBuildTemplate(in int index)
        {
            BuildTemplates.TryGetValue(index, out BuildTemplate build);
            return build;
        }

        /// <summary>
        /// Get the next free index available for storage.
        /// </summary>
        /// <returns>The next free index.</returns>
        public int GetNextFreeIndex() =>
            BuildTemplates.Values.Max(bt => bt.Index) + 1;

        /// <summary>
        /// Loads the build library.
        /// </summary>
        public void Load() =>
            Load(DefaultSaveLocation, loadWindowState: true);

        /// <summary>
        /// Loads the build library.
        /// </summary>
        /// <param name="location">The location to load from.</param>
        /// <param name="loadWindowState"><c>True</c> to also load the window state, otherwise <c>false</c>.</param>
        public void Load(string location, bool loadWindowState)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(location))
                {
                    XPathDocument doc = new XPathDocument(reader);
                    XPathNavigator nav = doc.CreateNavigator();
                    nav = nav.SelectSingleNode(nameof(BuildTemplates));
                    if (!int.TryParse(nav.GetAttribute("XmlFileVersion", string.Empty), out int xmlFileVersion))
                        xmlFileVersion = 1;

                    if (loadWindowState)
                    {
                        if (Enum.TryParse(nav.GetAttribute("WindowState", string.Empty), out WindowState windowState))
                            WindowState = windowState;
                        if (double.TryParse(nav.GetAttribute("Width", string.Empty), out double d))
                            Width = d;
                        if (double.TryParse(nav.GetAttribute("Height", string.Empty), out d))
                            Height = d;
                        if (double.TryParse(nav.GetAttribute("Left", string.Empty), out d))
                            Left = d;
                        if (double.TryParse(nav.GetAttribute("Top", string.Empty), out d))
                            Top = d;
                    }

                    foreach (XPathNavigator node in nav.Select(BuildTemplate.XmlNodeName))
                    {
                        if (node != null)
                        {
                            BuildTemplate build = new BuildTemplate();
                            build.Load(node);
                            if (!build.IsValid)
                            {
                                // Avoid any possible conflicts by moving the build to the next available index
                                while (BuildTemplates.ContainsKey(build.Index))
                                    build.Index++;
                                BuildTemplates[build.Index] = build;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Saves the build library.
        /// </summary>
        public void Save() =>
            Save(DefaultSaveLocation, saveWindowState: true);

        /// <summary>
        /// Saves the build library.
        /// </summary>
        /// <param name="location">The location to save to.</param>
        /// <param name="saveWindowState"><c>True</c> to also save the window state, otherwise <c>false</c>.</param>
        public void Save(string location, bool saveWindowState)
        {
            try
            {
                File.Delete(location);
                using (XmlWriter writer = XmlWriter.Create(location,
                                                           new XmlWriterSettings()
                                                           { Indent = true, NewLineChars = "\n" }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(nameof(BuildTemplates));
                    writer.WriteAttributeString("XmlFileVersion", XmlFileVersion.ToString());

                    if (saveWindowState)
                    {
                        writer.WriteAttributeString("WindowState", WindowState.ToString());
                        writer.WriteAttributeString("Width", Width.ToString());
                        writer.WriteAttributeString("Height", Height.ToString());
                        writer.WriteAttributeString("Left", Left.ToString());
                        writer.WriteAttributeString("Top", Top.ToString());
                    }

                    foreach (BuildTemplate build in BuildTemplates.Values)
                    {
                        build.Save(writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Sets the name of the build template at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="name">The new name of the template.</param>
        public void SetBuildTemplateName(in int index, in string name) =>
            GetBuildTemplate(index)?.SetName(name);

        /// <summary>
        /// Updates the current state of the window, so that it can be saved in the XML.
        /// </summary>
        /// <param name="windowState">The current window state.</param>
        /// <param name="size">The current size of the window.</param>
        /// <param name="left">The pixels left of the window.</param>
        /// <param name="top">The pixel above the window.</param>
        public void UpdateWindowStateForSaving(in WindowState windowState,
                                               in Size size,
                                               in double left,
                                               in double top)
        {
            if (Settings.SaveWindowState)
            {
                WindowState = windowState;
                Width = size.Width;
                Height = size.Height;
                Left = left;
                Top = top;
            }
        }

        #endregion Methods
    }
}
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
    /// The build library.
    /// Holds information on all templates in the library and provides methods to interface with them.
    /// </summary>
    public class BuildLibrary
    {
        private readonly string saveLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BuildLibrary.xml");

        /// <summary>
        /// Map of all the build templates in the library.
        /// </summary>
        private Dictionary<ulong, BuildTemplate> BuildTemplates
        { get; set; } = new Dictionary<ulong, BuildTemplate>();

        /// <summary>
        /// Loads the build library.
        /// </summary>
        public void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(saveLocation))
                {
                    XPathDocument doc = new XPathDocument(reader);
                    XPathNavigator nav = doc.CreateNavigator();
                    nav = nav.SelectSingleNode(nameof(BuildTemplates));
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
                    foreach (XPathNavigator node in nav.Select(BuildTemplate.XmlNodeName))
                    {
                        if (node != null)
                        {
                            BuildTemplate build = new BuildTemplate();
                            build.Load(node);
                            if (!build.IsValid)
                                BuildTemplates[build.Id] = build;
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
            Save(saveLocation, saveWindowState: true);

        /// <summary>
        /// Saves the build library.
        /// </summary>
        /// <param name="location">The location to save to.</param>
        /// <param name="saveWindowState"><c>True</c> to also save the window state, otherwise <c>false</c>.</param>
        public void Save(string location, bool saveWindowState)
        {
#if DEBUG
            if (MessageBox.Show($"Save to {location}?",
                                "Save?",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question,
                                MessageBoxResult.Yes) != MessageBoxResult.Yes)
                return;
#endif
            try
            {
                File.Delete(location);
                using (XmlWriter writer = XmlWriter.Create(location,
                                                           new XmlWriterSettings()
                                                           { Indent = true, NewLineChars = "\n" }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(nameof(BuildTemplates));

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

        public WindowState WindowState = WindowState.Normal;
        public double Width = 0, Height = 0, Left = 0, Top = 0;

        /// <summary>
        /// Updates the current state of the window, so that it can be saved in the XML.
        /// </summary>
        /// <param name="windowState">The current window state.</param>
        /// <param name="size">The current size of the window.</param>
        /// <param name="left">The pixels left of the window.</param>
        /// <param name="top">The pixel above the window.</param>
        public void UpdateWindowState(in WindowState windowState, in Size size, in double left, in double top)
        {
            WindowState = windowState;
            Width = size.Width;
            Height = size.Height;
            Left = left;
            Top = top;
        }

        /// <summary>
        /// Creates a new build template.
        /// </summary>
        /// <param name="page">The new templates page.</param>
        /// <param name="index">The new templates index.</param>
        /// <param name="data">The build template data.</param>
        public void CreateBuildTemplate(in int page, in int index, in string data)
        {
            BuildTemplate build = new BuildTemplate();
            if (build.SetBuildData(page, index, data))
            {
                BuildTemplates[build.Id] = build;
            }
        }

        /// <summary>
        /// Deletes the build template.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="index">The index.</param>
        public void DeleteBuildTemplate(in int page, in int index)
        {
            BuildTemplates.Remove(BuildTemplate.GetId(page, index));
        }

        /// <summary>
        /// Gets the build template at the specified location.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="index">The index.</param>
        /// <returns>The build template that was at the index, or null.</returns>
        public BuildTemplate GetBuildTemplate(in int page, in int index)
        {
            BuildTemplates.TryGetValue(BuildTemplate.GetId(page, index), out BuildTemplate build);
            return build;
        }

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
        /// Sets the name of the build template at the specified index.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="index">The index.</param>
        /// <param name="name">The new name of the template.</param>
        public void SetBuildTemplateName(in int page, in int index, in string name)
        {
            GetBuildTemplate(page, index)?.SetName(name);
        }
    }
}

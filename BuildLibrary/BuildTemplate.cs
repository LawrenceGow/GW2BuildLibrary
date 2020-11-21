using System;
using System.Xml;
using System.Xml.XPath;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Represents a build template in the library.
    /// </summary>
    public class BuildTemplate
    {
        /// <summary>
        /// The name of this element in XML.
        /// </summary>
        public const string XmlNodeName = "BuildTemplate";

        #region Properties

        /// <summary>
        /// The index of the slot the build is in.
        /// </summary>
        public int Index
        { get; set; } = -1;

        /// <summary>
        /// The name of the build.
        /// </summary>
        public string Name
        { get; protected set; } = string.Empty;

        /// <summary>
        /// The raw build data string.
        /// </summary>
        public string BuildData
        { get; protected set; } = string.Empty;

        /// <summary>
        /// The profession the build is for.
        /// </summary>
        public Profession Profession
        { get; set; } = Profession.None;

        /// <summary>
        /// The first specialization slot.
        /// </summary>
        public readonly SpecializationSlot Slot1 = new SpecializationSlot();

        /// <summary>
        /// The second specialization slot.
        /// </summary>
        public readonly SpecializationSlot Slot2 = new SpecializationSlot();

        /// <summary>
        /// The third specialization slot.
        /// </summary>
        public readonly SpecializationSlot Slot3 = new SpecializationSlot();

        #endregion

        /// <summary>
        /// Loads the build from the provided node.
        /// </summary>
        /// <param name="node">The node to load from.</param>
        public void Load(XPathNavigator node)
        {
            if (int.TryParse(node.GetAttribute("Index", string.Empty), out int index))
            {
                SetName(node.GetAttribute("Name", string.Empty));
                SetBuildData(index, node.GetAttribute("BuildData", string.Empty));
            }
        }

        /// <summary>
        /// Saves this build using the provided <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to save through.</param>
        public void Save(XmlWriter writer)
        {
            if (!IsValid)
            {
                writer.WriteStartElement(XmlNodeName);
                writer.WriteAttributeString("Index", Index.ToString());
                if (!string.IsNullOrEmpty(Name))
                    writer.WriteAttributeString("Name", Name);
                writer.WriteAttributeString("BuildData", BuildData);
                writer.WriteEndElement(); // End BuildTemplate
            }
        }

        /// <summary>
        /// Fired when the build has been updated in some way.
        /// </summary>
        public EventHandler Updated;

        /// <summary>
        /// Set the name of the build.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public void SetName(in string name)
        {
            if (name != Name)
            {
                Name = name;
                Updated?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Sets the raw build data.
        /// </summary>
        /// <param name="index">The index of the slot the build is in.</param>
        /// <param name="data">The raw build data.</param>
        /// <returns>
        /// <c>True</c> if the data was considered valid and has been set, otherwise <c>false</c>.
        /// </returns>
        public bool SetBuildData(in int index, in string data)
        {
            if (string.IsNullOrEmpty(data) || !data.StartsWith("[&") || !data.EndsWith("]"))
                return false;

            if (data != BuildData)
            {
                try
                {
                    byte[] raw = Convert.FromBase64String(data.Substring(2, data.Length - 3));
                    // For how the chat-link is built
                    // See: https://wiki.guildwars2.com/wiki/Chat_link_format#Build_templates_link
                    // Build templates are always 44 bytes long
                    if (raw.Length == 44
                        // Build templates have a link type of 13
                        && raw[0] == 13)
                    {
                        BuildData = data;
                        Index = index;

                        Slot1.LoadFromBytes(raw[2], raw[3]);
                        Slot2.LoadFromBytes(raw[4], raw[5]);
                        Slot3.LoadFromBytes(raw[6], raw[7]);

                        Profession = TemplateHelper.GetProfessionFromBytes(raw[1], Slot3.Specialization);

                        Updated?.Invoke(this, null);
                        return true;
                    }
                }
                catch (Exception)
                {
                    Index = -1;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the build is in a valid state.
        /// </summary>
        public bool IsValid
        {
            get { return Index < 0 | string.IsNullOrEmpty(BuildData); }
        }
    }
}
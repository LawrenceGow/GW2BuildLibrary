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
        /// The specialization in the first slot.
        /// </summary>
        public Specialization Specialization1
        { get; set; }

        public byte[] Traits1
        { get; set; } = new byte[3];

        /// <summary>
        /// The specialization in the second slot.
        /// </summary>
        public Specialization Specialization2
        { get; set; }

        /// <summary>
        /// The specialization in the third slot.
        /// </summary>
        public Specialization Specialization3
        { get; set; }

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

                        // Check https://wiki.guildwars2.com/wiki/Chat_link_format#Build_templates_link
                        // raw[2], raw[4], and raw[6] are the specializations
                        // raw[3], raw[5], and raw[7] are the trait choices

                        Specialization1 = (Specialization)raw[2];
                        Traits1[0] = (byte)(raw[3] & 0b00000011);
                        Traits1[1] = (byte)((raw[3] & 0b00001100) >> 2);
                        Traits1[2] = (byte)((raw[3] & 0b00110000) >> 4);

                        Specialization2 = (Specialization)raw[4];
                        Specialization3 = (Specialization)raw[6];

                        Profession = TemplateHelper.GetProfessionFromBytes(raw[1], TemplateHelper.GetSetByte(Specialization3));

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
using System;
using System.Xml;
using System.Xml.XPath;

namespace GW2BuildLibrary
{
    public class BuildTemplate
    {
        public const string XmlNodeName = "BuildTemplate";

        public int Page
        { get; set; } = -1;

        public int Index
        { get; set; } = -1;

        public ulong Id
        { get { return GetId(Page, Index); } }

        public string Name
        { get; protected set; } = string.Empty;

        public string BuildData
        { get; protected set; } = string.Empty;

        public Profession Profession
        { get; set; } = Profession.None;

        /// <summary>
        /// Gets a build template id from the given page and index.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static ulong GetId(in int page, in int index)
        {
            return (uint)((page << 16) | (int)index);
        }

        public void Load(XPathNavigator node)
        {
            if (int.TryParse(node.GetAttribute("Page", string.Empty), out int page) &&
                int.TryParse(node.GetAttribute("Index", string.Empty), out int index))
            {
                SetName(node.GetAttribute("Name", string.Empty));
                SetBuildData(page, index, node.GetAttribute("BuildData", string.Empty));
            }
        }

        public void Save(XmlWriter writer)
        {
            if (!IsValid)
            {
                writer.WriteStartElement(XmlNodeName);
                writer.WriteAttributeString("Page", Page.ToString());
                writer.WriteAttributeString("Index", Index.ToString());
                if (!string.IsNullOrEmpty(Name))
                    writer.WriteAttributeString("Name", Name);
                writer.WriteAttributeString("BuildData", BuildData);
                writer.WriteEndElement(); // End BuildTemplate
            }
        }

        public EventHandler Updated;

        public void SetName(in string name)
        {
            if (name != Name)
            {
                Name = name;
                Updated?.Invoke(this, null);
            }
        }

        public bool SetBuildData(in int page, in int index, in string data)
        {
            if (string.IsNullOrEmpty(data) || !data.StartsWith("[&") || !data.EndsWith("]"))
                return false;

            if (data != BuildData)
            {
                try
                {
                    byte[] raw = Convert.FromBase64String(data.Substring(2, data.Length - 3));
                    // Build templates are always 44 bytes long
                    // Build templates have a link type of 13
                    if (raw.Length == 44 && raw[0] == 13)
                    {
                        BuildData = data;
                        Page = page;
                        Index = index;
                        Profession = ProfessionHelper.FromBytes(raw[1], ProfessionHelper.GetSetByte(raw[6]));

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

        public bool IsValid
        {
            get { return Page < 0 || Index < 0 | string.IsNullOrEmpty(BuildData); }
        }
    }
}
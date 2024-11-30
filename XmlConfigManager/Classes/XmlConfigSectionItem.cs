using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigSectionItem : XmlConfigNameValueItem
    {
        internal static string identity = "section";

        public string Type { get; set; } = string.Empty;

        public XmlConfigSectionItem(XmlConfigParent cfgParent, XElement xDefinition, XElement xInstance) 
        {
            xmlDefinition = xDefinition;
            xmlInstance = xInstance;
            Parent = cfgParent;

            Name = xDefinition.Attribute("name").Value;
            Type = xDefinition.Attribute("type").Value;
        }

        public XmlConfigSectionItem(XmlConfigParent cfgParent, string sectionName)
        {
            Name = sectionName;
            Type = "System.Configuration.NameValueSection";
            Parent = cfgParent;

            xmlDefinition = new XElement(identity, new XAttribute("name", Name), new XAttribute("type", Type));

            cfgParent.xmlDefinition.Add(xmlDefinition);

            xmlInstance = new XElement(Name);

            cfgParent.xmlInstance.Add(xmlInstance);     
        }
    }
}

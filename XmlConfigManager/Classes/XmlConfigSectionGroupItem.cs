using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigSectionGroupItem : XmlConfigParent
    {
        internal static string identity = "sectionGroup";

        public XmlConfigSectionGroupItem(XmlConfigParent cfgParent, XElement xDefinition, XElement xInstance)
        {
            Parent = cfgParent;
            xmlDefinition = xDefinition;            
            xmlInstance = xInstance;

            Name = xmlDefinition.Attribute("name").Value;
        }

        public XmlConfigSectionGroupItem(XmlConfigParent cfgParent, string groupName)
        {
            Parent = cfgParent;
            Name = groupName;

            xmlDefinition = new XElement(identity, new XAttribute("name", Name));

            Parent.xmlDefinition.Add(xmlDefinition);

            xmlInstance = new XElement(Name);

            Parent.xmlInstance.Add(xmlInstance);
        }
    }
}

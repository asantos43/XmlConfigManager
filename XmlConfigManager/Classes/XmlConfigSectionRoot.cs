using System.Linq;
using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigSectionRoot : XmlConfigParent
    {
        internal static string identity = "configSections";

        public XmlConfigSectionRoot(XDocument xDocument)
        {
            var cs = xDocument.Root.Elements(identity).FirstOrDefault();
            
            Name = identity;

            if (cs != null) 
            {
                xmlDefinition = cs;
                xmlInstance = xDocument.Root;
            }
            else
            {
                xmlDefinition = new XElement(identity);

                xmlInstance = xDocument.Root;

                xmlInstance.Add(xmlDefinition);
            }
        }
    }
}

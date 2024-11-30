using System.Linq;
using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class AppSettingsSectionItem : XmlConfigNameValueItem
    {
        internal static string identity = "appSettings";

        public AppSettingsSectionItem(XDocument xDocument)
        {
            var cs = xDocument.Root.Elements(identity).FirstOrDefault();

            Name = identity;
            
            Parent = null;

            if (cs != null)
            {
                xmlDefinition = cs;
                xmlInstance = cs;
            }
            else
            {
                xmlDefinition = new XElement(identity);

                xmlInstance = xmlDefinition;

                xDocument.Root.Add(xmlDefinition);
            }
        }
    }
}

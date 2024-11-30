using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigItem
    {
        internal XElement xmlDefinition;

        internal XElement xmlInstance;

        public string Name { get; set; } = string.Empty;

        public XmlConfigParent Parent { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

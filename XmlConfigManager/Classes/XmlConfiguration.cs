using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfiguration
    {
        private readonly XDocument xmlConfig;
        private readonly string filePath;
        public AppSettingsSectionItem AppSettings { get; }
        public XmlConfigSectionRoot ConfigSections { get; }

        public XmlConfiguration(string filePath)
        {
            this.filePath = filePath;

            xmlConfig = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);                        

            ConfigSections = new XmlConfigSectionRoot(xmlConfig);
            AppSettings = new AppSettingsSectionItem(xmlConfig);
        }

        public void Save()
        {
            XDocument xdoc = XDocument.Parse(xmlConfig.ToString());

            xdoc.Save(filePath, SaveOptions.None);
        }

        public void SaveAs(string filePath)
        {
            XDocument xdoc = XDocument.Parse(xmlConfig.ToString());

            xdoc.Save(filePath, SaveOptions.None);
        }
    }
}

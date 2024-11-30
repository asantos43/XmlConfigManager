﻿using System.Configuration;
using System.Reflection;
using XmlConfigManager.Classes;

namespace XmlConfigFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = Assembly.GetExecutingAssembly().Location;

            Configuration configFile = ConfigurationManager.OpenExeConfiguration(fileName);

            XmlConfiguration xmlConfig = new XmlConfiguration(configFile.FilePath);

            XmlConfigSectionRoot configSections = xmlConfig.ConfigSections;
            AppSettingsSectionItem appSettings = xmlConfig.AppSettings;

            var section1 = configSections.AddSection("group1/section1");
            var section2 = configSections.AddSection("group1/section2");

            var section3 = configSections.AddSection("group2/section1");
            var section4 = configSections.AddSection("group2/section2");

            section1["item3"] = "value2";
            section1["item4"] = "true";

            section1.Remove("item5");
            section1.Remove("item6");

            section2["item5"] = "value3";
            section2["item6"] = "true";

            section3["item3"] = "value2";
            section3["item4"] = "true";

            section4["item5"] = "value3";
            section4["item6"] = "true";

            appSettings["item1"] = "value1";
            appSettings["item2"] = "true";

            xmlConfig.Save();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigParent : XmlConfigItem
    {
        internal Dictionary<string, XmlConfigSectionGroupItem> sectionGroups = null;

        internal Dictionary<string, XmlConfigSectionItem> sections = null;

        public void Clear()
        {
            xmlDefinition.RemoveNodes();
            xmlInstance.RemoveNodes();

            sectionGroups?.Clear();
            sections?.Clear();
        }

        public void Clear(string childPath)
        {
            if (!string.IsNullOrEmpty(childPath))
            {
                var split = childPath.Split('/');

                XmlConfigParent current = this;

                for (int i = 0; i < split.Length; i++)
                {
                    if (i == split.Length - 1)
                    {
                        if (current.SectionGroups().TryGetValue(split[i], out XmlConfigSectionGroupItem sectionGroup))
                        {
                            sectionGroup.Clear();
                        }
                        else if (current.Sections().TryGetValue(split[i], out XmlConfigSectionItem section))
                        {
                            section.Clear();
                        }
                    }
                    else
                    {
                        if (!current.SectionGroups().ContainsKey(split[i]))
                        {
                            break;
                        }

                        current = current.SectionGroups()[split[i]];
                    }
                }
            }
        }

        public XmlConfigSectionItem AddSection(string sectionPath)
        {
            var split = sectionPath.Split('/');

            XmlConfigParent current = this;

            XmlConfigSectionItem result = null;

            for (int i = 0; i < split.Length; i++)
            {
                if (string.IsNullOrEmpty(split[i]))
                {
                    return null;
                }

                if (i == split.Length - 1)
                {
                    if (!current.Sections().TryGetValue(split[i], out XmlConfigSectionItem section))
                    {
                        section = new XmlConfigSectionItem(current, split[i]);

                        current.AddSection(section);

                        return section;
                    }

                    return section;
                }

                if (current.SectionGroups().ContainsKey(split[i]))
                {
                    current = current.SectionGroups()[split[i]];
                }              
                else
                {
                    var newSectionGroup = new XmlConfigSectionGroupItem(current, split[i]);

                    current.AddSectionGroup(newSectionGroup);

                    current = newSectionGroup;
                }
            }

            return result;
        }

        public XmlConfigSectionGroupItem AddSectionGroup(string sectionGroupPath)
        {
            var split = sectionGroupPath.Split('/');

            XmlConfigParent current = this;

            XmlConfigSectionGroupItem result = null;

            for (int i = 0; i < split.Length; i++)
            {
                if (string.IsNullOrEmpty(split[i]))
                {
                    return null;
                }

                if (current.SectionGroups().ContainsKey(split[i]))
                {
                    result = current.SectionGroups()[split[i]];

                    current = result;
                }
                else
                {
                    result = new XmlConfigSectionGroupItem(current, split[i]);

                    current.AddSectionGroup(result);

                    current = result;
                }
            }

            return result;
        }

        public void AddSection(XmlConfigSectionItem section)
        {
            if (sections == null) Sections();

            sections.Add(section.Name, section);
        }

        public void AddSectionGroup(XmlConfigSectionGroupItem sectionGroup)
        {
            if (sectionGroups == null) SectionGroups();

            sectionGroups.Add(sectionGroup.Name, sectionGroup);
        }

        public bool RemoveSection(string sectionPath)
        {
            var split = sectionPath.Split('/');

            XmlConfigParent current = this;

            for (int i = 0; i < split.Length; i++)
            {
                if (i == split.Length - 1)
                {
                    if (current.Sections().TryGetValue(split[i], out XmlConfigSectionItem section))
                    {
                        current.Sections().Remove(section.Name);

                        section.xmlDefinition.Remove();
                        section.xmlInstance.Remove();

                        return true;
                    }

                    break;
                }
                else if (!current.SectionGroups().ContainsKey(split[i]))
                {
                    break;
                }

                current = current.SectionGroups()[split[i]];
            }

            return false;
        }

        public bool RemoveSectionGroup(string sectionGroupPath)
        {
            var split = sectionGroupPath.Split('/');

            XmlConfigParent current = this;

            for (int i = 0; i < split.Length; i++)
            {
                if (!current.SectionGroups().ContainsKey(split[i]))
                {
                    break;
                }

                current = current.SectionGroups()[split[i]];

                if (i == split.Length - 1)
                {
                    var sectionGroup = (XmlConfigSectionGroupItem)current;

                    sectionGroup.Parent.SectionGroups().Remove(sectionGroup.Name);

                    sectionGroup.xmlDefinition.Remove();
                    sectionGroup.xmlInstance.Remove();

                    return true;
                }
            }

            return false;
        }

        public XmlConfigSectionItem Section(string sectionPath)
        {
            var split = sectionPath.Split('/');

            XmlConfigParent current = this;

            for (int i = 0; i < split.Length; i++)
            {
                if (i == split.Length - 1)
                {
                    if(current.Sections().TryGetValue(split[i], out XmlConfigSectionItem section))
                    {
                        return section;
                    }                                        
                }
                else
                {
                    if (!current.SectionGroups().ContainsKey(split[i]))
                    {
                        return null;
                    }

                    current = current.SectionGroups()[split[i]];
                }
            }
            
            return null;
        }

        public XmlConfigSectionGroupItem SectionGroup(string sectionGroupPath)
        {
            var split = sectionGroupPath.Split('/');

            XmlConfigParent current = this;

            XmlConfigSectionGroupItem result = null;

            for (int i = 0; i < split.Length; i++)
            {
                if (!current.SectionGroups().ContainsKey(split[i]))
                {
                    break;
                }

                current = current.SectionGroups()[split[i]];

                if (i == split.Length - 1)
                {
                    result = (XmlConfigSectionGroupItem)current;
                }
            }

            return result;
        }

        public Dictionary<string, XmlConfigSectionItem> Sections()
        {
            if ((sections != null))
            {
                return sections;
            }

            sections = new Dictionary<string, XmlConfigSectionItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var definition in xmlDefinition.Elements())
            {
                if (definition.Name.ToString().Equals(XmlConfigSectionItem.identity, StringComparison.OrdinalIgnoreCase))
                {
                    string name = definition.Attribute("name").Value;

                    var instance = xmlInstance.Elements().FirstOrDefault(e => e.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (instance != null)
                    {
                        XmlConfigSectionItem section = new XmlConfigSectionItem(this, definition, instance);

                        sections.Add(section.Name, section);
                    }
                }
            }

            return sections;
        }

        public Dictionary<string, XmlConfigSectionItem> Sections(string sectionGroupPath)
        {
            var split = sectionGroupPath.Split('/');

            XmlConfigParent current = this;

            Dictionary<string, XmlConfigSectionItem> result = new Dictionary<string, XmlConfigSectionItem>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < split.Length; i++)
            {
                if (!current.SectionGroups().ContainsKey(split[i]))
                {
                    break;
                }
                
                current = current.SectionGroups()[split[i]];

                if (i == split.Length - 1)
                {
                    result = current.Sections();
                }
            }

            return result;
        }

        public Dictionary<string, XmlConfigSectionGroupItem> SectionGroups()
        {
            if ((sectionGroups != null))
            {
                return sectionGroups;
            }

            sectionGroups = new Dictionary<string, XmlConfigSectionGroupItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var definition in xmlDefinition.Elements())
            {
                if (definition.Name.ToString().Equals(XmlConfigSectionGroupItem.identity, StringComparison.OrdinalIgnoreCase))
                {
                    string name = definition.Attribute("name").Value;

                    var instance = xmlInstance.Elements().FirstOrDefault(e => e.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (instance != null)
                    {
                        XmlConfigSectionGroupItem sectionGroup = new XmlConfigSectionGroupItem(this, definition, instance);

                        sectionGroups.Add(sectionGroup.Name, sectionGroup);
                    }
                }
            }

            return sectionGroups;
        }

        public Dictionary<string, XmlConfigSectionGroupItem> SectionGroups(string sectionGroupPath)
        {
            var split = sectionGroupPath.Split('/');

            XmlConfigParent current = this;

            Dictionary<string, XmlConfigSectionGroupItem> result = new Dictionary<string, XmlConfigSectionGroupItem>();

            for (int i = 0; i < split.Length; i++)
            {
                if (!current.SectionGroups().ContainsKey(split[i]))
                {
                    break;
                }
                
                current = current.SectionGroups()[split[i]];

                if (i == split.Length - 1)
                {
                    result = current.SectionGroups();
                }
            }

            return result;
        }
    }
}

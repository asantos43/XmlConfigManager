using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmlConfigManager.Classes
{
    public class XmlConfigNameValueItem : XmlConfigItem
    {
        private object ParseStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            else if (int.TryParse(str, out int parseInt))
            {
                return parseInt;
            }
            else if (double.TryParse(str, out double parseDouble))
            {
                return parseDouble;
            }
            else if (TimeSpan.TryParse(str, out TimeSpan parseTimeSpan))
            {
                return parseTimeSpan;
            }
            else if (bool.TryParse(str, out bool parseBool))
            {
                return parseBool;
            }
            else if (Enum.TryParse<DayOfWeek>(str, true, out DayOfWeek parsedDay))
            {
                return parsedDay;
            }
            else if (DateTime.TryParse(str, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }

            return str;
        }

        public string[] AllKeys 
        { 
            get 
            { 
                return xmlInstance.Elements("add").Select(x => x.Attribute("key").Value).ToArray(); 
            } 
        }

        public string this[string key]
        {
            get
            {
                XElement appSetting = xmlInstance.Elements("add").FirstOrDefault(x => x.Attribute("key").Value == key);

                return appSetting.Attribute("value").Value;
            }
            set
            {
                XElement appSetting = xmlInstance.Elements("add").FirstOrDefault(x => x.Attribute("key").Value == key);

                if (appSetting == null)
                {
                    //Create the new appSetting
                    xmlInstance.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", value)));
                }
                else
                {
                    //Update the current appSetting
                    appSetting.Attribute("value").Value = value;
                }
            }
        }

        public bool Contains(string key)
        {
            XElement appSetting = xmlInstance.Elements("add").FirstOrDefault(x => x.Attribute("key").Value == key);

            return appSetting != null;
        }

        public string Add(string key, string value)
        {
            xmlInstance.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", value)));

            return value;
        }

        public bool Remove(string key)
        {
            XElement appSetting = xmlInstance.Elements("add").FirstOrDefault(x => x.Attribute("key").Value == key);

            if (appSetting != null)
            {
                appSetting.Remove();

                return true;
            }

            return false;
        }

        public void LoadFromDic(ConcurrentDictionary<string, object> dic)
        {
            foreach (var item in dic)
            {
                if (Contains(item.Key))
                {
                    this[item.Key] = item.Value != null ? item.Value.ToString() : string.Empty;
                }
                else
                {
                    Add(item.Key, item.Value != null ? item.Value.ToString() : string.Empty);
                }
            }
        }

        public void LoadFronDic(ConcurrentDictionary<string, string> dic)
        {
            foreach (var item in dic)
            {
                if (Contains(item.Key))
                {
                    this[item.Key] = item.Value;
                }
                else
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public void LoadFromDic(ConcurrentDictionary<string, List<string>> dic, string separator = "|")
        {
            foreach (var item in dic)
            {
                if (Contains(item.Key))
                {
                    this[item.Key] = string.Join(separator, item.Value);
                }
                else
                {
                    Add(item.Key, string.Join(separator, item.Value));
                }
            }
        }

        public ConcurrentDictionary<string, List<string>> GetDic(char separator = '|')
        {
            ConcurrentDictionary<string, List<string>> result = new ConcurrentDictionary<string, List<string>>();

            foreach (var item in AllKeys)
            {
                string value = this[item];

                if (!string.IsNullOrEmpty(value))
                {
                    result[item] = value.Split(separator).ToList();
                }
            }

            return result;
        }

        public Dictionary<string, object> GetObjectDic(char separator = '|')
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var item in AllKeys)
            {
                string value = this[item];

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains(separator))
                    {
                        var valueList = value.Split(separator);

                        List<object> list = new List<object>();

                        foreach (var str in valueList) 
                        {
                            list.Add(ParseStr(str));
                        }

                        result[item] = list;    
                    }
                    else
                    {
                        result[item] = ParseStr(value);
                    }
                }
                else
                {
                    result[item] = string.Empty;
                }
            }

            return result;
        }
    }
}

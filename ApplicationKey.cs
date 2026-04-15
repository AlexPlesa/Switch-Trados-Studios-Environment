using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Switch_Trados_Studios_Environment
{
    public class ApplicationKey
    {
        public Dictionary<string, string> ApplicationKeys = new Dictionary<string, string>()
        {
            {"Studio19" ,"n9JUV2wjQeKOVB0muXqL0Q%3D%3D"},
            {"Studio18" ,"n8JUV2wjQeKOVB0muXqL0Q%3D%3D"},
            {"Studio17" ,"n7JUV2wjQeKOVB0muXqL0Q%3D%3D"},
            {"Studio16" ,"n6JUV2wjQeKOVB0muXqL0Q%3D%3D"}
        };

        public void UpdateApplicationKeyBasedOnStudioVersion(string location, XmlNode configuration)
        {
            switch(location)
            {
                case string loc when loc.Contains("Studio19"):
                    configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio19"];
                    break;
                case string loc when loc.Contains("Studio18"):
                    configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio18"];
                    break;
                case string loc when loc.Contains("Studio17"):
                    configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio17"];
                    break;
                case string loc when loc.Contains("Studio16"):
                    configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = ApplicationKeys["Studio16"];
                    break;
                default:
                    throw new Exception("Unsupported Studio version");
            }
        }

        public void SetApplicationKey(string applicationKey, XmlNode newConfiguration)
        {
            newConfiguration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value = applicationKey;
        }

        public string GetApplicationKey(XmlNode configuration)
        {
            return configuration.SelectSingleNode($"{Constants.BestMatchServiceSettingsNode}/tracking/setting").Attributes["value"].Value;
        }
    }
}

using Microsoft.Win32;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Switch_Trados_Studios_Environment
{
    public class InstalledBuilds
    {
        public List<InstalledBuildInformation> listOfInstalledBuilds = new List<InstalledBuildInformation>();
        public Dictionary<int, string> studioBuildTypeDictionary = new Dictionary<int, string>();
        public int nrOfInstalledBuilds;

        public InstalledBuilds()
        {
            listOfInstalledBuilds = PopulateListOfInstalledBuilds();
            studioBuildTypeDictionary = PopulateStudioDictionaries();
            nrOfInstalledBuilds = studioBuildTypeDictionary.Count();
        }


        private List<InstalledBuildInformation> PopulateListOfInstalledBuilds()
        {
            RegistryKey SdlInstallRegistry = Registry.LocalMachine.OpenSubKey(Constants.Trados32bit);

            List<string> installedBuilds = new List<string>();
            List<InstalledBuildInformation> installedBuildsInformation = new List<InstalledBuildInformation>();
            if (SdlInstallRegistry != null)
            {
                var installedStudioBuilds = SdlInstallRegistry.GetSubKeyNames()
               .Where<string>(val => val.StartsWith("Studio1") && !val.Contains("License") && !val.Contains("MTStudio")).ToList();
                installedBuilds.AddRange(installedStudioBuilds);
            }
            foreach(var buildName in installedBuilds)
            {
                RegistryKey tradosInstallInformation = Registry.LocalMachine.OpenSubKey(Path.Combine(Constants.Trados32bit, buildName));
                installedBuildsInformation.Add(new InstalledBuildInformation
                {
                    DisplayName = (string)tradosInstallInformation.GetValue("DisplayName"),
                    DisplayVersion = (string)tradosInstallInformation.GetValue("DisplayVersion"),
                    InstallLocation = tradosInstallInformation.GetValue("DisplayIcon").ToString().Substring(0, tradosInstallInformation.GetValue("DisplayIcon").ToString().LastIndexOf('\\'))
                });
                
            }
            
            return installedBuildsInformation;
        }

        private Dictionary<int, string> PopulateStudioDictionaries()
        {
            int Key = 0;
            Dictionary<int, string> buildTypeDictionary = new Dictionary<int, string>();
            foreach (var build in listOfInstalledBuilds)
            {
                buildTypeDictionary.Add(Key, $"{build.DisplayName} {build.DisplayVersion}");
                Key++;
            }
            return buildTypeDictionary;
        }
    }
}

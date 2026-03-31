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
            List<(string RegistryPath, string BuildName)> installedBuilds = new List<(string, string)>();
            List<InstalledBuildInformation> installedBuildsInformation = new List<InstalledBuildInformation>();

            var registryPaths = new[] { Constants.Trados64bit, Constants.Trados32bit };

            foreach (var registryPath in registryPaths)
            {
                RegistryKey installRegistry = Registry.LocalMachine.OpenSubKey(registryPath);
                if (installRegistry != null)
                {
                    var installedStudioBuilds = installRegistry.GetSubKeyNames()
                        .Where(val => val.StartsWith("Studio1") && !val.Contains("License") && !val.Contains("MTStudio"))
                        .Where(val => !installedBuilds.Any(b => b.BuildName == val))
                        .Select(val => (registryPath, val))
                        .ToList();
                    installedBuilds.AddRange(installedStudioBuilds);
                }
            }

            foreach (var (registryPath, buildName) in installedBuilds)
            {
                RegistryKey tradosInstallInformation = Registry.LocalMachine.OpenSubKey(Path.Combine(registryPath, buildName));
                if (tradosInstallInformation != null)
                {
                    installedBuildsInformation.Add(new InstalledBuildInformation
                    {
                        DisplayName = (string)tradosInstallInformation.GetValue("DisplayName"),
                        DisplayVersion = (string)tradosInstallInformation.GetValue("DisplayVersion"),
                        InstallLocation = tradosInstallInformation.GetValue("DisplayIcon").ToString().Substring(0, tradosInstallInformation.GetValue("DisplayIcon").ToString().LastIndexOf('\\'))
                    });
                }
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

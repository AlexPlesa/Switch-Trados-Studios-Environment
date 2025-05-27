using System;
using System.IO;

namespace Switch_Trados_Studios_Environment
{
    internal class LanguageCloudConfig
    {

        private static string _machineUserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        InstalledBuilds installedBuilds = new InstalledBuilds();
        DirectoryInfo appdataDirectory = new DirectoryInfo($@"{_machineUserPath}\AppData\Roaming\");

        public void DeleteLanguageCloudConfig(int studioBuildType)
        {
            string buildType = installedBuilds.nrOfInstalledBuilds > studioBuildType ? installedBuilds.studioBuildTypeDictionary[studioBuildType] : "Studio18";
            string languageCloudMachineTranslationLocation = buildType.Contains("16.") ? "SDL\\SDL Trados Studio" : "Trados\\Trados Studio";


            if (Directory.Exists($@"{appdataDirectory}{languageCloudMachineTranslationLocation}"))
            {
                FileInfo fileInfo = new FileInfo($@"{appdataDirectory}{languageCloudMachineTranslationLocation}\{buildType}\{Constants.LanguageCloudMachineTranslation}");
                if (fileInfo.Exists) fileInfo.Delete();
            }
        }
    }
}

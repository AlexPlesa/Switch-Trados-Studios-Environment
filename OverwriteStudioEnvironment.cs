using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Linq;

namespace Switch_Trados_Studios_Environment
{
    class OverwriteStudioEnvironment
    {
        public InstalledBuilds installedBuilds = new InstalledBuilds();
        ApplicationKey applicationKey = new ApplicationKey();

        public void SwitchStudiosLcEnvironment(int selectedStudioTypeIndexe, int environment)
        {
            var selectedStudioType = installedBuilds.studioBuildTypeDictionary[selectedStudioTypeIndexe];
            string installLocation;
            string environmentFilePath;
            bool isDevBuild = selectedStudioType.Contains("\\");
            if (!isDevBuild) 
            {
                InstalledBuildInformation installedBuildInformation = installedBuilds.listOfInstalledBuilds.Single(b => $"{b.DisplayName} {b.DisplayVersion}" == selectedStudioType);
                installLocation = installedBuildInformation.InstallLocation;
                bool isMiltiregion = double.Parse(installedBuildInformation.DisplayVersion.Replace(".", ""))/10000 > 1801.9; //Checks if the installed studio version support Language Cloud multiregion
                environmentFilePath = EnvironmentFiles.GetPathToTheSpecificEnvironmentFile(environment, isMiltiregion);
            }
            else
            {
                installLocation = selectedStudioType;
                environmentFilePath = EnvironmentFiles.GetPathToTheSpecificEnvironmentFile(environment, true);
            }

            XmlDocument bestMatchServicesSettings = new XmlDocument();
            bestMatchServicesSettings.Load(environmentFilePath);
            var newLanguageCloudSyncConfig = bestMatchServicesSettings.SelectSingleNode(Constants.LanguageCloudSyncConfigNode);
            var newBestMatchServiceSettings = bestMatchServicesSettings.SelectSingleNode(Constants.BestMatchServiceSettingsNode);
            var newBestMatchServiceUrlConfig = bestMatchServicesSettings.SelectSingleNode(Constants.BestMatchServiceUrlsConfigNode);
            

            XmlDocument tradosStudioConfigFile = new XmlDocument();
            string lcEnvironmentFilePath = installedBuilds.nrOfInstalledBuilds <= selectedStudioTypeIndexe ?
                installedBuilds.studioBuildTypeDictionary[selectedStudioTypeIndexe] : 
                Path.Combine(installLocation, Constants.TradosStudioConfigFile);
            
            tradosStudioConfigFile.Load(lcEnvironmentFilePath);
            var oldLanguageCloudSyncConfig = tradosStudioConfigFile.SelectSingleNode(Constants.LanguageCloudSyncConfigNode);
            var oldBestMatchServiceSettings = tradosStudioConfigFile.SelectSingleNode(Constants.BestMatchServiceSettingsNode);
            var oldBestMatchServiceUrlConfig = tradosStudioConfigFile.SelectSingleNode(Constants.BestMatchServiceUrlsConfigNode);

            var configuration = oldBestMatchServiceSettings.ParentNode;
            if (!installLocation.Contains("Studio16"))
            {
                configuration.InsertBefore(tradosStudioConfigFile.ImportNode(newLanguageCloudSyncConfig, true), oldLanguageCloudSyncConfig);
                configuration.RemoveChild(oldLanguageCloudSyncConfig);
            }
            
            configuration.InsertBefore(tradosStudioConfigFile.ImportNode(newBestMatchServiceSettings, true), oldBestMatchServiceSettings);
            configuration.RemoveChild(oldBestMatchServiceSettings);

            configuration.InsertBefore(tradosStudioConfigFile.ImportNode(newBestMatchServiceUrlConfig, true), oldBestMatchServiceUrlConfig);
            configuration.RemoveChild(oldBestMatchServiceUrlConfig);

            applicationKey.ChangeApplicationKey(installLocation, configuration);
            
            tradosStudioConfigFile.Save(lcEnvironmentFilePath);
        }
    }
}

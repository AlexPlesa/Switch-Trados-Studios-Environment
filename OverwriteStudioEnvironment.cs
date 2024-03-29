﻿using System;
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
        EnvironmentFiles environmentFiles = new EnvironmentFiles();
        ApplicationKey applicationKey = new ApplicationKey();

        public void SwitchStudiosLcEnvironment(int selectedStudioTypeIndexe, int environment)
        {
            XmlDocument bestMatchServicesSettings = new XmlDocument();
            bestMatchServicesSettings.Load(environmentFiles.GetPathToTheSpecificEnvironmentFile(environment));
            var newLanguageCloudSyncConfig = bestMatchServicesSettings.SelectSingleNode(Constants.LanguageCloudSyncConfigNode);
            var newBestMatchServiceSettings = bestMatchServicesSettings.SelectSingleNode(Constants.BestMatchServiceSettingsNode);
            var newBestMatchServiceUrlConfig = bestMatchServicesSettings.SelectSingleNode(Constants.BestMatchServiceUrlsConfigNode);
            var selectedStudioType = installedBuilds.studioBuildTypeDictionary[selectedStudioTypeIndexe];
            var installLocation = selectedStudioType.Contains("\\") ? selectedStudioType : new InstallPath(selectedStudioType).installLocation;

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

using System;

namespace Switch_Trados_Studios_Environment
{
    public class Constants
    {
        public const string LanguageCloudMachineTranslation = "LanguageCloudMachineTranslation.bin";
        public const string TradosStudioConfigFile = "SDLTradosStudio.exe.config";
        public const string BestMatchServiceSettingsNode = "/configuration/BestMatchServiceSettings";
        public const string BestMatchServiceUrlsConfigNode = "/configuration/BestMatchServiceUrlsConfig";
        public const string LanguageCloudSyncConfigNode = "/configuration/LanguageCloudSyncConfig";
        public const string SdlInstallRegistryPath = @"SOFTWARE\WOW6432Node\SDL";
        public const string TradosInstallRegistryPath = @"SOFTWARE\WOW6432Node\Trados";
        public const string Trados64bit = $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"; // 64-bit programs on 64-bit system
        public const string Trados32bit = $@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\"; // 32-bit programs on 64-bit system
    }
}

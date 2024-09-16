using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Trados_Studios_Environment
{
    internal static class EnvironmentFiles
    {
        public static Dictionary<int, string> environmentDictionary = new Dictionary<int, string>();
        public static string environmentFolderPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), @"Environment Files\");

        public static string GetPathToTheSpecificEnvironmentFile(int environment, bool isMultiregion)
        {
            if (isMultiregion) return $@"{environmentFolderPath}\MultiregionConfigs\{environmentDictionary[environment]}";
            else return $@"{environmentFolderPath}\SingleRegionConfigs\{environmentDictionary[environment]}";
        }

        public static string AddCustomLocation()
        {
            var dialog = new OpenFileDialog();
            try
            {
                //dialog.IsFolderPicker = true;
                //dialog.EnsurePathExists = true;
                dialog.Filter = "config files (*.exe.config)|*.exe.config|All files (*.*)|*.*\\";
                dialog.ShowDialog();
                return dialog.FileName;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static string[] GetEnvironmentFilesDictionaries()
        {
            var environmentConfigFiles = Directory.GetFiles($@"{environmentFolderPath}\MultiregionConfigs\");

            for (int position = 0; position < environmentConfigFiles.Count(); position++)
            {
                FileInfo fileInfo = new FileInfo(environmentConfigFiles[position]);
                environmentDictionary.Add(position, fileInfo.Name);
            }

            return environmentConfigFiles;
        }
    }
}

using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;

namespace Switch_Trados_Studios_Environment
{
    class CheckForUpdates
    {
        public static async Task<(bool hasUpdate, string version)> IsNewVersionAvailableAsync()
        {
            string localXaml = await File.ReadAllTextAsync($@"{Path.GetDirectoryName(Environment.ProcessPath)}\AppVersion.xml").ConfigureAwait(false);
            string localVersion = ExtractVersionFromTitle(localXaml);

            string githubRawUrl = "https://raw.githubusercontent.com/AlexPlesa/Switch-Trados-Studios-Environment/master/MainWindow.xaml";
            using var httpClient = new HttpClient();
            try
            {
                string remoteXaml = await httpClient.GetStringAsync(githubRawUrl).ConfigureAwait(false);
                string remoteVersion = ExtractVersionFromTitle(remoteXaml);
                return (IsVersionGreater(remoteVersion, localVersion), remoteVersion);
            }
            catch (HttpRequestException)
            {
                // Handle network errors or GitHub access issues
                return (false, "Internet connection error");
            }
        }

        private static string ExtractVersionFromTitle(string xamlContent)
        {
            var match = Regex.Match(xamlContent, @"Title=""Switch Environment ([\d\.]+)""");
            return match.Success ? match.Groups[1].Value : "0.0.0";
        }

        private static bool IsVersionGreater(string remoteVersion, string localVersion)
        {
            var remoteVersionNumber = int.Parse(remoteVersion.Replace(".", string.Empty));
            var localVersionNumber = int.Parse(localVersion.Replace(".", string.Empty));

            return (remoteVersionNumber > localVersionNumber);
        }
    }
}


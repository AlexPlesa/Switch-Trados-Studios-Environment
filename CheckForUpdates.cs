using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Switch_Trados_Studios_Environment
{
    class CheckForUpdates
    {
        public static async Task<(bool hasUpdate, string version)> IsNewVersionAvailableAsync(string localWindowTitle)
        {
            string localVersion = Regex.Match(localWindowTitle, @"\d+\.\d+\.\d+").Value;

            string githubRawUrl = "https://raw.githubusercontent.com/AlexPlesa/Switch-Trados-Studios-Environment/master/MainWindow.xaml";
            using var httpClient = new HttpClient();
            try
            {
                string remoteXaml = await httpClient.GetStringAsync(githubRawUrl).ConfigureAwait(false);
                string remoteVersion = ExtractVersionFromGithubTitle(remoteXaml);
                return (IsVersionGreater(remoteVersion, localVersion), remoteVersion);
            }
            catch (HttpRequestException)
            {
                // Handle network errors or GitHub access issues
                return (false, "Internet connection error");
            }
        }

        private static string ExtractVersionFromGithubTitle(string xamlContent)
        {
            var match = Regex.Match(xamlContent, @"Switch Environment ([\d\.]+)""");
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


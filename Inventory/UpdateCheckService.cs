using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Inventory
{
    internal sealed class UpdateInfo
    {
        public Version Version { get; set; }
        public string TagName { get; set; }
        public string HtmlUrl { get; set; }
        public string Notes { get; set; }

        /// <summary>
        /// Direct download URL of a release asset named like "Inventory-vX.Y.Z.zip" (contains
        /// "Inventory", ends in ".zip"), if the release has one. Null when no such asset is
        /// attached - callers fall back to the notify-only flow in that case.
        /// </summary>
        public string AssetDownloadUrl { get; set; }
        public long AssetSize { get; set; }
    }

    /// <summary>
    /// Checks a public GitHub repo's Releases API for a version newer than AppVersion.Current.
    /// The repo (GITHUB_RELEASES_REPO) is public and the call is anonymous - no token, so none of
    /// the credential-sprawl concerns already flagged for App.config's DB settings apply here.
    /// System Config > Updates / StartUpdateCheckWatcher in Form1.cs decides what happens with the
    /// result: notify-only (open the release page, a deliberate manual action) unless the release
    /// has a matching zip asset and AUTO_UPDATE_ENABLED is on, in which case SelfUpdateService
    /// downloads and silently installs it once the till is idle - see SelfUpdateService.cs.
    /// </summary>
    internal static class UpdateCheckService
    {
        private const string DefaultRepo = "yeyjun/_conv_v3";

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        static UpdateCheckService()
        {
            // First external HTTPS API call in the app that parses a JSON response (existing
            // HttpClient usages elsewhere just hit plain-text endpoints) - GitHub's API requires
            // TLS 1.2, and nothing today guarantees the OS/.NET default negotiates up to it.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BrewCode-UpdateChecker");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        }

        /// <summary>
        /// Fails open (returns null, meaning "no update to show") on any error - network down,
        /// repo/releases not reachable, unparsable response, or a release tag that doesn't look
        /// like a version - matching this app's offline-resilience convention everywhere else.
        /// Callers cannot distinguish "up to date" from "couldn't check", which is the intended
        /// trade-off: a failed check should never interrupt the till with an error.
        /// </summary>
        public static async Task<UpdateInfo> CheckForUpdateAsync()
        {
            try
            {
                string repo = ConfigurationManager.AppSettings["GITHUB_RELEASES_REPO"];
                if (string.IsNullOrWhiteSpace(repo))
                    repo = DefaultRepo;

                using (HttpResponseMessage response = await _httpClient.GetAsync(
                    $"https://api.github.com/repos/{repo}/releases/latest"))
                {
                    if (!response.IsSuccessStatusCode)
                        return null; // includes 404, e.g. no releases published yet

                    string json = await response.Content.ReadAsStringAsync();
                    JObject release = JObject.Parse(json);

                    string tagName = release.Value<string>("tag_name");
                    if (string.IsNullOrWhiteSpace(tagName))
                        return null;

                    string versionText = tagName.StartsWith("v", StringComparison.OrdinalIgnoreCase)
                        ? tagName.Substring(1)
                        : tagName;

                    if (!Version.TryParse(versionText, out Version remoteVersion) ||
                        !Version.TryParse(AppVersion.Current, out Version localVersion))
                        return null;

                    if (remoteVersion <= localVersion)
                        return null;

                    JArray assets = release.Value<JArray>("assets") ?? new JArray();
                    JObject zipAsset = assets
                        .OfType<JObject>()
                        .FirstOrDefault(a => IsSelfUpdateZipAssetName(a.Value<string>("name")));

                    return new UpdateInfo
                    {
                        Version = remoteVersion,
                        TagName = tagName,
                        HtmlUrl = release.Value<string>("html_url"),
                        Notes = release.Value<string>("body"),
                        AssetDownloadUrl = zipAsset?.Value<string>("browser_download_url"),
                        AssetSize = zipAsset?.Value<long?>("size") ?? 0,
                    };
                }
            }
            catch
            {
                return null;
            }
        }

        private static bool IsSelfUpdateZipAssetName(string name) =>
            !string.IsNullOrWhiteSpace(name) &&
            name.IndexOf("Inventory", StringComparison.OrdinalIgnoreCase) >= 0 &&
            name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
    }
}

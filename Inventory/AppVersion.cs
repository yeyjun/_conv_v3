namespace Inventory
{
    /// <summary>
    /// The single human-meaningful version number for this build - AssemblyVersion and
    /// AssemblyFileVersion are both frozen at "1.0.0.0" and never read anywhere in the app, and
    /// Inventory.csproj's ApplicationVersion is for the unused ClickOnce feed. Bump this by hand
    /// to match the git tag before cutting a new GitHub Release (e.g. tag "v1.0.1" ->
    /// Current = "1.0.1"), so UpdateCheckService has something to compare the release tag against.
    /// </summary>
    internal static class AppVersion
    {
        public const string Current = "1.0.0";
    }
}

using Microsoft.Win32;
using System.Windows.Forms;

namespace Inventory
{
    // Launches BrewCode when Windows starts, via the per-user Run key (HKCU) rather than
    // the Startup folder or a scheduled task - no admin rights needed, and it only affects
    // the current Windows user, matching the rest of this app's per-machine settings.
    internal static class AutoStartService
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "BrewCode";

        public static bool IsEnabled
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false))
                    return key?.GetValue(ValueName) != null;
            }
        }

        public static void SetEnabled(bool enabled)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKeyPath))
            {
                if (enabled)
                    key.SetValue(ValueName, "\"" + Application.ExecutablePath + "\"");
                else
                    key.DeleteValue(ValueName, throwOnMissingValue: false);
            }
        }
    }
}

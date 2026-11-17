using System;
using System.Globalization;
using System.IO;

namespace _conv
{
    /// <summary>
    /// Tracks whether the first-run "Getting Started" walkthrough (<see cref="TutorialForm"/>) has
    /// already been shown on this machine. Local, per-machine state (Documents\App
    /// Inventory\OfflineCache\TutorialShown.txt) rather than a DB/App.config setting, the same
    /// reasoning TrialLicense uses for its trial-start marker - it should survive independently of
    /// which store user logs in and not require a DB round-trip just to decide whether to pop up
    /// a dialog.
    /// </summary>
    internal static class TutorialState
    {
        private static string StateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "TutorialShown.txt");

        public static bool HasBeenShown
        {
            get
            {
                try
                {
                    return File.Exists(StateFilePath);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>Records that the walkthrough has been shown, so it won't auto-launch again on
        /// this machine. Safe to call even if it can't be written - worst case it shows again next
        /// launch, which beats blocking startup over it.</summary>
        public static void MarkShown()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StateFilePath));
                File.WriteAllText(StateFilePath, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
            }
            catch
            {
            }
        }
    }
}

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Inventory
{
    /// <summary>
    /// Time-limited trial gate for new installs of this app. The trial clock is a local,
    /// per-machine timestamp (Documents\App Inventory\OfflineCache\TrialStart.txt) stamped the
    /// first time the app ever runs - not App.config - so it survives independently of config
    /// edits, the same reasoning ScheduledReportService uses for its last-sent marker.
    /// TRIAL_ENABLED/TRIAL_DAYS/LICENSE_KEY (System Config > Trial / Licensing) control whether
    /// the gate applies at all, how long it lasts, and how to bypass it permanently.
    /// </summary>
    internal static class TrialLicense
    {
        private const int DefaultTrialDays = 30;

        // Not meant to withstand someone reading the source - only to stop a key issued for one
        // store working on another and rule out casual guesses. Consistent with this app's
        // existing security bar (MANAGER_PIN/DB credentials already sit in plaintext App.config).
        private const string LicenseSalt = "BACLIG-APPINV-TRIAL-2026";

        // Encrypts .tmrgo license files so opening one in Notepad (or any other editor) shows
        // ciphertext, not the store code/key in the clear. Same reasoning as LicenseSalt above -
        // duplicated verbatim in RemoteController\LicenseKeyGenerator.cs, which has no project
        // reference to this one and must produce byte-for-byte compatible output. Change either
        // copy and the other stops being able to read/write files the other one made.
        private const string FileKeyPassphrase = "BACLIG-APPINV-TMRGO-FILEKEY-2026";
        private static readonly byte[] FileKeySalt = Encoding.UTF8.GetBytes("BACLIG-TMRGO-SALT");

        private static string StateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "TrialStart.txt");

        // A separate marker from TrialStart.txt/LICENSE_KEY on purpose: this one just means "the
        // first-run Registration form has been completed on this machine", independent of whether
        // a license was actually entered there (the store may have chosen to continue on the
        // trial instead). Without it, a new install with TRIAL_ENABLED=false (this app's own
        // production App.config ships that way) would never see the gate at all.
        private static string RegisteredMarkerPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "Registered.txt");

        public static bool IsRegistered => File.Exists(RegisteredMarkerPath);

        public static void MarkRegistered()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RegisteredMarkerPath));
                File.WriteAllText(RegisteredMarkerPath, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
            }
            catch
            {
                // Best-effort, same as EnsureTrialStarted below - worst case the Registration
                // form reappears on the next launch, which is just an extra click, not data loss.
            }
        }

        public static bool IsEnabled =>
            bool.TryParse(ConfigurationManager.AppSettings["TRIAL_ENABLED"], out bool enabled) && enabled;

        public static int TrialDays =>
            int.TryParse(ConfigurationManager.AppSettings["TRIAL_DAYS"], out int days) && days > 0
                ? days
                : DefaultTrialDays;

        /// <summary>Stamps the trial start date on first run. Cheap to call on every startup.</summary>
        public static DateTime EnsureTrialStarted()
        {
            DateTime? existing = ReadTrialStart();
            if (existing.HasValue)
                return existing.Value;

            DateTime now = DateTime.UtcNow;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StateFilePath));
                File.WriteAllText(StateFilePath, now.ToString("o", CultureInfo.InvariantCulture));
            }
            catch
            {
                // If this can't be written, the trial can't be tracked across restarts - treat
                // it as started "now" for this run rather than blocking startup over it.
            }
            return now;
        }

        public static DateTime? TrialStartDate => ReadTrialStart();

        private static DateTime? ReadTrialStart()
        {
            try
            {
                if (!File.Exists(StateFilePath))
                    return null;

                return DateTime.TryParse(File.ReadAllText(StateFilePath), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out DateTime start) ? (DateTime?)start : null;
            }
            catch
            {
                return null;
            }
        }

        public static int DaysRemaining
        {
            get
            {
                DateTime start = TrialStartDate ?? DateTime.UtcNow;
                int elapsed = (int)(DateTime.UtcNow - start).TotalDays;
                return Math.Max(0, TrialDays - elapsed);
            }
        }

        private static string StoreCode => ConfigurationManager.AppSettings["STORE_CODE"] ?? "";

        public static bool IsLicensed => GetLicenseStatus().IsActive;

        /// <summary>Expiry date of the currently configured LICENSE_KEY, if it's a validly-issued
        /// (even if now expired) key for this store's STORE_CODE. Null for a perpetual license,
        /// an unset key, or a key that doesn't validate at all.</summary>
        public static DateTime? LicenseExpiryUtc => GetLicenseStatus().ExpiryUtc;

        /// <summary>Subscription tier (Basic/Pro/Enterprise) of the most recently imported
        /// .tmrgo file - set in TryActivateFromFile, blank for a manually-typed key (which
        /// carries no tier) or a fresh install that's never imported one. Purely descriptive,
        /// same as the Tier field in the file itself - nothing here gates features by tier.</summary>
        public static string SubscriptionTier => ConfigurationManager.AppSettings["SUBSCRIPTION_TIER"] ?? "";

        private static void SetSubscriptionTier(string tier)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["SUBSCRIPTION_TIER"] == null)
                    config.AppSettings.Settings.Add("SUBSCRIPTION_TIER", tier);
                else
                    config.AppSettings.Settings["SUBSCRIPTION_TIER"].Value = tier;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
                // Non-critical - worst case the tier just doesn't show up in System Config's
                // status label, same fail-soft reasoning as EnsureTrialStarted/MarkRegistered.
            }
        }

        /// <summary>True once a license key was entered for this store and its embedded expiry
        /// date has passed - distinct from the trial simply running out, so the UI can tell a
        /// cashier/manager "renew your license" instead of "your trial ended".</summary>
        public static bool HasExpiredLicenseKey
        {
            get
            {
                (bool isActive, DateTime? expiryUtc, bool isValidKey) = GetLicenseStatus();
                return isValidKey && !isActive && expiryUtc.HasValue;
            }
        }

        private static (bool IsActive, DateTime? ExpiryUtc, bool IsValidKey) GetLicenseStatus()
        {
            string entered = (ConfigurationManager.AppSettings["LICENSE_KEY"] ?? "").Trim();
            if (string.IsNullOrEmpty(entered) || !TryParseAndValidateKey(entered, StoreCode, out DateTime? expiryUtc))
                return (false, null, false);

            bool active = !expiryUtc.HasValue || DateTime.UtcNow.Date <= expiryUtc.Value;
            return (active, expiryUtc, true);
        }

        /// <summary>True only once the trial has actually run out - never true before the trial start is stamped.</summary>
        public static bool IsExpired =>
            IsEnabled && !IsLicensed && TrialStartDate.HasValue && DaysRemaining <= 0;

        /// <summary>Validates the entered key against this store's Store Code and its own embedded
        /// expiry date and, if valid and not already expired, persists it to App.config.</summary>
        public static bool TryActivate(string enteredKey)
        {
            string trimmed = (enteredKey ?? "").Trim();
            if (!TryParseAndValidateKey(trimmed, StoreCode, out DateTime? expiryUtc))
                return false;
            if (expiryUtc.HasValue && DateTime.UtcNow.Date > expiryUtc.Value)
                return false;

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["LICENSE_KEY"] == null)
                    config.AppSettings.Settings.Add("LICENSE_KEY", trimmed);
                else
                    config.AppSettings.Settings["LICENSE_KEY"].Value = trimmed;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Activates from a ".tmrgo" file (produced by the standalone RemoteController
        /// companion app's License Key Generator) instead of typing the key in by hand. The file
        /// is AES-encrypted - opening it in Notepad or any other editor shows ciphertext, not the
        /// store code/key - so it's decrypted here first, then parsed the same "Key=Value" per
        /// line format as before. StoreCode is optional and only used to give a clearer error
        /// when the file was clearly issued for a different store; the key itself is what's
        /// actually validated, the same as manual entry.
        /// </summary>
        public static bool TryActivateFromFile(string filePath, out string error)
        {
            error = null;
            try
            {
                string content;
                try
                {
                    content = Encoding.UTF8.GetString(DecryptFile(File.ReadAllBytes(filePath)));
                }
                catch (CryptographicException)
                {
                    error = "That file isn't a valid .tmrgo license file.";
                    return false;
                }

                var values = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split(new[] { '=' }, 2))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);

                if (!values.TryGetValue("LicenseKey", out string key) || string.IsNullOrWhiteSpace(key))
                {
                    error = "That file doesn't contain a LicenseKey line.";
                    return false;
                }

                if (values.TryGetValue("StoreCode", out string fileStoreCode) &&
                    !string.IsNullOrWhiteSpace(fileStoreCode) &&
                    !string.Equals(fileStoreCode.Trim(), StoreCode, StringComparison.OrdinalIgnoreCase))
                {
                    error = $"This license file was issued for store code \"{fileStoreCode.Trim()}\", not \"{StoreCode}\".";
                    return false;
                }

                if (!TryActivate(key))
                {
                    error = "The license key in that file isn't valid for this store.";
                    return false;
                }

                // Tier is purely descriptive (see LicenseKeyGenerator.SaveLicenseFile) - not part
                // of what TryActivate just validated - so a file without it (e.g. one generated
                // before this field existed) still activates fine, just without a tier to show.
                if (values.TryGetValue("Tier", out string tier) && !string.IsNullOrWhiteSpace(tier))
                    SetSubscriptionTier(tier.Trim());

                return true;
            }
            catch (Exception ex)
            {
                error = $"Could not read that file: {ex.Message}";
                return false;
            }
        }

        /// <summary>Deterministic per-store, per-expiry key: SHA256(STORE_CODE + salt + expiry
        /// segment), formatted as 4x4 hex groups. Mirrors
        /// RemoteController\LicenseKeyGenerator.cs's ComputeKey exactly - keep both in sync.</summary>
        private static string ComputeExpectedKey(string storeCode, string expirySegment)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes((storeCode ?? "").Trim().ToUpperInvariant() + LicenseSalt + expirySegment));
                string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
                return string.Join("-", Enumerable.Range(0, 4).Select(i => hex.Substring(i * 4, 4)));
            }
        }

        /// <summary>
        /// Splits a "XXXX-XXXX-XXXX-XXXX-EXPIRY" key into its hash-group body and expiry segment,
        /// re-derives the expected hash for that exact expiry segment, and returns the parsed
        /// expiry (null for a perpetual "PERM" key) only if the two match. A tampered date segment
        /// (e.g. hand-editing "PERM" onto an otherwise-expired key, or pushing a yyMMdd out further)
        /// no longer matches its hash group, so it fails here rather than silently extending itself.
        /// </summary>
        private static bool TryParseAndValidateKey(string fullKey, string storeCode, out DateTime? expiryUtc)
        {
            expiryUtc = null;
            string[] parts = (fullKey ?? "").Trim().Split('-');
            if (parts.Length != 5)
                return false;

            string keyBody = string.Join("-", parts.Take(4));
            string expirySegment = parts[4].ToUpperInvariant();

            if (expirySegment != "PERM")
            {
                if (!DateTime.TryParseExact(expirySegment, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedExpiry))
                    return false;
                expiryUtc = parsedExpiry.Date;
            }

            return string.Equals(keyBody, ComputeExpectedKey(storeCode, expirySegment), StringComparison.OrdinalIgnoreCase);
        }

        private static byte[] DecryptFile(byte[] cipherBytes)
        {
            (byte[] key, byte[] iv) = DeriveFileKey();
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            }
        }

        private static (byte[] key, byte[] iv) DeriveFileKey()
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(FileKeyPassphrase, FileKeySalt, 10000))
                return (deriveBytes.GetBytes(32), deriveBytes.GetBytes(16));
        }
    }
}

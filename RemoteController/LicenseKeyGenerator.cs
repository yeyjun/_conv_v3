using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RemoteController
{
    /// <summary>
    /// Issues license keys for BrewCode installs. This tool is intentionally detached from
    /// the main app's project (no shared reference, no build dependency) so it can be handed to
    /// someone who only ever needs to issue licenses, not build or run the POS app itself - but
    /// that means the key algorithm below is a deliberate copy of
    /// Inventory\TrialLicense.cs's ComputeExpectedKey, and the two MUST be changed together.
    /// If you rotate LicenseSalt here, every previously issued key file becomes invalid unless
    /// Inventory\TrialLicense.cs is updated with the exact same value at the same time.
    /// </summary>
    internal static class LicenseKeyGenerator
    {
        private const string LicenseSalt = "BACLIG-APPINV-TRIAL-2026";

        // Encrypts .tmrgo files so opening one in Notepad (or any other editor) shows ciphertext,
        // not the store code/key in the clear - only TrialLicense.cs's matching DecryptFile can
        // read it back. Duplicated verbatim there since this project has no reference to it;
        // change one copy and the other stops being able to read files this one writes.
        private const string FileKeyPassphrase = "BACLIG-APPINV-TMRGO-FILEKEY-2026";
        private static readonly byte[] FileKeySalt = Encoding.UTF8.GetBytes("BACLIG-TMRGO-SALT");

        /// <summary>Encodes an expiry date the same way on both ends of the key: "PERM" for a
        /// perpetual license, otherwise a 6-digit yyMMdd - short enough to keep the key
        /// copy/paste-able while still being human-readable in the key itself.</summary>
        private static string ExpirySegment(DateTime? expiryUtc) =>
            expiryUtc.HasValue ? expiryUtc.Value.Date.ToString("yyMMdd") : "PERM";

        /// <summary>
        /// Deterministic per-store, per-expiry key: SHA256(STORE_CODE + salt + expiry segment),
        /// formatted as 4x4 hex groups with the expiry segment appended as a 5th group. Baking the
        /// expiry into the hash input (rather than just appending it unchecked) means changing the
        /// date segment without regenerating the key makes it fail validation - the expiry can't
        /// be extended by editing the string.
        /// </summary>
        public static string ComputeKey(string storeCode, DateTime? expiryUtc)
        {
            string expirySegment = ExpirySegment(expiryUtc);
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes((storeCode ?? "").Trim().ToUpperInvariant() + LicenseSalt + expirySegment));
                string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
                string keyBody = string.Join("-", Enumerable.Range(0, 4).Select(i => hex.Substring(i * 4, 4)));
                return $"{keyBody}-{expirySegment}";
            }
        }

        /// <summary>
        /// Writes an AES-encrypted ".tmrgo" file - decrypted, it's the same "Key=Value" per line
        /// format TrialLicense.TryActivateFromFile expects. StoreCode and ExpiryUtc are included
        /// only so a future activation attempt can give a clearer error, and so the file is
        /// human-readable if decrypted for troubleshooting; the license key itself (which already
        /// has the expiry baked into its own hash) is what's actually checked.
        /// </summary>
        public static void SaveLicenseFile(string filePath, string storeCode, string licenseKey, DateTime? expiryUtc,
            string tier, int? monthsOfUse)
        {
            string content =
                $"StoreCode={storeCode.Trim()}{Environment.NewLine}" +
                $"Tier={tier}{Environment.NewLine}" +
                $"MonthsOfUse={(monthsOfUse.HasValue ? monthsOfUse.Value.ToString() : "PERM")}{Environment.NewLine}" +
                $"LicenseKey={licenseKey}{Environment.NewLine}" +
                $"ExpiryUtc={(expiryUtc.HasValue ? expiryUtc.Value.ToString("o") : "PERM")}{Environment.NewLine}" +
                $"IssuedUtc={DateTime.UtcNow:o}{Environment.NewLine}";

            File.WriteAllBytes(filePath, EncryptFile(Encoding.UTF8.GetBytes(content)));
        }

        private static byte[] EncryptFile(byte[] plainBytes)
        {
            (byte[] key, byte[] iv) = DeriveFileKey();
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
        }

        private static (byte[] key, byte[] iv) DeriveFileKey()
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(FileKeyPassphrase, FileKeySalt, 10000))
                return (deriveBytes.GetBytes(32), deriveBytes.GetBytes(16));
        }
    }
}

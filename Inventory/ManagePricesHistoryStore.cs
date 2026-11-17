using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Inventory
{
    /// <summary>
    /// Persists the Manage Prices tab's "Recent Price Edits" history panel so it survives an app
    /// restart instead of only lasting the current session. Written as XML, then AES-encrypted
    /// before hitting disk - the on-disk file is ciphertext, not readable item prices/serials in
    /// the clear, if someone opens it outside the app. Same key-derivation approach (a hardcoded
    /// passphrase/salt through Rfc2898DeriveBytes) as TrialLicense.cs's .tmrgo files: not meant to
    /// withstand someone reading this source, just to stop casual inspection of the file itself -
    /// consistent with this app's existing security bar.
    /// </summary>
    internal static class ManagePricesHistoryStore
    {
        private const string FileKeyPassphrase = "BACLIG-APPINV-MANAGEPRICES-HISTORY-2026";
        private static readonly byte[] FileKeySalt = Encoding.UTF8.GetBytes("BACLIG-MANAGEPRICES-SALT");

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "ManagePricesHistory.xml");

        /// <summary>Overwrites the stored history with the given entries, newest-first (the same
        /// order the history panel displays them in).</summary>
        public static void Save(IEnumerable<string> entries)
        {
            try
            {
                var root = new XElement("ManagePricesHistory",
                    entries.Select(entry => new XElement("Entry", entry)));

                using (var plainStream = new MemoryStream())
                {
                    root.Save(plainStream);

                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                    File.WriteAllBytes(FilePath, Encrypt(plainStream.ToArray()));
                }
            }
            catch
            {
                // Best-effort only - a failed write here should never block editing prices.
            }
        }

        /// <summary>Returns the stored entries, newest-first, or an empty list if nothing's been
        /// saved yet or the file can't be read/decrypted (e.g. it was hand-edited/corrupted).</summary>
        public static List<string> Load()
        {
            var result = new List<string>();
            try
            {
                if (!File.Exists(FilePath))
                    return result;

                byte[] plainBytes = Decrypt(File.ReadAllBytes(FilePath));

                XElement root;
                using (var plainStream = new MemoryStream(plainBytes))
                    root = XElement.Load(plainStream);

                result.AddRange(root.Elements("Entry").Select(entry => entry.Value));
            }
            catch
            {
                // Corrupt/foreign file, or decryption failed - start with an empty history
                // rather than crashing Manage Prices over it.
            }
            return result;
        }

        private static byte[] Encrypt(byte[] plainBytes)
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

        private static byte[] Decrypt(byte[] cipherBytes)
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

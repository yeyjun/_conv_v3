using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Inventory
{
    internal static class OperationLog
    {
        private static readonly object SyncRoot = new object();

        private const string FileKeyPassphrase = "BACLIG-APPINV-OPERATIONLOG-FILEKEY-2026";
        private static readonly byte[] FileKeySalt = Encoding.UTF8.GetBytes("BACLIG-OPERATIONLOG-SALT");

        private static string DirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "Operations");

        private static string FilePathFor(DateTime when) =>
            Path.Combine(DirectoryPath, $"{when:yyyy-MM-dd}.xml");

        public static void RecordCreate(string module, string summary) =>
            Append(module, new XElement("Create", new XAttribute("summary", summary ?? "")));

        public static void RecordRead(string module, string summary) =>
            Append(module, new XElement("Read", new XAttribute("summary", summary ?? "")));

        public static void RecordUpdate(string module, string summary) =>
            Append(module, new XElement("Update", new XAttribute("summary", summary ?? "")));

        public static void RecordDelete(string module, string summary) =>
            Append(module, new XElement("Delete", new XAttribute("summary", summary ?? "")));

        public static void RecordLogin(string module, string summary) =>
            Append(module, new XElement("Login", new XAttribute("summary", summary ?? "")));

        public static void RecordAction(string module, string summary) =>
            Append(module, new XElement("Action", new XAttribute("summary", summary ?? "")));

        public static void RecordSale(string module, string transactionCode, string seller, decimal grandTotal,
            IEnumerable<(string Serial, int Quantity, decimal ItemPrice)> lineItems)
        {
            var details = new XElement("Sale",
                new XAttribute("transactionCode", transactionCode ?? ""),
                new XAttribute("seller", seller ?? ""),
                new XAttribute("grandTotal", grandTotal),
                (lineItems ?? Enumerable.Empty<(string Serial, int Quantity, decimal ItemPrice)>())
                    .Select(item => new XElement("Item",
                        new XAttribute("serial", item.Serial ?? ""),
                        new XAttribute("quantity", item.Quantity),
                        new XAttribute("price", item.ItemPrice))));

            Append(module, details);
        }

        public static string ReadRawXml(DateTime day)
        {
            try
            {
                return LoadDecrypted(FilePathFor(day))?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static void Append(string module, XElement details)
        {
            lock (SyncRoot)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string path = FilePathFor(now);
                    Directory.CreateDirectory(DirectoryPath);

                    XElement root;
                    try
                    {
                        root = LoadDecrypted(path) ?? new XElement("Operations");
                    }
                    catch
                    {
                        root = new XElement("Operations");
                    }

                    int nextNumber = root.Elements("Operation")
                        .Select(op => ParseOpNumber((string)op.Attribute("id")))
                        .DefaultIfEmpty(0)
                        .Max() + 1;

                    root.Add(new XElement("Operation",
                        new XAttribute("id", $"OP{nextNumber}"),
                        new XAttribute("timestamp", now.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XAttribute("module", module ?? "Unknown"),
                        details));

                    SaveEncrypted(path, root);
                }
                catch
                {
                }
            }
        }

        private static int ParseOpNumber(string id) =>
            !string.IsNullOrEmpty(id) && id.StartsWith("OP", StringComparison.Ordinal)
                && int.TryParse(id.Substring(2), out int n) ? n : 0;

        private static XElement LoadDecrypted(string path)
        {
            if (!File.Exists(path))
                return null;

            byte[] plainBytes = DecryptFile(File.ReadAllBytes(path));
            return XElement.Parse(Encoding.UTF8.GetString(plainBytes));
        }

        private static void SaveEncrypted(string path, XElement root)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting));
            File.WriteAllBytes(path, EncryptFile(plainBytes));
        }

        private static byte[] EncryptFile(byte[] plainBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveFileKey();
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
                    return result;
                }
            }
        }

        private static byte[] DecryptFile(byte[] fileBytes)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] iv = new byte[aes.BlockSize / 8];
                Buffer.BlockCopy(fileBytes, 0, iv, 0, iv.Length);

                aes.Key = DeriveFileKey();
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    return decryptor.TransformFinalBlock(fileBytes, iv.Length, fileBytes.Length - iv.Length);
            }
        }

        private static byte[] DeriveFileKey()
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(FileKeyPassphrase, FileKeySalt, 10000))
                return deriveBytes.GetBytes(32);
        }
    }
}

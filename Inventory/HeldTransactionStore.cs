using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Inventory
{
    internal static class HeldTransactionStore
    {
        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "HeldTransactions.xml");

        public static void Save(IEnumerable<Purchases.HeldTransaction> transactions)
        {
            try
            {
                var root = new XElement("HeldTransactions",
                    transactions.Select(t => new XElement("Transaction",
                        new XAttribute("ticketCode", t.TicketCode ?? ""),
                        new XAttribute("transactionCode", t.TransactionCode ?? ""),
                        new XAttribute("selltypeIndex", t.SelltypeIndex),
                        new XAttribute("grandTotal", t.GrandTotal),
                        new XAttribute("counterText", t.CounterText ?? ""),
                        new XAttribute("heldAt", t.HeldAt.ToString("o", CultureInfo.InvariantCulture)),
                        new XAttribute("remarks", t.Remarks ?? ""),
                        t.Items.Select(row => new XElement("Row",
                            row.Select(cell => new XElement("Cell", cell?.ToString() ?? "")))))));

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                root.Save(FilePath);
            }
            catch
            {
            }
        }

        public static List<Purchases.HeldTransaction> Load()
        {
            var result = new List<Purchases.HeldTransaction>();
            try
            {
                if (!File.Exists(FilePath))
                    return result;

                XElement root = XElement.Load(FilePath);
                foreach (XElement t in root.Elements("Transaction"))
                {
                    string ticketCode = (string)t.Attribute("ticketCode");
                    if (string.IsNullOrEmpty(ticketCode))
                        continue;

                    var items = t.Elements("Row")
                        .Select(row => row.Elements("Cell").Select(c => (object)c.Value).ToArray())
                        .ToList();

                    DateTime.TryParse((string)t.Attribute("heldAt"), CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind, out DateTime heldAt);

                    result.Add(new Purchases.HeldTransaction
                    {
                        TicketCode = ticketCode,
                        TransactionCode = (string)t.Attribute("transactionCode"),
                        SelltypeIndex = (int?)t.Attribute("selltypeIndex") ?? -1,
                        GrandTotal = (decimal?)t.Attribute("grandTotal") ?? 0m,
                        CounterText = (string)t.Attribute("counterText"),
                        HeldAt = heldAt,
                        Remarks = (string)t.Attribute("remarks"),
                        Items = items,
                    });
                }
            }
            catch
            {
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace YourApp.Data
{
    internal static class PendingItemStore
    {
        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "PendingItems.xml");

        public static DateTime? GetLastUpdated()
        {
            try
            {
                return File.Exists(FilePath) ? (DateTime?)File.GetLastWriteTime(FilePath) : null;
            }
            catch
            {
                return null;
            }
        }

        public static int GetPendingCount()
        {
            try
            {
                return LoadElements().Count;
            }
            catch
            {
                return 0;
            }
        }

        public static bool IsPending(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                return false;

            return LoadElements().Any(i =>
                string.Equals((string)i.Attribute("serial"), serial, StringComparison.OrdinalIgnoreCase));
        }

        public static void Add(string serial, string description, string category, decimal price, decimal retailPrice,
            decimal pricePerPc, int quantity, DateTime dateOfEntry)
        {
            List<XElement> items = LoadElements();
            items.RemoveAll(i => string.Equals((string)i.Attribute("serial"), serial, StringComparison.OrdinalIgnoreCase));
            items.Add(new XElement("Item",
                new XAttribute("serial", serial),
                new XAttribute("description", description),
                new XAttribute("category", category ?? ""),
                new XAttribute("price", price),
                new XAttribute("retail_price", retailPrice),
                new XAttribute("price_perpc", pricePerPc),
                new XAttribute("quantity", quantity),
                new XAttribute("date_of_entry", dateOfEntry.ToString("o"))));

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            new XElement("PendingItems", items).Save(FilePath);

            try
            {
                DataTable current = DataCacheManager.Instance.GetTable("Items");
                DataTable merged = current.Copy();
                MergeInto(merged);
                DataCacheManager.Instance.ReplaceTable("Items", merged);
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public readonly struct Record
        {
            public Record(string serial, string description, string category, decimal price,
                decimal retailPrice, decimal pricePerPc, int quantity, DateTime dateOfEntry)
            {
                Serial = serial;
                Description = description;
                Category = category;
                Price = price;
                RetailPrice = retailPrice;
                PricePerPc = pricePerPc;
                Quantity = quantity;
                DateOfEntry = dateOfEntry;
            }

            public string Serial { get; }
            public string Description { get; }
            public string Category { get; }
            public decimal Price { get; }
            public decimal RetailPrice { get; }
            public decimal PricePerPc { get; }
            public int Quantity { get; }
            public DateTime DateOfEntry { get; }
        }

        /// <summary>Every item still queued offline, for PendingItemSyncService to push to the
        /// real database once it's reachable again.</summary>
        public static List<Record> GetAll() =>
            LoadElements().Select(i => new Record(
                (string)i.Attribute("serial"),
                (string)i.Attribute("description"),
                (string)i.Attribute("category") ?? "",
                (decimal?)i.Attribute("price") ?? 0m,
                (decimal?)i.Attribute("retail_price") ?? 0m,
                (decimal?)i.Attribute("price_perpc") ?? 0m,
                (int?)i.Attribute("quantity") ?? 0,
                (DateTime?)i.Attribute("date_of_entry") ?? DateTime.Now))
            .ToList();

        /// <summary>Removes one item from the pending queue once PendingItemSyncService has
        /// successfully written it to the database - it no longer needs to be merged into the
        /// cache as a local-only overlay.</summary>
        public static void Remove(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                return;

            List<XElement> items = LoadElements();
            int removed = items.RemoveAll(i =>
                string.Equals((string)i.Attribute("serial"), serial, StringComparison.OrdinalIgnoreCase));
            if (removed == 0)
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            new XElement("PendingItems", items).Save(FilePath);
        }

        public static void AdjustQuantityIfPending(string serial, long delta) =>
            UpdateQuantityIfPending(serial, current => current + delta);

        public static void SetQuantityIfPending(string serial, long value) =>
            UpdateQuantityIfPending(serial, _ => value);

        private static void UpdateQuantityIfPending(string serial, Func<long, long> newQuantity)
        {
            if (string.IsNullOrEmpty(serial))
                return;

            try
            {
                List<XElement> items = LoadElements();
                XElement item = items.FirstOrDefault(i =>
                    string.Equals((string)i.Attribute("serial"), serial, StringComparison.OrdinalIgnoreCase));
                if (item == null)
                    return;

                long current = (long?)item.Attribute("quantity") ?? 0;
                item.SetAttributeValue("quantity", newQuantity(current));

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                new XElement("PendingItems", items).Save(FilePath);
            }
            catch
            {
            }
        }

        public static void MergeInto(DataTable itemsTable)
        {
            if (itemsTable == null || !itemsTable.Columns.Contains("serial"))
                return;

            foreach (XElement item in LoadElements())
            {
                try
                {
                    MergeOne(itemsTable, item);
                }
                catch
                {
                }
            }
        }

        private static void MergeOne(DataTable itemsTable, XElement item)
        {
            string serial = (string)item.Attribute("serial");
            if (string.IsNullOrEmpty(serial))
                return;

            DataRow existing = FindRowBySerial(itemsTable, serial);
            DataRow row = existing ?? itemsTable.NewRow();

            SetIfExists(row, itemsTable, "serial", serial);
            SetIfExists(row, itemsTable, "description", (string)item.Attribute("description"));
            SetIfExists(row, itemsTable, "category", (string)item.Attribute("category"));
            SetIfExists(row, itemsTable, "price", (decimal?)item.Attribute("price"));
            SetIfExists(row, itemsTable, "retail_price", (decimal?)item.Attribute("retail_price"));
            SetIfExists(row, itemsTable, "price_perpc", (decimal?)item.Attribute("price_perpc"));
            SetIfExists(row, itemsTable, "new_quantity", (int?)item.Attribute("quantity"));
            SetIfExists(row, itemsTable, "qty_inpc", (int?)item.Attribute("quantity"));
            SetIfExists(row, itemsTable, "date_of_entry", (DateTime?)item.Attribute("date_of_entry"));

            if (existing == null)
                itemsTable.Rows.Add(row);
        }

        private static DataRow FindRowBySerial(DataTable table, string serial)
        {
            foreach (DataRow row in table.Rows)
            {
                if (string.Equals(row["serial"] as string, serial, StringComparison.OrdinalIgnoreCase))
                    return row;
            }

            return null;
        }

        private static void SetIfExists(DataRow row, DataTable table, string column, object value)
        {
            if (table.Columns.Contains(column))
                row[column] = value ?? DBNull.Value;
        }

        private static List<XElement> LoadElements()
        {
            try
            {
                return File.Exists(FilePath)
                    ? new List<XElement>(XElement.Load(FilePath).Elements("Item"))
                    : new List<XElement>();
            }
            catch
            {
                return new List<XElement>();
            }
        }
    }
}

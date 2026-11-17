using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Inventory
{
    /// <summary>
    /// Persists the store's flat tax rate to a local XML file so it survives restarts and
    /// doesn't need the database - read once at startup, updated only through TaxConfigForm.
    /// </summary>
    public static class TaxConfig
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "TaxConfig.xml");

        public static decimal TaxRatePercent { get; private set; }

        /// <summary>
        /// The rate actually used for computing and displaying tax. Forced to 0 whenever "Tax
        /// Config" is hidden from the menu (System Config &gt; Menus), so hiding that menu item
        /// fully turns tax off in print and on-screen totals instead of merely blocking access to
        /// the screen that changes the still-configured rate.
        /// </summary>
        public static decimal EffectiveTaxRatePercent =>
            Purchases.IsMenuHidden("taxConfigToolStripMenuItem") ? 0m : TaxRatePercent;

        static TaxConfig()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    TaxRatePercent = 0m;
                    return;
                }

                var doc = XDocument.Load(FilePath);
                string rateText = doc.Root?.Element("TaxRatePercent")?.Value;

                TaxRatePercent = decimal.TryParse(rateText, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate)
                    ? rate
                    : 0m;
            }
            catch
            {
                // Missing/corrupt config just means no tax is applied, not a crash.
                TaxRatePercent = 0m;
            }
        }

        public static void Save(decimal ratePercent)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            var doc = new XDocument(
                new XElement("TaxConfig",
                    new XElement("TaxRatePercent", ratePercent.ToString(CultureInfo.InvariantCulture))));
            doc.Save(FilePath);

            TaxRatePercent = ratePercent;
        }

        public static decimal ComputeTax(decimal subtotal) =>
            Math.Round(subtotal * (EffectiveTaxRatePercent / 100m), 2, MidpointRounding.AwayFromZero);

        public static decimal ComputeTotalWithTax(decimal subtotal) =>
            subtotal + ComputeTax(subtotal);
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Inventory
{
    /// <summary>
    /// Looks up a sample product photo for an item by barcode, via Open Food Facts' free,
    /// keyless API - a reasonable match for this minimart's catalog since a sample of
    /// items_.serial showed the large majority are real EAN-13/UPC barcodes rather than
    /// internal SKU codes. On-demand only (called from AddItemForm when a barcode is entered),
    /// never bulk-prefetched: most items are never reopened for editing, so there's no reason
    /// to hit the API for the whole catalog up front. Downloaded photos are cached to disk
    /// under OfflineCache so a repeat lookup (or offline use afterward) never re-hits the
    /// network, matching this app's offline-resilience convention. Fails open (returns null,
    /// meaning "no photo to show") on any error - no barcode match, network down, bad response -
    /// this is a cosmetic convenience and must never block adding/editing an item.
    /// </summary>
    internal static class ItemPhotoService
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        private static string CacheDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "ItemPhotos");

        static ItemPhotoService()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BrewCode-ItemPhotoLookup");
        }

        // GTIN-8/12/13/14 are the common real-world barcode lengths; anything else (internal
        // timestamp-style serials, short PLU codes like loose produce) is skipped without ever
        // making a network call.
        private static bool LooksLikeBarcode(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return false;

            foreach (char c in serial)
                if (!char.IsDigit(c))
                    return false;

            int len = serial.Length;
            return len == 8 || len == 12 || len == 13 || len == 14;
        }

        /// <summary>Returns a local file path to a cached/downloaded photo for this barcode,
        /// or null if none could be found/fetched.</summary>
        public static async Task<string> GetPhotoPathAsync(string serial)
        {
            serial = (serial ?? "").Trim();
            if (!LooksLikeBarcode(serial))
                return null;

            try
            {
                Directory.CreateDirectory(CacheDir);
                string cachedPath = Path.Combine(CacheDir, serial + ".jpg");
                if (File.Exists(cachedPath))
                    return cachedPath;

                string imageUrl = await LookupImageUrlAsync(serial);
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return null;

                byte[] bytes = await _httpClient.GetByteArrayAsync(imageUrl);
                File.WriteAllBytes(cachedPath, bytes);
                return cachedPath;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string> LookupImageUrlAsync(string barcode)
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://world.openfoodfacts.org/api/v2/product/{barcode}.json?fields=image_front_url,image_url"))
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                string json = await response.Content.ReadAsStringAsync();
                JObject root = JObject.Parse(json);

                if (root.Value<int?>("status") != 1)
                    return null;

                JObject product = root.Value<JObject>("product");
                return product?.Value<string>("image_front_url") ?? product?.Value<string>("image_url");
            }
        }
    }
}

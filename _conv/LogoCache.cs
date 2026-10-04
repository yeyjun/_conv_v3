using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YourApp.Data
{
    /// <summary>
    /// Fetches the store logo from its remote URL and keeps a local copy so receipt
    /// printing still has a logo when offline, instead of PictureBox's automatic
    /// ImageLocation loader silently swapping in a broken-image icon. The download only
    /// ever happens once per app session - every caller shares the same in-memory image,
    /// and the derived window icon is reused by every form via <see cref="ApplyTo"/>.
    /// </summary>
    public static class LogoCache
    {
        private const string LogoUrl = "https://drive.google.com/thumbnail?id=1qVDEd-pMDSposIfi8oBN4ceF7XXqdkTu";

        private static string CachePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "logo.png");

        private static readonly object SyncRoot = new object();
        private static Task<Image> _loadTask;

        /// <summary>The logo rendered as a window icon, available once <see cref="LoadAsync"/> (or
        /// <see cref="Preload"/>) has completed. Null if the logo could never be loaded.</summary>
        public static Icon CachedIcon { get; private set; }

        // The app's own bundled icon (Resources\AppLogo.ico, baked into the exe via
        // <ApplicationIcon> - the same icon Explorer/the taskbar show) - guarantees every window
        // gets a title-bar icon immediately, with no dependency on the network. ApplyTo prefers
        // the live store logo once LoadAsync has actually finished, same as before.
        private static readonly Lazy<Icon> FallbackIcon = new Lazy<Icon>(() =>
        {
            try
            {
                return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
                return null;
            }
        });

        /// <summary>Downloads the logo once per app session and hands every caller the same
        /// in-memory image thereafter, refreshing the on-disk cache on success and falling back
        /// to the last cached copy if the download fails. Returns null if neither is available.</summary>
        public static Task<Image> LoadAsync()
        {
            lock (SyncRoot)
            {
                if (_loadTask == null)
                    _loadTask = LoadCoreAsync();
                return _loadTask;
            }
        }

        /// <summary>Sets the form's title-bar/taskbar icon - the downloaded store logo if it's
        /// ready, otherwise the app's own bundled icon so the title bar is never left blank.</summary>
        public static void ApplyTo(Form form)
        {
            Icon icon = CachedIcon ?? FallbackIcon.Value;
            if (icon != null)
                form.Icon = icon;
        }

        private static async Task<Image> LoadCoreAsync()
        {
            Image image;
            try
            {
                using (var client = new WebClient())
                {
                    byte[] bytes = await client.DownloadDataTaskAsync(LogoUrl);
                    TrySaveToCache(bytes);
                    image = FromBytes(bytes);
                }
            }
            catch
            {
                image = TryLoadFromCache();
            }

            if (image != null)
                CachedIcon = TryCreateIcon(image);

            return image;
        }

        // Bitmap.GetHicon() returns a native HICON that GDI+ does not own or clean up, so it must be
        // destroyed explicitly - Icon.Clone() gives us a self-contained Icon before we do that.
        private static Icon TryCreateIcon(Image image)
        {
            try
            {
                using (var bitmap = new Bitmap(image))
                {
                    IntPtr hIcon = bitmap.GetHicon();
                    try
                    {
                        using (var handleIcon = Icon.FromHandle(hIcon))
                        {
                            return (Icon)handleIcon.Clone();
                        }
                    }
                    finally
                    {
                        NativeMethods.DestroyIcon(hIcon);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);
        }

        private static void TrySaveToCache(byte[] bytes)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CachePath));
                File.WriteAllBytes(CachePath, bytes);
            }
            catch
            {
                // Best-effort only - failing to save the snapshot should never block a successful download.
            }
        }

        private static Image TryLoadFromCache()
        {
            try
            {
                if (!File.Exists(CachePath))
                    return null;

                return FromBytes(File.ReadAllBytes(CachePath));
            }
            catch
            {
                return null;
            }
        }

        // Copies into a fresh Bitmap rather than returning Image.FromStream directly on the
        // MemoryStream - GDI+ keeps the backing stream alive for the Image's lifetime, so
        // returning it straight from a `using` block would leave a disposed stream behind it.
        private static Image FromBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var temp = new Bitmap(ms))
            {
                return new Bitmap(temp);
            }
        }
    }
}

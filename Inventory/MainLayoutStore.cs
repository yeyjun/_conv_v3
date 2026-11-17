using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace YourApp.Data
{
    public static class MainLayoutStore
    {
        private const string EmptyLayoutXml = "<MainLayout />";
        private const int MinControlSize = 10;

        private struct LayoutEntry
        {
            public Point Location;
            public Size? Size;
        }

        public static string FilePath => Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "MainLayout.xml");

        public static string ReadRawXml()
        {
            try
            {
                return File.Exists(FilePath) ? File.ReadAllText(FilePath) : EmptyLayoutXml;
            }
            catch
            {
                return EmptyLayoutXml;
            }
        }

        public static void WriteRawXml(string xml)
        {
            XElement.Parse(xml);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, xml);
        }

        public static void ApplyTo(Control root)
        {
            Dictionary<string, LayoutEntry> saved = Load();
            if (saved.Count == 0)
                return;

            ApplyRecursive(root, saved);
        }

        public static void SaveFrom(Control root)
        {
            var entries = new Dictionary<string, LayoutEntry>();
            CaptureRecursive(root, entries);
            Save(entries);
        }

        private static void ApplyRecursive(Control control, Dictionary<string, LayoutEntry> saved)
        {
            foreach (Control child in control.Controls)
            {
                if (!UsesAutomaticLayout(child) && !string.IsNullOrEmpty(child.Name) &&
                    saved.TryGetValue(child.Name, out LayoutEntry entry))
                {
                    if (entry.Size.HasValue)
                        child.Size = ClampSize(entry.Size.Value);

                    child.Location = ClampLocationToParent(entry.Location, child);
                }

                ApplyRecursive(child, saved);
            }
        }

        internal static bool UsesAutomaticLayout(Control control) =>
            control.Dock != DockStyle.None || control.Anchor != (AnchorStyles.Top | AnchorStyles.Left);

        private static Point ClampLocationToParent(Point location, Control child)
        {
            Control parent = child.Parent;
            if (parent == null || parent.ClientSize.Width <= 0 || parent.ClientSize.Height <= 0)
                return location;

            const int minVisible = 24;
            int maxX = Math.Max(0, parent.ClientSize.Width - minVisible);
            int maxY = Math.Max(0, parent.ClientSize.Height - minVisible);

            int x = Math.Min(Math.Max(location.X, 0), maxX);
            int y = Math.Min(Math.Max(location.Y, 0), maxY);
            return new Point(x, y);
        }

        private static Size ClampSize(Size size) => new Size(
            Math.Max(MinControlSize, size.Width),
            Math.Max(MinControlSize, size.Height));

        private static void CaptureRecursive(Control control, Dictionary<string, LayoutEntry> entries)
        {
            foreach (Control child in control.Controls)
            {
                if (!UsesAutomaticLayout(child) && !string.IsNullOrEmpty(child.Name))
                    entries[child.Name] = new LayoutEntry { Location = child.Location, Size = child.Size };

                CaptureRecursive(child, entries);
            }
        }

        private static Dictionary<string, LayoutEntry> Load()
        {
            var result = new Dictionary<string, LayoutEntry>();
            try
            {
                if (!File.Exists(FilePath))
                    return result;

                XElement root = XElement.Load(FilePath);
                foreach (XElement control in root.Elements("Control"))
                {
                    string name = (string)control.Attribute("name");
                    if (string.IsNullOrEmpty(name))
                        continue;

                    int x = (int?)control.Attribute("x") ?? 0;
                    int y = (int?)control.Attribute("y") ?? 0;

                    int? width = (int?)control.Attribute("width");
                    int? height = (int?)control.Attribute("height");
                    Size? size = (width.HasValue && height.HasValue) ? new Size(width.Value, height.Value) : (Size?)null;

                    result[name] = new LayoutEntry { Location = new Point(x, y), Size = size };
                }
            }
            catch
            {
            }

            return result;
        }

        private static void Save(Dictionary<string, LayoutEntry> entries)
        {
            var root = new XElement("MainLayout",
                entries.Select(p => new XElement("Control",
                    new XAttribute("name", p.Key),
                    new XAttribute("x", p.Value.Location.X),
                    new XAttribute("y", p.Value.Location.Y),
                    p.Value.Size.HasValue ? new XAttribute("width", p.Value.Size.Value.Width) : null,
                    p.Value.Size.HasValue ? new XAttribute("height", p.Value.Size.Value.Height) : null)));

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            root.Save(FilePath);
        }
    }
}

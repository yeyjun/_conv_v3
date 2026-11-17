using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace _conv
{
    /// <summary>
    /// Fonts embedded in the executable so the app renders the same on machines
    /// where they are not installed.
    /// </summary>
    internal static class AppFonts
    {
        private static readonly PrivateFontCollection fonts = new PrivateFontCollection();

        public static FontFamily MorallySerif { get; }

        static AppFonts()
        {
            byte[] data;
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("_conv.Fonts.Morally Serif.otf"))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }

            // GDI+ requires the memory passed to AddMemoryFont to stay allocated for the
            // lifetime of the collection, so this buffer is intentionally never freed.
            IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            fonts.AddMemoryFont(ptr, data.Length);

            MorallySerif = fonts.Families[0];
        }
    }
}

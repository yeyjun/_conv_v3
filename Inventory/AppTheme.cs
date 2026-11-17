using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace YourApp.Data
{
    /// <summary>
    /// Optional accent background/foreground colors, configured on the Appearance tab in System
    /// Configuration (ACCENT_BACKGROUND_COLOR / ACCENT_FOREGROUND_COLOR, stored as "#RRGGBB").
    /// Applied recursively to a form and its controls via <see cref="ApplyTo"/>, called from every
    /// form's constructor the same way as <see cref="LogoCache.ApplyTo"/>. Each control's original
    /// designed color is captured the first time it's themed, so <see cref="ApplyTo"/> can also
    /// revert a control back to its design-time look if the accent gets disabled later.
    /// <see cref="ReapplyToOpenForms"/> lets System Configuration push a change out to every window
    /// already open in the current session instead of only ones opened afterward.
    /// </summary>
    public static class AppTheme
    {
        // Grid/chart/image controls keep their own colors - recoloring them tends to make cell
        // data or chart series unreadable rather than "accented".
        private static readonly Type[] SkipTypes =
        {
            typeof(DataGridView), typeof(Chart), typeof(PictureBox), typeof(ProgressBar)
        };

        private static readonly ConditionalWeakTable<Control, ColorPair> OriginalColors =
            new ConditionalWeakTable<Control, ColorPair>();

        // Controls that must never be recolored regardless of type - e.g. System Configuration's
        // own color-preview swatches, which need to keep showing the picked color.
        private static readonly HashSet<Control> ExemptControls = new HashSet<Control>();

        private sealed class ColorPair
        {
            public Color Back;
            public Color Fore;
        }

        public static Color BackgroundColor { get; private set; }
        public static Color ForegroundColor { get; private set; }
        public static bool IsConfigured { get; private set; }

        static AppTheme()
        {
            Reload();
        }

        /// <summary>Re-reads the accent colors from appSettings. Call after System Configuration
        /// saves new values so forms opened later in the same session pick them up.</summary>
        public static void Reload()
        {
            string bg = ConfigurationManager.AppSettings["ACCENT_BACKGROUND_COLOR"];
            string fg = ConfigurationManager.AppSettings["ACCENT_FOREGROUND_COLOR"];

            IsConfigured = !string.IsNullOrWhiteSpace(bg) || !string.IsNullOrWhiteSpace(fg);
            BackgroundColor = ParseOrDefault(bg, SystemColors.Control);
            ForegroundColor = ParseOrDefault(fg, SystemColors.ControlText);
        }

        /// <summary>Applies the current accent (or reverts to each control's original colors if the
        /// accent isn't configured) to the given form/control and everything inside it.</summary>
        public static void ApplyTo(Control root)
        {
            if (root == null)
                return;

            ApplyRecursive(root);
        }

        /// <summary>Re-themes every window already open in this session - the counterpart to every
        /// new form picking up the theme via <see cref="ApplyTo"/> in its own constructor.</summary>
        public static void ReapplyToOpenForms()
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                ApplyTo(form);
        }

        /// <summary>Marks a control as never themed - for controls whose color IS the data, like a
        /// color-preview swatch, rather than just styling.</summary>
        public static void Exempt(Control control)
        {
            if (control != null)
                ExemptControls.Add(control);
        }

        private static void ApplyRecursive(Control control)
        {
            if (Array.IndexOf(SkipTypes, control.GetType()) < 0 && !ExemptControls.Contains(control))
            {
                ColorPair original = OriginalColors.GetValue(
                    control, c => new ColorPair { Back = c.BackColor, Fore = c.ForeColor });

                control.BackColor = IsConfigured ? BackgroundColor : original.Back;
                control.ForeColor = IsConfigured ? ForegroundColor : original.Fore;
            }

            foreach (Control child in control.Controls)
                ApplyRecursive(child);
        }

        private static Color ParseOrDefault(string html, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(html))
                return fallback;

            try
            {
                return ColorTranslator.FromHtml(html);
            }
            catch
            {
                return fallback;
            }
        }
    }
}

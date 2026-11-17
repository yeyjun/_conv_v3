using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace YourApp.Data
{
    /// <summary>
    /// Optional per-machine font size adjustment, configured on the Appearance tab in System
    /// Configuration (FONT_SCALE_PT - a signed point delta, e.g. "2" makes every control 2pt
    /// bigger than its own designed size, "-1" makes everything 1pt smaller). Applied recursively
    /// to a form and its controls via <see cref="ApplyTo"/>, called from every form's constructor
    /// right after <see cref="AppTheme.ApplyTo"/>. Each control's original designed font size is
    /// captured the first time it's touched, so the delta is always relative to that original size
    /// (not whatever size was last applied), and can be reverted if the setting is cleared.
    /// <see cref="ReapplyToOpenForms"/> lets System Configuration push a change out to every window
    /// already open in the current session instead of only ones opened afterward.
    /// </summary>
    public static class AppFontScale
    {
        // Keeps a large negative/positive delta from shrinking text to unreadable or blowing up
        // layouts entirely, if someone edits App.config directly past what the System Config UI allows.
        private const float MinimumPt = 6f;
        private const float MaximumPt = 72f;

        private static readonly ConditionalWeakTable<Control, FontSizeBox> OriginalSizes =
            new ConditionalWeakTable<Control, FontSizeBox>();

        private sealed class FontSizeBox
        {
            public float Size;
        }

        public static float DeltaPt { get; private set; }
        public static bool IsConfigured { get; private set; }

        static AppFontScale()
        {
            Reload();
        }

        /// <summary>Re-reads FONT_SCALE_PT from appSettings. Call after System Configuration saves
        /// a new value so forms opened later in the same session pick it up.</summary>
        public static void Reload()
        {
            string raw = ConfigurationManager.AppSettings["FONT_SCALE_PT"];
            IsConfigured = float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float delta) && delta != 0f;
            DeltaPt = IsConfigured ? delta : 0f;
        }

        /// <summary>Applies the current delta (or reverts to each control's original font size if
        /// not configured) to the given form/control and everything inside it.</summary>
        public static void ApplyTo(Control root)
        {
            if (root == null)
                return;

            ApplyRecursive(root);
        }

        /// <summary>Re-applies to every window already open in this session - the counterpart to
        /// every new form picking up the setting via <see cref="ApplyTo"/> in its own constructor.</summary>
        public static void ReapplyToOpenForms()
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                ApplyTo(form);
        }

        private static void ApplyRecursive(Control control)
        {
            FontSizeBox original = OriginalSizes.GetValue(control, c => new FontSizeBox { Size = c.Font.Size });

            float targetSize = IsConfigured
                ? Math.Min(Math.Max(original.Size + DeltaPt, MinimumPt), MaximumPt)
                : original.Size;

            if (Math.Abs(control.Font.Size - targetSize) > 0.01f)
                control.Font = new Font(control.Font.FontFamily, targetSize, control.Font.Style);

            foreach (Control child in control.Controls)
                ApplyRecursive(child);
        }
    }
}

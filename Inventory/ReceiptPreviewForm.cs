using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Live preview of the receipt, opened from System Config > Receipt Testing. Renders the
    /// same drawing code the physical printer uses (Purchases.RenderReceiptPreview) onto a
    /// virtual sheet of paper, and re-renders on every change to the price-override slider or
    /// paper-width selector so the effect is visible immediately without an actual print. The
    /// paper's margins are shown as draggable boundary handles directly on that virtual paper -
    /// dragging one previews the new boundary live; releasing it (dropping) re-renders the
    /// receipt with that margin applied. Everything here (margins, paper width, price override)
    /// stays preview-only until Save is clicked, at which point the margins/paper width are
    /// written to App.config and take effect on real printing too - see btnSave_Click.
    /// </summary>
    public partial class ReceiptPreviewForm : Form
    {
        // Panel.DoubleBuffered is protected, not settable from outside a Control subclass - this
        // exists purely to turn it on, so dragging a handle repaints without flicker.
        private sealed class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel() => DoubleBuffered = true;
        }

        private enum DragTarget { None, Left, Right, Top }

        private readonly Purchases _purchases;

        // Index lines up with cbxPaperWidth's items; null means "don't override - use whatever
        // ApplyReceiptPrinterSettings detected from the configured printer".
        private static readonly int?[] PaperWidthMmByComboIndex = { null, 80, 58 };

        private const int HandleRadiusPx = 6;
        private const int HandleYPx = 16;
        private const int MinContentWidthPx = 60;
        private const int MaxTopMarginPx = 200;

        // Null until the tester drags a handle - before that, RenderReceiptPreview's own
        // detected/configured margins are used.
        private int? _marginLeftOverride;
        private int? _marginRightOverride;
        private int? _marginTopOverride;

        // The most recently rendered margins/page width, in preview pixels - what the boundary
        // handles are drawn at when nothing is actively being dragged.
        private int _pageWidthPx;
        private int _marginLeftPx;
        private int _marginRightPx;
        private int _marginTopPx;

        private DragTarget _activeDrag = DragTarget.None;
        private float _dragValuePx;

        public ReceiptPreviewForm(Purchases purchases)
        {
            InitializeComponent();
            _purchases = purchases;
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private void ReceiptPreviewForm_Load(object sender, EventArgs e)
        {
            chkPriceOverride.Checked = Purchases.TestPriceOverride.HasValue;
            tbPriceOverride.Value = Math.Min(Math.Max((int)(Purchases.TestPriceOverride ?? 1m), tbPriceOverride.Minimum), tbPriceOverride.Maximum);

            int configuredWidthMm = int.TryParse(ConfigurationManager.AppSettings["RECEIPT_PAPER_WIDTH_MM"], out int mm) ? mm : 58;
            int widthIndex = Array.IndexOf(PaperWidthMmByComboIndex, configuredWidthMm == 0 ? (int?)null : configuredWidthMm);
            cbxPaperWidth.SelectedIndex = widthIndex >= 0 ? widthIndex : Array.IndexOf(PaperWidthMmByComboIndex, (int?)58);
            chkPaperWidthIsDefault.Checked = false;

            UpdatePriceOverrideState();
            RefreshPreview();
        }

        private void UpdatePriceOverrideState()
        {
            tbPriceOverride.Enabled = chkPriceOverride.Checked;
            lblPriceOverrideValue.Text = $"Override price: {tbPriceOverride.Value:N0}.00";
            Purchases.TestPriceOverride = chkPriceOverride.Checked ? (decimal?)tbPriceOverride.Value : null;
        }

        private void RefreshPreview()
        {
            Image previousImage = pnlPaper.BackgroundImage;
            pnlPaper.BackgroundImage = null;
            previousImage?.Dispose();

            int? widthMm = cbxPaperWidth.SelectedIndex >= 0 ? PaperWidthMmByComboIndex[cbxPaperWidth.SelectedIndex] : null;
            Purchases.ReceiptPreviewResult result = _purchases.RenderReceiptPreview(
                widthMm, _marginLeftOverride, _marginRightOverride, _marginTopOverride);

            _pageWidthPx = result.PageWidthPx;
            _marginLeftPx = result.MarginLeftPx;
            _marginRightPx = result.MarginRightPx;
            _marginTopPx = result.MarginTopPx;

            pnlPaper.Size = result.Image.Size;
            pnlPaper.BackgroundImage = result.Image;
            pnlPaper.Invalidate();
        }

        private void chkPriceOverride_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePriceOverrideState();
            RefreshPreview();
        }

        private void tbPriceOverride_Scroll(object sender, EventArgs e)
        {
            UpdatePriceOverrideState();
            RefreshPreview();
        }

        private void cbxPaperWidth_SelectedIndexChanged(object sender, EventArgs e) => RefreshPreview();

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshPreview();

        /// <summary>
        /// Writes whatever margins were actually dragged (left blank/"Automatic" for any handle
        /// never touched this session, so it stays on the printer-driver/hard-margin default
        /// instead of getting frozen at today's reading) plus the paper-width selection to
        /// App.config, so real checkout printing - not just this preview - uses them from now on.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                SetSetting(config, "RECEIPT_MARGIN_LEFT_MM", MarginOverrideToMmString(_marginLeftOverride));
                SetSetting(config, "RECEIPT_MARGIN_RIGHT_MM", MarginOverrideToMmString(_marginRightOverride));
                SetSetting(config, "RECEIPT_MARGIN_TOP_MM", MarginOverrideToMmString(_marginTopOverride));

                if (chkPaperWidthIsDefault.Checked)
                {
                    int? paperWidthMm = cbxPaperWidth.SelectedIndex >= 0 ? PaperWidthMmByComboIndex[cbxPaperWidth.SelectedIndex] : null;
                    SetSetting(config, "RECEIPT_PAPER_WIDTH_MM", paperWidthMm.HasValue ? paperWidthMm.Value.ToString(CultureInfo.InvariantCulture) : "");
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                lblSaveStatus.ForeColor = Color.Teal;
                lblSaveStatus.Text = "Saved - real printing now uses these margins/paper width too.";
            }
            catch (Exception ex)
            {
                lblSaveStatus.ForeColor = Color.Tomato;
                lblSaveStatus.Text = "Failed to save: " + ex.Message;
            }
        }

        private static string MarginOverrideToMmString(int? marginHundredthsOverride) =>
            marginHundredthsOverride.HasValue
                ? Purchases.HundredthsToMm(marginHundredthsOverride.Value).ToString("0.0", CultureInfo.InvariantCulture)
                : "";

        private static void SetSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void ReceiptPreviewForm_FormClosed(object sender, FormClosedEventArgs e) =>
            pnlPaper.BackgroundImage?.Dispose();

        private PointF LeftHandlePos => new PointF(_activeDrag == DragTarget.Left ? _dragValuePx : _marginLeftPx, HandleYPx);
        private PointF RightHandlePos => new PointF(_pageWidthPx - (_activeDrag == DragTarget.Right ? _dragValuePx : _marginRightPx), HandleYPx);
        private PointF TopHandlePos => new PointF(_pageWidthPx / 2f, _activeDrag == DragTarget.Top ? _dragValuePx : _marginTopPx);

        private void pnlPaper_Paint(object sender, PaintEventArgs e)
        {
            float leftPx = _activeDrag == DragTarget.Left ? _dragValuePx : _marginLeftPx;
            float rightEdgePx = _pageWidthPx - (_activeDrag == DragTarget.Right ? _dragValuePx : _marginRightPx);
            float topPx = _activeDrag == DragTarget.Top ? _dragValuePx : _marginTopPx;

            using (var guidePen = new Pen(Color.DodgerBlue, 1f) { DashStyle = DashStyle.Dash })
            {
                e.Graphics.DrawLine(guidePen, leftPx, 0, leftPx, pnlPaper.Height);
                e.Graphics.DrawLine(guidePen, rightEdgePx, 0, rightEdgePx, pnlPaper.Height);
                e.Graphics.DrawLine(guidePen, 0, topPx, pnlPaper.Width, topPx);
            }

            DrawHandle(e.Graphics, Font, LeftHandlePos, PxToMm(leftPx));
            DrawHandle(e.Graphics, Font, RightHandlePos, PxToMm(_pageWidthPx - rightEdgePx));
            DrawHandle(e.Graphics, Font, TopHandlePos, PxToMm(topPx));
        }

        private static void DrawHandle(Graphics g, Font font, PointF center, double mm)
        {
            var dot = new RectangleF(center.X - HandleRadiusPx, center.Y - HandleRadiusPx, HandleRadiusPx * 2, HandleRadiusPx * 2);
            g.FillEllipse(Brushes.DodgerBlue, dot);
            g.DrawEllipse(Pens.White, dot);

            string label = $"{mm:0.0}mm";
            SizeF labelSize = g.MeasureString(label, font);
            g.FillRectangle(Brushes.DodgerBlue, center.X + HandleRadiusPx + 2, center.Y - labelSize.Height / 2, labelSize.Width + 4, labelSize.Height);
            g.DrawString(label, font, Brushes.White, center.X + HandleRadiusPx + 4, center.Y - labelSize.Height / 2);
        }

        private static double PxToMm(float px) => px / Purchases.ReceiptPreviewDpi * 25.4;

        private static bool Near(Point p, PointF target) =>
            Math.Abs(p.X - target.X) <= HandleRadiusPx + 3 && Math.Abs(p.Y - target.Y) <= HandleRadiusPx + 3;

        private void pnlPaper_MouseDown(object sender, MouseEventArgs e)
        {
            if (Near(e.Location, LeftHandlePos))
            {
                _activeDrag = DragTarget.Left;
                _dragValuePx = _marginLeftPx;
            }
            else if (Near(e.Location, RightHandlePos))
            {
                _activeDrag = DragTarget.Right;
                _dragValuePx = _marginRightPx;
            }
            else if (Near(e.Location, TopHandlePos))
            {
                _activeDrag = DragTarget.Top;
                _dragValuePx = _marginTopPx;
            }
        }

        private void pnlPaper_MouseMove(object sender, MouseEventArgs e)
        {
            if (_activeDrag == DragTarget.None)
            {
                bool overHandle = Near(e.Location, LeftHandlePos) || Near(e.Location, RightHandlePos) || Near(e.Location, TopHandlePos);
                Cursor = overHandle ? Cursors.Hand : Cursors.Default;
                return;
            }

            switch (_activeDrag)
            {
                case DragTarget.Left:
                    _dragValuePx = Clamp(e.X, 0, _pageWidthPx - _marginRightPx - MinContentWidthPx);
                    break;
                case DragTarget.Right:
                    _dragValuePx = Clamp(_pageWidthPx - e.X, 0, _pageWidthPx - _marginLeftPx - MinContentWidthPx);
                    break;
                case DragTarget.Top:
                    _dragValuePx = Clamp(e.Y, 0, MaxTopMarginPx);
                    break;
            }

            pnlPaper.Invalidate();
        }

        private void pnlPaper_MouseUp(object sender, MouseEventArgs e)
        {
            if (_activeDrag == DragTarget.None)
                return;

            int hundredths = (int)Math.Round(_dragValuePx / Purchases.ReceiptPreviewDpi * 100);
            switch (_activeDrag)
            {
                case DragTarget.Left:
                    _marginLeftOverride = hundredths;
                    break;
                case DragTarget.Right:
                    _marginRightOverride = hundredths;
                    break;
                case DragTarget.Top:
                    _marginTopOverride = hundredths;
                    break;
            }

            _activeDrag = DragTarget.None;
            RefreshPreview();
        }

        private static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
    }
}

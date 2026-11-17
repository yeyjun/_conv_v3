using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
{
    /// <summary>
    /// Startup splash shown while <see cref="DataCacheManager.LoadAll"/> and the store
    /// logo download run, so the app never appears to hang on a slow/offline connection.
    /// Runs its own message loop via Application.Run(splash) - closes itself once loading
    /// finishes (or fails), handing control back to Program.Main to open the real app.
    /// </summary>
    public partial class SplashForm : Form
    {
        /// <summary>Set if startup loading threw. Program.Main checks this after the splash closes.</summary>
        public Exception LoadError { get; private set; }

        public SplashForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            ApplyBrandFont();
            lblTitle.Text = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";
        }

        private void ApplyBrandFont()
        {
            try
            {
                lblTitle.Font = new Font(AppFonts.MorallySerif, 22F, FontStyle.Regular);
                lblTitle.UseCompatibleTextRendering = true;
            }
            catch
            {
                // Embedded font failed to load - the designer's default Segoe UI title is fine.
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            using (GraphicsPath path = RoundedRectPath(ClientRectangle, 18))
                Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle borderRect = Rectangle.Inflate(ClientRectangle, -1, -1);
            using (GraphicsPath path = RoundedRectPath(borderRect, 17))
            using (var pen = new Pen(Color.FromArgb(255, 215, 140), 1.5f))
                e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedRectPath(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            IProgress<string> progress = new Progress<string>(status => lblStatus.Text = status);

            try
            {
                await Task.Run(() => DataCacheManager.Instance.LoadAll(status => progress.Report(status)));

                progress.Report("Loading store branding...");
                Image logo = await LogoCache.LoadAsync();
                if (logo != null)
                {
                    picLogo.Image = logo;
                    picLogo.Visible = true;
                }

                progress.Report("Ready.");
            }
            catch (Exception ex)
            {
                LoadError = ex;
            }
            finally
            {
                Close();
            }
        }
    }
}

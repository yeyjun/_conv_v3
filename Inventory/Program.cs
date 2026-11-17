using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;
namespace _conv
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Last-resort safety net: without this, any exception that slips past a form's own
            // try/catch (e.g. a cancelled/timed-out DB or network call surfacing as a raw
            // OperationCanceledException/TaskCanceledException) shows .NET's default unhandled-
            // exception dialog and can tear the app down mid-sale - the opposite of the offline-
            // resilience goal everywhere else in this app.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => HandleUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleUnhandledException(e.ExceptionObject as Exception);

            using (var splash = new SplashForm())
            {
                Application.Run(splash); // loads DataCacheManager + the logo, then closes itself

                if (splash.LoadError != null)
                {
                    MessageBox.Show($"Failed to load startup data:\n{splash.LoadError.Message}",
                        "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (DataCacheManager.Instance.IsOfflineMode)
            {
                MessageBox.Show(
                    "Could not reach the database. Showing a local stock snapshot saved on " +
                    $"{DataCacheManager.Instance.OfflineSnapshotAsOf:g}.\n\n" +
                    "Prices and quantities may be outdated, and sales/inventory changes will not be saved until the connection is restored.",
                    "Offline Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!Purchases.IsReceiptPrinterConfigured())
            {
                MessageBox.Show(
                    "No receipt printer is set up yet (or the one configured isn't installed on this machine).\n\n" +
                    "You'll be shown the available printers next so you can pick one now, or set it later from System Config > Printer.",
                    "Receipt Printer Not Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                using (var selectPrinterForm = new SelectPrinterForm())
                    selectPrinterForm.ShowDialog();
            }

            TrialLicense.EnsureTrialStarted();
            if (TrialLicense.IsExpired)
            {
                using (var trialForm = new TrialExpiredForm())
                {
                    if (trialForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            (bool maintenanceOn, string maintenanceMessage) = AppConfigService.GetMaintenanceStatusAsync().GetAwaiter().GetResult();
            if (maintenanceOn)
            {
                using (var maintenanceForm = new MaintenanceForm(maintenanceMessage))
                {
                    if (maintenanceForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            if (!SubscriptionTokenService.IsActiveAsync().GetAwaiter().GetResult())
            {
                using (var subscriptionForm = new SubscriptionExpiredForm())
                {
                    if (subscriptionForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            _ = Task.Run(() => ScheduledReportService.CheckAndSendAsync());

            RemoteControlServer.Restart();
            try
            {
                Application.Run(new Purchases());
            }
            finally
            {
                RemoteControlServer.Stop();
            }
        }

        /// <summary>
        /// Catches whatever an individual form's own try/catch didn't - notably a cancelled or
        /// timed-out background operation (OperationCanceledException/TaskCanceledException),
        /// which is an expected hiccup on a flaky connection, not a bug. Logged the same way as
        /// any other activity event (falls back to the offline log file if the DB is down) and
        /// shown to the user in plain language instead of a raw .NET exception dialog.
        /// </summary>
        private static void HandleUnhandledException(Exception ex)
        {
            if (ex == null)
                return;

            bool isCancellation = ex is OperationCanceledException;

            string message = isCancellation
                ? "A background operation (e.g. a network or database call) timed out or was cancelled. You can keep using the app."
                : $"An unexpected error occurred and was recovered from:\n{ex.Message}";

            MessageBox.Show(message, isCancellation ? "Operation Cancelled" : "Unexpected Error",
                MessageBoxButtons.OK, isCancellation ? MessageBoxIcon.Warning : MessageBoxIcon.Error);

            _ = ActivityLog.LogAsync($"Unhandled {(isCancellation ? "cancellation" : "exception")}: {ex}");
        }
    }
}

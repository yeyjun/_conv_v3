using Hangfire;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;
namespace Inventory
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

            if (!TrialLicense.IsRegistered)
            {
                using (var registrationForm = new RegistrationForm())
                {
                    if (registrationForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            using (var splash = new SplashForm())
            {
                Application.Run(splash); // loads DataCacheManager + the logo, then closes itself

                if (splash.LoadError != null)
                {
                    ErrorDialog.Show(null, "Failed to load startup data. The application cannot continue.",
                        "Startup Error", splash.LoadError);
                    return;
                }
            }

            if (DataCacheManager.Instance.IsOfflineMode)
            {
                ErrorDialog.Show(null,
                    "Could not reach the database. Showing a local stock snapshot saved on " +
                    $"{DataCacheManager.Instance.OfflineSnapshotAsOf:g}.\n\n" +
                    "Prices and quantities may be outdated, and sales/inventory changes will not be saved until the connection is restored.",
                    "Offline Mode", $"DB_SERVER={ConfigurationManager.AppSettings["DB_SERVER"]}\nSnapshot timestamp: {DataCacheManager.Instance.OfflineSnapshotAsOf:O}",
                    MessageBoxIcon.Warning);
            }

            if (!Purchases.IsReceiptPrinterConfigured())
            {
                ErrorDialog.Show(null,
                    "No receipt printer is set up yet (or the one configured isn't installed on this machine).\n\n" +
                    "You'll be shown the available printers next so you can pick one now, or set it later from System Config > Printer.",
                    "Receipt Printer Not Set", "No printer name found in App.config / Windows default printer.", MessageBoxIcon.Warning);

                using (var selectPrinterForm = new SelectPrinterForm())
                {
                    if (selectPrinterForm.ShowDialog() == DialogResult.OK)
                    {
                        if (ConfirmDialog.Show(null,
                            $"Printer \"{selectPrinterForm.SelectedPrinterName}\" is now set up.\n\n" +
                            "Print a test page now to confirm it prints correctly? This prints the longest item name in your catalog, the hardest case for the receipt layout to fit.",
                            "Print Test Page?") == DialogResult.Yes)
                        {
                            try
                            {
                                SelectPrinterForm.PrintTestPage();
                            }
                            catch (Exception ex)
                            {
                                ErrorDialog.Show(null,
                                    "Could not print the test page. You can try again later from System Config > Printer.",
                                    "Test Print Failed", ex);
                            }
                        }
                    }
                }
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

            Task<(bool Enabled, string Message)> maintenanceStatusTask = AppConfigService.GetMaintenanceStatusAsync();
            Task<bool> subscriptionActiveTask = SubscriptionTokenService.IsActiveAsync();
            Task.WhenAll(maintenanceStatusTask, subscriptionActiveTask).GetAwaiter().GetResult();

            (bool maintenanceOn, string maintenanceMessage) = maintenanceStatusTask.Result;
            if (maintenanceOn)
            {
                using (var maintenanceForm = new MaintenanceForm(maintenanceMessage))
                {
                    if (maintenanceForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            if (!subscriptionActiveTask.Result)
            {
                using (var subscriptionForm = new SubscriptionExpiredForm())
                {
                    if (subscriptionForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            _ = Task.Run(() => ScheduledReportService.CheckAndSendAsync());
            _ = Task.Run(() => ScheduledInventoryPrintService.CheckAndPrintAsync());

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

            if (isCancellation)
            {
                ErrorDialog.Show(null,
                    "A background operation (e.g. a network or database call) timed out or was cancelled. You can keep using the app.",
                    "Operation Cancelled", ex, MessageBoxIcon.Warning);
            }
            else
            {
                ErrorDialog.Show(null, "An unexpected error occurred and was recovered from. You can keep using the app.",
                    "Unexpected Error", ex);
            }

            string logText = $"Unhandled {(isCancellation ? "cancellation" : "exception")}: {ex}";
            _ = ActivityLog.LogAsync(logText);
            OperationLog.RecordAction(nameof(Program), logText);
        }
    }
}

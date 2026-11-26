using System;
using System.Configuration;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Shared by OkDialog/ConfirmDialog/ErrorDialog: counts down to each dialog's recommended
    /// action (its AcceptButton) and shows the remaining seconds on a label rather than
    /// dismissing silently, then performs a real click on that button - not just setting
    /// form.DialogResult directly - so any Click handlers wired to it still run. Reuses the
    /// same ALERT_AUTO_DISMISS_ENABLED/ALERT_AUTO_DISMISS_SECONDS appSettings (System Config)
    /// that already gated OkDialog/ConfirmDialog's prior silent timers.
    /// </summary>
    internal static class AutoDismissCountdown
    {
        public static bool Enabled =>
            !string.Equals(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

        public static int Seconds =>
            int.TryParse(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_SECONDS"], out int s) && s > 0 ? s : 5;

        /// <summary>Starts the visible countdown on <paramref name="countdownLabel"/>, auto-clicking
        /// <paramref name="recommendedButton"/> when it reaches zero. No-ops (and hides the label)
        /// when auto-dismiss is turned off in System Config.</summary>
        public static void Attach(Form form, Button recommendedButton, Label countdownLabel)
        {
            if (!Enabled)
            {
                countdownLabel.Visible = false;
                return;
            }

            int remaining = Seconds;
            string action = recommendedButton.Text.Replace("&", "");

            void UpdateText() => countdownLabel.Text = $"Auto-selecting \"{action}\" in {remaining}s...";
            UpdateText();

            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                remaining--;
                if (remaining <= 0)
                {
                    timer.Stop();
                    ((IButtonControl)recommendedButton).PerformClick();
                }
                else
                {
                    UpdateText();
                }
            };
            timer.Start();
            form.FormClosed += (s, e) => timer.Dispose();
        }
    }
}

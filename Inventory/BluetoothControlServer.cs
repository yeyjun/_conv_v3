using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Configuration;
using System.Threading;

namespace Inventory
{
    /// <summary>
    /// Bluetooth RFCOMM counterpart to RemoteControlServer's TCP listener - lets RemoteController
    /// talk to this terminal over a direct device-to-device Bluetooth connection instead of the
    /// network, useful when there's no shared Wi-Fi/LAN (or the internet is down) but both machines
    /// are physically nearby. Off by default (REMOTE_CONTROL_BLUETOOTH_ENABLED) and gated by the
    /// same REMOTE_CONTROL_TOKEN as TCP; every accepted connection is handed to
    /// RemoteControlServer.HandleRequestOverStream, so it's the exact same command set and
    /// auth/lockout logic - only how the bytes get here differs.
    ///
    /// Not TLS-wrapped like the TCP path - RFCOMM connections require the two devices to already
    /// be paired at the OS level (Windows Settings > Bluetooth), which is its own authentication
    /// step (a PIN/passkey exchange during pairing) with a fundamentally different trust model than
    /// "reachable from the internet."
    /// </summary>
    internal static class BluetoothControlServer
    {
        // Fixed, app-specific RFCOMM service identifier - RemoteController's BluetoothClient.Connect
        // must use this exact same GUID to find this service on the paired device. Not a secret
        // (SDP service records are discoverable), just an address - REMOTE_CONTROL_TOKEN is what
        // actually authenticates a request once connected.
        internal static readonly Guid ServiceGuid = new Guid("6f1b2c3d-4e5f-4a6b-8c7d-9e0f1a2b3c4d");

        private static BluetoothListener _listener;
        private static Thread _acceptThread;
        private static volatile bool _stopping;

        public static bool IsRunning { get; private set; }
        public static string LastError { get; private set; }

        public static bool IsEnabled =>
            bool.TryParse(ConfigurationManager.AppSettings["REMOTE_CONTROL_BLUETOOTH_ENABLED"], out bool enabled) && enabled;

        /// <summary>
        /// This machine's Bluetooth radio name/address, or null if there's no radio present (or it's
        /// off) - shown on System Config > Remote Control the same way the LAN IP list is, so an
        /// admin knows what to look for when pairing RemoteController's machine to this one.
        /// </summary>
        public static (string Name, string Address)? GetLocalRadioInfo()
        {
            try
            {
                BluetoothRadio radio = BluetoothRadio.Default;
                if (radio == null)
                    return null;
                return (radio.Name, radio.LocalAddress.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Starts the listener if enabled, a token is configured, and a Bluetooth radio is
        /// available. Safe to call more than once. Failure (no radio, radio off, etc.) is recorded
        /// in LastError rather than thrown - the TCP transport works independently either way.</summary>
        public static void Restart()
        {
            Stop();

            if (!IsEnabled)
                return;

            string token = ConfigurationManager.AppSettings["REMOTE_CONTROL_TOKEN"] ?? "";
            if (string.IsNullOrWhiteSpace(token))
                return;

            _stopping = false;

            try
            {
                _listener = new BluetoothListener(ServiceGuid) { ServiceName = "App Inventory Remote Control" };
                _listener.Start();
                IsRunning = true;
                LastError = null;

                _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "RemoteControlServer-Bluetooth" };
                _acceptThread.Start();
            }
            catch (Exception ex)
            {
                IsRunning = false;
                LastError = ex.Message;
                _listener = null;
            }
        }

        public static void Stop()
        {
            _stopping = true;
            IsRunning = false;
            try
            {
                _listener?.Stop();
            }
            catch
            {
                // Best-effort only.
            }
            _listener = null;
        }

        private static void AcceptLoop()
        {
            while (!_stopping)
            {
                BluetoothClient client;
                try
                {
                    client = _listener.AcceptBluetoothClient();
                }
                catch
                {
                    // Listener was stopped (or failed) - exit quietly either way.
                    break;
                }

                ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
            }
        }

        private static void HandleClient(BluetoothClient client)
        {
            string remoteId = "bt:?";
            try
            {
                remoteId = "bt:" + ((BluetoothEndPoint)client.Client.RemoteEndPoint).Address;

                using (client)
                using (System.Net.Sockets.NetworkStream stream = client.GetStream())
                {
                    RemoteControlServer.HandleRequestOverStream(stream, remoteId);
                }
            }
            catch
            {
                // Dropped/malformed connection - not this app's problem.
            }
        }
    }
}

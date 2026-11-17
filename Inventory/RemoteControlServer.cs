using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Optional, off-by-default listener that lets the standalone RemoteController companion app
    /// (a separate project - see RemoteController.sln entry, no reference to this project) talk to
    /// a running instance of this app: a health/status check, remote menu control, and pushing a
    /// license key to activate remotely instead of someone typing it in on-site. Reachable over
    /// TCP (LAN or, with port forwarding, the public internet) and, separately, Bluetooth RFCOMM
    /// for a direct device-to-device connection with no network involved at all.
    ///
    /// Deliberately minimal and non-destructive - there is no command here that deletes data,
    /// changes prices, or touches a sale. Every request must present REMOTE_CONTROL_TOKEN
    /// (System Config > Remote Control) or it's rejected; with no token configured neither listener
    /// starts at all. The TCP listener is TLS-encrypted (self-signed cert, see
    /// RemoteControlCertificate.cs) since it may be reachable over the open internet; the Bluetooth
    /// listener isn't (RFCOMM already requires OS-level device pairing, a different trust model).
    /// A per-remote-address lockout after repeated bad tokens covers both transports.
    ///
    /// Protocol: one line per request, one connection per request (no session state).
    ///   Request:  {token}|{COMMAND}[|{arg}]\n
    ///   Response: OK|...\n   or   ERR|{reason}\n
    /// Commands: PING, STATUS, ACTIVATE|{licenseKey}, GET_MENUS, SET_MENUS|{comma-separated hidden menu names}
    /// </summary>
    internal static class RemoteControlServer
    {
        private static TcpListener _tcpListener;
        private static Thread _tcpAcceptThread;
        private static volatile bool _stopping;

        public static bool IsRunning { get; private set; }
        public static string LastError { get; private set; }

        public static bool IsEnabled =>
            bool.TryParse(ConfigurationManager.AppSettings["REMOTE_CONTROL_ENABLED"], out bool enabled) && enabled;

        public static int Port =>
            int.TryParse(ConfigurationManager.AppSettings["REMOTE_CONTROL_PORT"], out int port) && port > 0 && port <= 65535
                ? port
                : 51820;

        private static string ConfiguredToken => ConfigurationManager.AppSettings["REMOTE_CONTROL_TOKEN"] ?? "";

        /// <summary>The self-signed cert's thumbprint - shown on System Config > Remote Control so
        /// it can be handed to whoever configures RemoteController to pin (trust-on-first-use).</summary>
        public static string CertificateThumbprint
        {
            get
            {
                try
                {
                    return RemoteControlCertificate.GetThumbprint();
                }
                catch (Exception ex)
                {
                    return $"(unavailable: {ex.Message})";
                }
            }
        }

        private static readonly HttpClient _publicIpHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        /// <summary>
        /// This machine's internet-facing IPv4 address, as seen from outside - a machine behind a
        /// router/NAT has no way to know this from its own network adapters (GetLocalIPv4Addresses
        /// only sees the LAN-side address), so it's asked of a public echo service instead. Shown
        /// on System Config > Remote Control for the "connect over the public internet" case; still
        /// requires the router to be configured to forward the configured port to this machine,
        /// which is outside anything this app can do.
        /// </summary>
        public static async Task<string> GetPublicIPv4Async()
        {
            try
            {
                string ip = await _publicIpHttpClient.GetStringAsync("https://api.ipify.org");
                return ip.Trim();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This machine's LAN-facing IPv4 addresses - shown on System Config > Remote Control so
        /// whoever is running the RemoteController companion app knows what to point it at. Only
        /// active, non-loopback adapters are considered (skips disabled/disconnected NICs), since
        /// an address on a NIC that's down isn't reachable anyway.
        /// </summary>
        public static string[] GetLocalIPv4Addresses()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                  nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .SelectMany(nic => nic.GetIPProperties().UnicastAddresses)
                    .Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(addr => addr.Address.ToString())
                    .Distinct()
                    .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>Starts the listener(s) if enabled and a token is configured. Safe to call more
        /// than once - restarts if the port or token changed since the last call.</summary>
        public static void Restart()
        {
            Stop();

            if (!IsEnabled || string.IsNullOrWhiteSpace(ConfiguredToken))
                return;

            _stopping = false;

            try
            {
                // Touches disk (and generates the cert on first ever run) before Start() so a
                // slow/failed cert load surfaces as LastError instead of a silent dead listener.
                RemoteControlCertificate.GetOrCreate();

                _tcpListener = new TcpListener(IPAddress.Any, Port);
                _tcpListener.Start();
                IsRunning = true;
                LastError = null;

                _tcpAcceptThread = new Thread(TcpAcceptLoop) { IsBackground = true, Name = "RemoteControlServer-Tcp" };
                _tcpAcceptThread.Start();
            }
            catch (Exception ex)
            {
                // Never let a bad port/permission/cert issue take the main app down with it.
                IsRunning = false;
                LastError = ex.Message;
                _tcpListener = null;
            }

            BluetoothControlServer.Restart();
        }

        public static void Stop()
        {
            _stopping = true;
            IsRunning = false;
            try
            {
                _tcpListener?.Stop();
            }
            catch
            {
                // Best-effort only.
            }
            _tcpListener = null;

            BluetoothControlServer.Stop();
        }

        private static void TcpAcceptLoop()
        {
            while (!_stopping)
            {
                TcpClient client;
                try
                {
                    client = _tcpListener.AcceptTcpClient();
                }
                catch
                {
                    // Listener was stopped (or failed) - exit quietly either way.
                    break;
                }

                ThreadPool.QueueUserWorkItem(_ => HandleTcpClient(client));
            }
        }

        private static void HandleTcpClient(TcpClient client)
        {
            string remoteId = "tcp:?";
            try
            {
                remoteId = "tcp:" + ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                if (IsLockedOut(remoteId))
                    return;

                using (client)
                using (NetworkStream rawStream = client.GetStream())
                using (var sslStream = new SslStream(rawStream, leaveInnerStreamOpen: false))
                {
                    X509Certificate2 cert = RemoteControlCertificate.GetOrCreate();
                    // SslProtocols.None lets the OS pick the strongest protocol both sides support,
                    // rather than hardcoding a version this machine's Windows/.NET build might not
                    // actually offer.
                    sslStream.AuthenticateAsServer(cert, clientCertificateRequired: false,
                        enabledSslProtocols: SslProtocols.None, checkCertificateRevocation: false);

                    HandleRequestOverStream(sslStream, remoteId);
                }
            }
            catch
            {
                // TLS handshake failure, dropped connection, malformed request - not this app's problem.
            }
        }

        /// <summary>Shared by both transports: read one line, dispatch it, write one line back.</summary>
        internal static void HandleRequestOverStream(Stream stream, string remoteId)
        {
            stream.ReadTimeout = 5000;
            stream.WriteTimeout = 5000;

            string line = ReadLine(stream);
            string response = HandleRequest(line, remoteId);

            byte[] bytes = Encoding.UTF8.GetBytes(response + "\n");
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string ReadLine(Stream stream)
        {
            var sb = new StringBuilder();
            int b;
            // Line-by-line, capped so a client can't hold a connection open streaming garbage forever.
            while (sb.Length < 4096 && (b = stream.ReadByte()) != -1 && b != '\n')
            {
                if (b != '\r')
                    sb.Append((char)b);
            }
            return sb.ToString();
        }

        // Locks out a remote address (IP or Bluetooth address) after repeated bad tokens - the
        // listener may be reachable from the open internet now, where a stranger guessing at the
        // token is a real scenario a LAN-only tool didn't have to worry about.
        private const int MaxFailuresBeforeLockout = 5;
        private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(5);
        private static readonly ConcurrentDictionary<string, FailureInfo> _failuresByRemote =
            new ConcurrentDictionary<string, FailureInfo>();

        private sealed class FailureInfo
        {
            public int Count;
            public DateTime WindowStart = DateTime.UtcNow;
            public DateTime? LockedUntil;
        }

        private static bool IsLockedOut(string remoteId) =>
            _failuresByRemote.TryGetValue(remoteId, out FailureInfo info) &&
            info.LockedUntil.HasValue && DateTime.UtcNow < info.LockedUntil.Value;

        private static void RecordAuthFailure(string remoteId)
        {
            FailureInfo info = _failuresByRemote.GetOrAdd(remoteId, _ => new FailureInfo());
            lock (info)
            {
                if (DateTime.UtcNow - info.WindowStart > LockoutWindow)
                {
                    info.Count = 0;
                    info.WindowStart = DateTime.UtcNow;
                    info.LockedUntil = null;
                }

                info.Count++;
                if (info.Count >= MaxFailuresBeforeLockout)
                    info.LockedUntil = DateTime.UtcNow.Add(LockoutWindow);
            }
        }

        private static void RecordAuthSuccess(string remoteId) => _failuresByRemote.TryRemove(remoteId, out _);

        private static string HandleRequest(string line, string remoteId)
        {
            string[] parts = (line ?? "").Split('|');
            if (parts.Length < 2)
                return "ERR|BAD_REQUEST";

            string token = parts[0];
            string command = parts[1].Trim().ToUpperInvariant();

            string expectedToken = ConfiguredToken;
            if (string.IsNullOrEmpty(expectedToken) || !string.Equals(token, expectedToken, StringComparison.Ordinal))
            {
                RecordAuthFailure(remoteId);
                return "ERR|UNAUTHORIZED";
            }

            RecordAuthSuccess(remoteId);

            switch (command)
            {
                case "PING":
                    string storeName = ConfigurationManager.AppSettings["STORE_NAME"] ?? "";
                    string storeCode = ConfigurationManager.AppSettings["STORE_CODE"] ?? "";
                    return $"OK|PONG|{storeName}|{storeCode}";

                case "STATUS":
                    bool offline = YourApp.Data.DataCacheManager.Instance.IsOfflineMode;
                    return "OK|STATUS|" +
                        $"Offline={offline}|" +
                        $"TrialEnabled={TrialLicense.IsEnabled}|" +
                        $"Licensed={TrialLicense.IsLicensed}|" +
                        $"DaysRemaining={TrialLicense.DaysRemaining}";

                case "ACTIVATE":
                    if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[2]))
                        return "ERR|BAD_REQUEST";
                    return TrialLicense.TryActivate(parts[2].Trim()) ? "OK|ACTIVATED" : "ERR|INVALID_KEY";

                case "GET_MENUS":
                {
                    string allItems = string.Join(",",
                        Purchases.ConfigurableMenuItems.Select(m => $"{m.Name}:{m.Label}"));
                    string hiddenItems = string.Join(",",
                        Purchases.ConfigurableMenuItems.Select(m => m.Name).Where(Purchases.IsMenuHidden));
                    return $"OK|MENUS|{allItems}|{hiddenItems}";
                }

                case "SET_MENUS":
                {
                    string hiddenArg = parts.Length >= 3 ? parts[2] : "";
                    var validNames = new HashSet<string>(
                        Purchases.ConfigurableMenuItems.Select(m => m.Name), StringComparer.OrdinalIgnoreCase);

                    string[] requestedHidden = hiddenArg
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(n => n.Trim())
                        .Where(validNames.Contains)
                        .ToArray();

                    if (!SetHiddenMenus(string.Join(",", requestedHidden)))
                        return "ERR|SAVE_FAILED";

                    ApplyMenuVisibilityToRunningInstance();
                    return "OK|MENUS_UPDATED";
                }

                default:
                    return "ERR|UNKNOWN_COMMAND";
            }
        }

        private static bool SetHiddenMenus(string commaSeparatedNames)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["HIDDEN_MENUS"] == null)
                    config.AppSettings.Settings.Add("HIDDEN_MENUS", commaSeparatedNames);
                else
                    config.AppSettings.Settings["HIDDEN_MENUS"].Value = commaSeparatedNames;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Applies a HIDDEN_MENUS change to the main window immediately, the same way it already
        /// refreshes after System Config closes - without this, a remote menu change would sit in
        /// App.config unnoticed until the next login/restart. Marshaled onto the UI thread since
        /// this runs on a ThreadPool worker; a Purchases window that isn't open yet (e.g. the
        /// request lands during startup) just means there's nothing to refresh - ApplyMenuVisibility
        /// already runs on its own Load, so it'll pick up the new setting whenever it does open.
        /// </summary>
        private static void ApplyMenuVisibilityToRunningInstance()
        {
            try
            {
                Purchases purchases = Application.OpenForms.Cast<Form>().OfType<Purchases>().FirstOrDefault();
                if (purchases == null)
                    return;

                if (purchases.InvokeRequired)
                    purchases.Invoke((MethodInvoker)(() => purchases.ApplyMenuVisibility()));
                else
                    purchases.ApplyMenuVisibility();
            }
            catch
            {
                // Best-effort only - the config change already succeeded either way.
            }
        }
    }
}

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RemoteController
{
    /// <summary>
    /// Talks to a running BrewCode terminal's RemoteControlServer (Inventory\RemoteControlServer.cs)
    /// over TCP+TLS, using its one-request-per-connection protocol. Kept as a standalone duplicate
    /// of that protocol rather than a shared library, same reasoning as LicenseKeyGenerator - this
    /// tool has no build dependency on the main app.
    ///
    /// Request:  {token}|{COMMAND}[|{arg}]\n
    /// Response: OK|...   or   ERR|{reason}
    ///
    /// The terminal presents a self-signed certificate (there's no real CA involved), so this is a
    /// trust-on-first-use model: pass the thumbprint shown on that terminal's System Config >
    /// Remote Control screen as <paramref name="pinnedThumbprint"/> to require an exact match, or
    /// leave it blank to still get an encrypted connection without protection against an active
    /// man-in-the-middle.
    /// </summary>
    internal static class RemoteControlClient
    {
        public static string SendCommand(string host, int port, string token, string command, string arg,
            string pinnedThumbprint, int timeoutMs = 5000)
        {
            using (var client = new TcpClient())
            {
                IAsyncResult connectResult = client.BeginConnect(host, port, null, null);
                if (!connectResult.AsyncWaitHandle.WaitOne(timeoutMs))
                    throw new TimeoutException($"Could not connect to {host}:{port} within {timeoutMs}ms.");
                client.EndConnect(connectResult);

                using (NetworkStream rawStream = client.GetStream())
                using (var sslStream = new SslStream(rawStream, false,
                    (sender, cert, chain, errors) => ValidateServerCertificate(cert, pinnedThumbprint)))
                {
                    rawStream.ReadTimeout = timeoutMs;
                    rawStream.WriteTimeout = timeoutMs;

                    sslStream.AuthenticateAsClient(host, null, SslProtocols.None, false);
                    sslStream.ReadTimeout = timeoutMs;
                    sslStream.WriteTimeout = timeoutMs;

                    return SendOverStream(sslStream, token, command, arg);
                }
            }
        }

        private static bool ValidateServerCertificate(X509Certificate cert, string pinnedThumbprint)
        {
            if (string.IsNullOrWhiteSpace(pinnedThumbprint))
                return true;

            using (var cert2 = new X509Certificate2(cert))
                return string.Equals(cert2.Thumbprint, pinnedThumbprint.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Shared by the TCP+TLS path above and BluetoothControlClient, which connects over
        /// a plain (unencrypted) RFCOMM stream instead - same request/response framing either way.</summary>
        internal static string SendOverStream(Stream stream, string token, string command, string arg)
        {
            string request = arg == null ? $"{token}|{command}" : $"{token}|{command}|{arg}";
            byte[] requestBytes = Encoding.UTF8.GetBytes(request + "\n");
            stream.Write(requestBytes, 0, requestBytes.Length);

            return ReadLine(stream);
        }

        private static string ReadLine(Stream stream)
        {
            var sb = new StringBuilder();
            int b;
            while (sb.Length < 4096 && (b = stream.ReadByte()) != -1 && b != '\n')
            {
                if (b != '\r')
                    sb.Append((char)b);
            }
            return sb.ToString();
        }
    }
}

using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MauiRemoteController.Services;

/// <summary>
/// Talks to a running BrewCode terminal's RemoteControlServer (Inventory\RemoteControlServer.cs
/// in the POS app's repo) over TCP+TLS, using its one-request-per-connection protocol. This is a
/// deliberate duplicate of that protocol rather than a shared library - this app has no build
/// dependency on the POS app, same reasoning the existing WinForms RemoteController companion
/// already follows.
///
/// Request:  {token}|{COMMAND}[|{arg}]\n
/// Response: OK|...   or   ERR|{reason}
///
/// The terminal presents a self-signed certificate (there's no real CA involved), so this is a
/// trust-on-first-use model: pass the thumbprint the user confirmed against that terminal's own
/// System Config > Remote Control screen as <paramref name="pinnedThumbprint"/> to require an
/// exact match, or leave it null/empty during the first-connect flow (see
/// <see cref="GetServerCertificateThumbprintAsync"/>) to still get an encrypted connection without
/// protection against an active man-in-the-middle.
/// </summary>
public static class RemoteControlClient
{
    private const int MaxLineLength = 4096;

    public static async Task<string> SendCommandAsync(
        string host, int port, string token, string command, string? arg,
        string? pinnedThumbprint, int timeoutMs = 5000, CancellationToken cancellationToken = default)
    {
        // One deadline covers connect + handshake + request/response, rather than separate
        // budgets per phase - simpler to reason about for a caller than the original WinForms
        // client's per-phase timeouts, and just as effective at not hanging the UI.
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);

        using var client = new TcpClient();
        await ConnectAsync(client, host, port, timeoutCts.Token, cancellationToken, timeoutMs).ConfigureAwait(false);

        using NetworkStream rawStream = client.GetStream();
        using var sslStream = new SslStream(rawStream, leaveInnerStreamOpen: false,
            (_, cert, _, _) => ValidateServerCertificate(cert, pinnedThumbprint));

        await AuthenticateAsync(sslStream, host, timeoutCts.Token, cancellationToken, timeoutMs).ConfigureAwait(false);

        return await SendOverStreamAsync(sslStream, token, command, arg, timeoutCts.Token).ConfigureAwait(false);
    }

    /// <summary>
    /// Connects and completes the TLS handshake while accepting whatever certificate the
    /// terminal presents, purely so the caller can show the user its thumbprint and ask them to
    /// confirm it against the terminal's own System Config screen before saving it as pinned.
    /// Sends no command - the connection is closed as soon as the handshake completes.
    /// </summary>
    public static async Task<string> GetServerCertificateThumbprintAsync(
        string host, int port, int timeoutMs = 5000, CancellationToken cancellationToken = default)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);

        using var client = new TcpClient();
        await ConnectAsync(client, host, port, timeoutCts.Token, cancellationToken, timeoutMs).ConfigureAwait(false);

        using NetworkStream rawStream = client.GetStream();
        using var sslStream = new SslStream(rawStream, leaveInnerStreamOpen: false,
            (_, _, _, _) => true);

        await AuthenticateAsync(sslStream, host, timeoutCts.Token, cancellationToken, timeoutMs).ConfigureAwait(false);

        X509Certificate? remoteCert = sslStream.RemoteCertificate
            ?? throw new InvalidOperationException($"{host}:{port} did not present a certificate.");

        using var cert2 = new X509Certificate2(remoteCert);
        if (string.IsNullOrEmpty(cert2.Thumbprint))
            throw new InvalidOperationException($"{host}:{port}'s certificate has no thumbprint.");

        return cert2.Thumbprint;
    }

    private static async Task ConnectAsync(TcpClient client, string host, int port,
        CancellationToken timeoutToken, CancellationToken callerToken, int timeoutMs)
    {
        try
        {
            await client.ConnectAsync(host, port, timeoutToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!callerToken.IsCancellationRequested)
        {
            throw new TimeoutException($"Could not connect to {host}:{port} within {timeoutMs}ms.");
        }
    }

    private static async Task AuthenticateAsync(SslStream sslStream, string host,
        CancellationToken timeoutToken, CancellationToken callerToken, int timeoutMs)
    {
        // SslProtocols.None lets the OS pick the strongest protocol both sides support, rather
        // than hardcoding a version this platform's TLS stack might not actually offer - matches
        // the server side (Inventory\RemoteControlServer.cs).
        var options = new SslClientAuthenticationOptions
        {
            TargetHost = host,
            EnabledSslProtocols = SslProtocols.None,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
        };

        try
        {
            await sslStream.AuthenticateAsClientAsync(options, timeoutToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!callerToken.IsCancellationRequested)
        {
            throw new TimeoutException($"TLS handshake with {host} timed out after {timeoutMs}ms.");
        }
    }

    private static bool ValidateServerCertificate(X509Certificate? certificate, string? pinnedThumbprint)
    {
        if (string.IsNullOrWhiteSpace(pinnedThumbprint))
            return true;

        if (certificate is null)
            return false;

        using var cert2 = new X509Certificate2(certificate);
        return string.Equals(cert2.Thumbprint, pinnedThumbprint.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> SendOverStreamAsync(
        Stream stream, string token, string command, string? arg, CancellationToken cancellationToken)
    {
        string request = arg is null ? $"{token}|{command}" : $"{token}|{command}|{arg}";
        byte[] requestBytes = Encoding.UTF8.GetBytes(request + "\n");
        await stream.WriteAsync(requestBytes, cancellationToken).ConfigureAwait(false);

        return await ReadLineAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<string> ReadLineAsync(Stream stream, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        var buffer = new byte[1];

        // Line-by-line, capped so a misbehaving/compromised terminal can't hold the connection
        // open streaming garbage forever - mirrors the server's own ReadLine cap.
        while (sb.Length < MaxLineLength)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
            if (read == 0)
                break;

            byte b = buffer[0];
            if (b == (byte)'\n')
                break;
            if (b != (byte)'\r')
                sb.Append((char)b);
        }

        return sb.ToString();
    }
}

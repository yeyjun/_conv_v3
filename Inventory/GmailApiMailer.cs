using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Inventory
{
    /// <summary>
    /// Sends report emails through the Gmail REST API (gmail.googleapis.com) instead of SMTP,
    /// selected via MAIL_PROVIDER=Gmail in System Config > Report Schedule. Authenticates with a
    /// long-lived OAuth 2.0 refresh token (GMAIL_REFRESH_TOKEN), exchanged for a fresh access
    /// token on every send against a Desktop-type Client ID/Secret (GMAIL_CLIENT_ID /
    /// GMAIL_CLIENT_SECRET) - the refresh token itself is obtained out-of-band (e.g. OAuth
    /// Playground) and pasted into System Config; there is no interactive sign-in in the app.
    /// </summary>
    internal static class GmailApiMailer
    {
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string SendEndpoint = "https://gmail.googleapis.com/gmail/v1/users/me/messages/send";

        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendEmailAsync(string recipient, string subject, string body,
            MemoryStream attachmentStream, string attachmentName)
        {
            string clientId = ConfigurationManager.AppSettings["GMAIL_CLIENT_ID"];
            string clientSecret = ConfigurationManager.AppSettings["GMAIL_CLIENT_SECRET"];
            string refreshToken = ConfigurationManager.AppSettings["GMAIL_REFRESH_TOKEN"];
            string sender = ConfigurationManager.AppSettings["GMAIL_SENDER"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) ||
                string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(sender))
                throw new InvalidOperationException(
                    "Gmail API is not fully configured (Client ID, Client Secret, Refresh Token, and Sender Address are all required).");

            string accessToken = await GetAccessTokenAsync(clientId, clientSecret, refreshToken);

            byte[] rawMessage = BuildRawMessage(sender, recipient, subject, body, attachmentStream.ToArray(), attachmentName);
            string encoded = Base64UrlEncode(rawMessage);

            using (var request = new HttpRequestMessage(HttpMethod.Post, SendEndpoint))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(new JObject { ["raw"] = encoded }.ToString(), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await HttpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        throw new InvalidOperationException($"Gmail API send failed ({(int)response.StatusCode}): {error}");
                    }
                }
            }
        }

        private static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string refreshToken)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
            });

            using (HttpResponseMessage response = await HttpClient.PostAsync(TokenEndpoint, form))
            {
                string json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Failed to refresh Gmail access token ({(int)response.StatusCode}): {json}");

                string accessToken = JObject.Parse(json).Value<string>("access_token");
                if (string.IsNullOrEmpty(accessToken))
                    throw new InvalidOperationException("Gmail token response did not include an access_token.");

                return accessToken;
            }
        }

        // Gmail's messages.send takes a full RFC 2822 message base64url-encoded under "raw" -
        // there is no separate attachment parameter, so the multipart MIME body is built by hand.
        private static byte[] BuildRawMessage(string from, string to, string subject, string body,
            byte[] attachmentBytes, string attachmentName)
        {
            string boundary = "----=_ReportBoundary_" + Guid.NewGuid().ToString("N");

            var mime = new StringBuilder();
            mime.Append("MIME-Version: 1.0\r\n");
            mime.Append($"From: {from}\r\n");
            mime.Append($"To: {to}\r\n");
            mime.Append($"Subject: {EncodeSubject(subject)}\r\n");
            mime.Append($"Content-Type: multipart/mixed; boundary=\"{boundary}\"\r\n\r\n");

            mime.Append($"--{boundary}\r\n");
            mime.Append("Content-Type: text/plain; charset=\"UTF-8\"\r\n");
            mime.Append("Content-Transfer-Encoding: 8bit\r\n\r\n");
            mime.Append(body);
            mime.Append("\r\n\r\n");

            mime.Append($"--{boundary}\r\n");
            mime.Append("Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet\r\n");
            mime.Append($"Content-Disposition: attachment; filename=\"{attachmentName}\"\r\n");
            mime.Append("Content-Transfer-Encoding: base64\r\n\r\n");
            mime.Append(Convert.ToBase64String(attachmentBytes, Base64FormattingOptions.InsertLineBreaks));
            mime.Append("\r\n\r\n");

            mime.Append($"--{boundary}--");

            return Encoding.UTF8.GetBytes(mime.ToString());
        }

        // Non-ASCII subjects need RFC 2047 encoded-word form; plain ASCII passes through untouched.
        private static string EncodeSubject(string subject)
        {
            foreach (char c in subject)
            {
                if (c > 127)
                    return "=?UTF-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(subject)) + "?=";
            }
            return subject;
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}

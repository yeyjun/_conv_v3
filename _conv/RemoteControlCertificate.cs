using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace _conv
{
    /// <summary>
    /// Generates (once) and reloads a self-signed TLS certificate for RemoteControlServer, so the
    /// shared token and every command/response are encrypted in transit - important once the
    /// listener may be reached over the public internet rather than only the store's own LAN.
    /// There's no real CA involved (a self-signed cert can't be), so the trust model is "trust on
    /// first use": System Config > Remote Control shows this certificate's thumbprint, and the
    /// RemoteController operator pastes it into the app once to pin it. Uses BouncyCastle (already
    /// a transitive dependency of Aspose.PDF/ClosedXML, already referenced in this project) since
    /// .NET Framework 4.7.2 has no in-box API for creating a certificate from scratch.
    /// </summary>
    internal static class RemoteControlCertificate
    {
        private static string CertPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "RemoteControlCert.pfx");

        // The .pfx password only protects the private key file at rest on this machine - it isn't
        // a secret shared with anyone, so a fixed value is fine (same reasoning as the app's other
        // embedded keys/salts elsewhere).
        private const string CertPassword = "BACLIG-APPINV-REMOTECERT";

        private static X509Certificate2 _cached;

        /// <summary>Loads the existing certificate, or generates and saves a new one on first use.</summary>
        public static X509Certificate2 GetOrCreate()
        {
            if (_cached != null)
                return _cached;

            if (File.Exists(CertPath))
            {
                try
                {
                    _cached = new X509Certificate2(CertPath, CertPassword,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                    return _cached;
                }
                catch
                {
                    // Corrupt/unreadable file - fall through and regenerate.
                }
            }

            _cached = Create();
            return _cached;
        }

        public static string GetThumbprint() => GetOrCreate().Thumbprint;

        private static X509Certificate2 Create()
        {
            var random = new SecureRandom();

            var keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(random, 2048));
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            var subject = new Org.BouncyCastle.Asn1.X509.X509Name("CN=App Inventory Remote Control");
            BigInteger serial = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);

            var certGen = new X509V3CertificateGenerator();
            certGen.SetSerialNumber(serial);
            certGen.SetIssuerDN(subject);
            certGen.SetSubjectDN(subject);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddYears(10));
            certGen.SetPublicKey(keyPair.Public);

            var signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private, random);
            Org.BouncyCastle.X509.X509Certificate bcCert = certGen.Generate(signatureFactory);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            var certEntry = new X509CertificateEntry(bcCert);
            store.SetCertificateEntry("RemoteControl", certEntry);
            store.SetKeyEntry("RemoteControl", new AsymmetricKeyEntry(keyPair.Private), new[] { certEntry });

            using (var ms = new MemoryStream())
            {
                store.Save(ms, CertPassword.ToCharArray(), random);
                byte[] pfxBytes = ms.ToArray();

                Directory.CreateDirectory(Path.GetDirectoryName(CertPath));
                File.WriteAllBytes(CertPath, pfxBytes);

                return new X509Certificate2(pfxBytes, CertPassword,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            }
        }
    }
}


using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Principal;

namespace Server
{
    public class ServerTest : IServerTest
    {
        public static List<string> portList = new List<string>();
        public static List<X509Certificate2> RevocationList = new List<X509Certificate2>();

        public static void GenerateServerCertificate(string caCommonName, string serverCommonName, string password)
        {
            try
            {
                // Create a Certificate Authority (CA) certificate
                using (var caKey = new RSACryptoServiceProvider(2048))
                {
                    var caRequest = new CertificateRequest($"CN={caCommonName}", caKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    // Include Basic Constraints extension to mark the CA certificate as a CA
                    caRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 0, true));

                    var caCertificate = caRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

                    // Create a server certificate signed by the CA
                    using (var serverKey = new RSACryptoServiceProvider(2048))
                    {
                        var serverRequest = new CertificateRequest($"CN={serverCommonName}", serverKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                        // Use the CA certificate's expiration date as a reference for the server certificate
                        var serverCertificate = serverRequest.Create(caCertificate, DateTimeOffset.Now, caCertificate.NotAfter, new byte[] { 1, 2, 3, 4 });

                        // Save server certificate
                        string serverCertPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", $"{serverCommonName}.cer");
                        var serverCertBytes = serverCertificate.Export(X509ContentType.Cert);
                        File.WriteAllBytes(serverCertPath, serverCertBytes);

                        // Save server private key (pvk)
                        string pvkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", $"{serverCommonName}.pvk");
                        var pvkBytes = ExportPrivateKey(serverKey);
                        File.WriteAllBytes(pvkPath, pvkBytes);

                        // Convert server certificate to pfx format
                        var pfx = new X509Certificate2(serverCertBytes);
                        string pfxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", $"{serverCommonName}.pfx");
                        var pfxBytes = pfx.Export(X509ContentType.Pkcs12, password);
                        File.WriteAllBytes(pfxPath, pfxBytes);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Generate Err: " + e);
            }
        }


        private static byte[] ExportPrivateKey(RSACryptoServiceProvider csp)
        {
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)0x06); // PRIVATEKEYBLOB
                writer.Write((byte)0x02); // Version
                writer.Write((byte)0x00); // Reserved
                writer.Write((byte)0x00); // ALG_ID
                WriteDword(writer, parameters.Modulus.Length);
                WriteDword(writer, parameters.Exponent.Length);
                WriteDword(writer, parameters.D.Length);
                WriteDword(writer, parameters.P.Length);
                WriteDword(writer, parameters.Q.Length);
                WriteDword(writer, parameters.DP.Length);
                WriteDword(writer, parameters.DQ.Length);
                WriteDword(writer, parameters.InverseQ.Length);
                WriteDword(writer, 0); // DSS Seed
                WriteDword(writer, 0); // DSS Counter
                WriteDword(writer, 0); // DSS X
                WriteDword(writer, 0); // DSS Y
                WriteDword(writer, 0); // DSS J
                WriteDword(writer, 0); // DSS C
                WriteDword(writer, 0); // DSS S
                writer.Write(parameters.Modulus);
                writer.Write(parameters.Exponent);
                writer.Write(parameters.D);
                writer.Write(parameters.P);
                writer.Write(parameters.Q);
                writer.Write(parameters.DP);
                writer.Write(parameters.DQ);
                writer.Write(parameters.InverseQ);
                return stream.ToArray();
            }
        }

        private static void WriteDword(BinaryWriter writer, int value)
        {
            writer.Write((byte)(value & 0xff));
            writer.Write((byte)((value >> 8) & 0xff));
            writer.Write((byte)((value >> 16) & 0xff));
            writer.Write((byte)((value >> 24) & 0xff));
        }

     
        
        public void AddPortToServerList(string port)
        {
            
            portList.Add(port);
        }

        public void RemovePortFromServerList(string port)
        {
            portList.Remove(port);
        }

        public List<string> ClientMenu(string clientPort)
        {
            List<string> ports = new List<string>();
            foreach (string port in portList)
            {
                if (port != clientPort)
                {
                    ports.Add(port);
                }
            }
            return ports;
        }

        public X509Certificate2 CertificateRequest()
        {
            string servCert = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            return CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, servCert);
        }


        public void CompromitedCert(X509Certificate2 cert)
        {
            RevocationList.Add(cert);
        }
    }
}


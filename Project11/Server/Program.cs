using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            string keyFile = "SecretKey";    
            //string folderNameDES = "DES/";
            byte[] eSecretKeyDes = SecretKey.GenerateKey(AlgorithmType.DES);
            SecretKey.StoreKey(eSecretKeyDes,keyFile);

            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:4001";
            ServiceHost host = new ServiceHost(typeof(ServerTest));

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            host.AddServiceEndpoint(typeof(IServerTest), binding, address);


            try
            {
                host.Open();
                Console.WriteLine("Server started successfully");
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();

            }
        }
    }
}

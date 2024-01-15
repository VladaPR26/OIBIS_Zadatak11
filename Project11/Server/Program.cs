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

            // Generisanje tajnog kljuca za kriptovanje poruka ka monitoring serveru
            string keyFile = "SecretKey";    
            //string folderNameDES = "AES/";
            byte[] eSecretKeyDes = SecretKey.GenerateKey(AlgorithmType.AES);
            SecretKey.StoreKey(eSecretKeyDes,keyFile);

            try
            {
                ServerTest.GenerateServerCertificate("TestCA", "wcfservice", "1234");
                ServerTest.GenerateServerCertificate("TestCA", "wcfclient", "1234");
                Console.WriteLine("Uspesno generisanje potrebnih sertifikata");
            }
            catch (Exception e)
            {
                Console.WriteLine("neuspesno generisanje potrebnih sertifikata: " + e);
            }

            // Kreiranje konekcije izmedju klijenta i servera
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:4001";
            ServiceHost host = new ServiceHost(typeof(ServerTest));

            // Windows autentifikacija servera i klijenta
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            // Otvaranje server hosta
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


using Common;
using Manager;
using SymmetricAlgorithms;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Preuzimanje tajnog kljuca iz fajla koji je server kreirao
            string keyFile = "SecretKey";
            string folderNameDES = "../../../Server/bin/Debug/";


            // Osnovna konekcija izmedju dva klijenta
            NetTcpBinding clientBinding = new NetTcpBinding();
            Console.WriteLine("Unesite port: ");
            string port = Console.ReadLine();
            string clientAddress = "net.tcp://localhost:" + port;
            ServiceHost host = new ServiceHost(typeof(WCFClient));
            NetTcpBinding clientServerBinding = new NetTcpBinding();

            
            // Window autentifikacija klijenta
            clientServerBinding.Security.Mode = SecurityMode.Transport;
            clientServerBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            clientServerBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            Console.WriteLine("User's identity name and its AuthenticationType:" + WindowsIdentity.GetCurrent().Name + " " + WindowsIdentity.GetCurrent().AuthenticationType);


            // Konekcija izmedju klijenta i servera
            NetTcpBinding serverBinding = new NetTcpBinding();
            EndpointAddress serverAddress = new EndpointAddress(new Uri("net.tcp://localhost:4001"));
            ChannelFactory<IServerTest> channelServer =
               new ChannelFactory<IServerTest>(serverBinding, serverAddress);
            IServerTest proxyServer = channelServer.CreateChannel();

            //Dodavanje porta u listu portova na koje klijenti mogu da se konektuju i ispisvanje menija sa slobodnim portovima za konekciju
            proxyServer.AddPortToServerList(port);
            
            Console.WriteLine("Pritisnite bilo koje dugme da prikazete listu slobodnih portova");
            Console.ReadKey();
            Console.WriteLine("Slobodni portovi na koje se mozete konektovati:");
            List<string> ports=proxyServer.ClientMenu(port);
            foreach (string p in ports)
            {
                Console.WriteLine(p);
            }


            // Veza klijenta sa monitoring serverom
            NetTcpBinding monitoringServerBinding = new NetTcpBinding();
            EndpointAddress monitoringServerAddress = new EndpointAddress(new Uri("net.tcp://localhost:4000"));
            string windowsName = WindowsIdentity.GetCurrent().Name; //Potrebe ispisa
            ChannelFactory<IMonitoringServerTest> channelMontoring =
              new ChannelFactory<IMonitoringServerTest>(monitoringServerBinding, monitoringServerAddress);
            IMonitoringServerTest proxyMonitoring = channelMontoring.CreateChannel();


            // Audit deo za logovanje konektovanja i diskonektovanja klijenta
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);


            // Dodavanje sertifikata serverskom delu klijenta
            string srvCertCN = "wcfservice";
            clientBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            host.AddServiceEndpoint(typeof(IWCFContract), clientBinding, clientAddress);
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            
            // Klijentski deo klijenta
            try
            {
                // Konekcija na drugog klijenta
                host.Open();
                Console.WriteLine("Client server started successfully");
                Console.WriteLine("Unesite port na koji zelite da se konektujete: ");
                string portConnect = Console.ReadLine();

                // Dodavanje sertifikata klijentskom delu klijenta
                clientServerBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                X509Certificate2 clnCert1 = proxyServer.CertificateRequest(); // Server izdaje sertifikat za povezivanje klijentu
                EndpointAddress clientServerAddres = new EndpointAddress(new Uri("net.tcp://localhost:" + portConnect), new X509CertificateEndpointIdentity(clnCert1));
                proxyServer.RemovePortFromServerList(portConnect); // Brisanje porta iz liste slobodnih portova za konektovanje klijenta na koji smo se konektovali jer vise nije slobodan
                // Pravljenje instance fatory-a klijenta i slanje poruke drugom klijentu, odnosno sifrovane poruke monitoring serveru
                string message = "";
                string monitoringMessage = "";
                using (WCFClient proxy = new WCFClient(clientServerBinding, clientServerAddres))
                {
                    proxy.Com("Korisnik se konektovao sa vama sa porta: " + port);

                    Audit.ConnectionSuccess(); // Kada se napravi factory odnosno konekcija Audit loguje da je korisnik povezan
                    // Klijent se diskonektuje kada napise x
                    while (!message.Equals("x"))
                    {
                        try
                        {
                            Console.WriteLine("Upisi poruku");
                            message = Console.ReadLine();
                            if (message.Equals("kompromitovan"))
                            {
                                try
                                {
                                    proxyServer.CompromitedCert(clnCert1);
                                    clnCert1 = proxyServer.CertificateRequest();
                                    Console.WriteLine("Kompromitovani sertifikat je povucen i dobili ste novi, mozete nastaviti sa komuniakcijom");
                                }
                                catch(Exception) 
                                {
                                    Console.WriteLine("Neuspesno dobavljanje novog sertifikata");
                                    Console.ReadKey();
                                }
                            }
                            proxy.Com(message);
                            // Poruka za monitoring server sa imenom korisnika koji salje, portom i vremenom slanje
                            monitoringMessage = windowsName +" sa porta:"+ port +": "+message + " \nPoslata:" +DateTime.Now.ToString();
                            monitoringMessage = TripleDES_Symm_Algorithm.EncryptMessage(monitoringMessage, SecretKey.LoadKey(folderNameDES + keyFile));
                            proxyMonitoring.TestMonitoringServer(monitoringMessage);
                            Console.ReadLine();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception" + e);
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                Console.ReadKey();
            }
            finally
            {
                Audit.DisconnectSuccess(); // Kada se klijent diskonektuje Audit to loguje 
                
                host.Close();
            }
            Console.ReadKey();
        }
    }
}

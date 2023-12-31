﻿
using Common;
using Manager;
using SymmetricAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string keyFile = "SecretKey";
            string folderNameDES = "../../../Server/bin/Debug/";
            

            NetTcpBinding clientBinding = new NetTcpBinding();
            Console.WriteLine("Unesite port: ");
            string port = Console.ReadLine();
            string clientAddress = "net.tcp://localhost:"+port;
            ServiceHost host = new ServiceHost(typeof(WCFClient));
            NetTcpBinding clientServerBinding = new NetTcpBinding();

            string srvCertCN = "wcfservice";
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

            clientServerBinding.Security.Mode = SecurityMode.Transport;
            clientServerBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            clientServerBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            Console.WriteLine("User's identity name and its AuthenticationType:" + WindowsIdentity.GetCurrent().Name + " " + WindowsIdentity.GetCurrent().AuthenticationType);
            NetTcpBinding monitoringServerBinding = new NetTcpBinding();
            EndpointAddress monitoringServerAddress = new EndpointAddress(new Uri("net.tcp://localhost:4000"));


            ChannelFactory<IMonitoringServerTest> channelMontoring =
              new ChannelFactory<IMonitoringServerTest>(monitoringServerBinding, monitoringServerAddress);
            IMonitoringServerTest proxyMonitoring = channelMontoring.CreateChannel();


            // Audit
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);


            host.AddServiceEndpoint(typeof(IWCFContract), clientBinding, clientAddress);
            try
            {
               
                    host.Open();
                    Console.WriteLine("Client server started successfully");
                    Console.WriteLine("Unesite port na koji zelite da se konektujete: ");
                    string portConnect = Console.ReadLine();
                    EndpointAddress clientServerAddres = new EndpointAddress(new Uri("net.tcp://localhost:" + portConnect));
                    string message = "";
                    using (WCFClient proxy = new WCFClient(clientServerBinding, clientServerAddres))
                    {
                    Audit.ConnectionSuccess();
                    
                        while (!message.Equals(TripleDES_Symm_Algorithm.EncryptMessage("x", SecretKey.LoadKey(folderNameDES + keyFile))))
                        {
                            Console.WriteLine("Upisi poruku");
                            message = Console.ReadLine();
                            proxy.factory.TestComunication(message);
                            message=TripleDES_Symm_Algorithm.EncryptMessage(message, SecretKey.LoadKey(folderNameDES+keyFile));
                            proxyMonitoring.TestMonitoringServer(message);
                            Console.ReadLine();
                            
                        }
                    }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                Audit.DisconnectSuccess();
                host.Close();
            }

            NetTcpBinding serverBinding = new NetTcpBinding();
            
            serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            EndpointAddress serverAddress = new EndpointAddress(new Uri("net.tcp://localhost:4001"), new X509CertificateEndpointIdentity(srvCert));

            string cltCertCN = Common.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            ChannelFactory<IServerTest> channelServer =
               new ChannelFactory<IServerTest>(serverBinding,serverAddress);

            channelServer.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            channelServer.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            channelServer.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            //IServerTest proxyServer = channelServer.CreateChannel();
            //string testServer;
            //Console.WriteLine("To server: ");
            //testServer = Console.ReadLine();
            //proxyServer.TestServer(testServer);

            Console.ReadKey();
        }
    }
}

using Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace Client
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
    {
        IWCFContract factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
        : base(binding, address)
        {
            try
            {
                //cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
                string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
                this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
                this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

                factory = this.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine("[WCFClient Constructor] ERROR = {0}", e.Message);
                // Handle the exception as needed, e.g., throw it or log it.
            }
        }

        public WCFClient()
        {
        }

        public void TestComunication(string message)
        {

            Console.WriteLine(message);

        }

        public void Com(string message)
        {
            factory.TestComunication(message);

        }

        public void Dispose()
        {
            try
            {
                if (factory != null)
                {
                    factory = null;
                }

                this.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Dispose] ERROR = {0}", e.Message);
            }
        }
    }
}

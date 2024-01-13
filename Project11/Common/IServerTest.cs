using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IServerTest
    {
        [OperationContract]
        void AddPortToServerList(string port);

        [OperationContract]
        void RemovePortFromServerList(string port);

        [OperationContract]
        List<string> ClientMenu(string clientPort);

        [OperationContract]
        X509Certificate2 CertificateRequest();

     

        [OperationContract]
        void CompromitedCert(X509Certificate2 cert);
    }
}

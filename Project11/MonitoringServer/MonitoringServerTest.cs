using Common;
using Manager;
using SymmetricAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringServer
{
    class MonitoringServerTest : IMonitoringServerTest
    {
        public void TestMonitoringServer(string message)
        {
            string keyFile = "SecretKey";
            string folderNameDES = "../../../Server/bin/Debug/";
            string eMessage = TripleDES_Symm_Algorithm.DecryptMessage(message, SecretKey.LoadKey(folderNameDES + keyFile));
            Console.WriteLine(eMessage);
        }
    }
}

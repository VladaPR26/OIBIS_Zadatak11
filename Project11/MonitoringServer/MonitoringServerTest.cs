using Common;
using Manager;
using SymmetricAlgorithms;
using System;
using System.Collections.Generic;
using System.IO;
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
            string eMessage = AES_Symm_Algorithm.DecryptMessage(message, SecretKey.LoadKey(folderNameDES + keyFile));
            Console.WriteLine(eMessage);
            WriteInFile(eMessage);
        }

        private static void WriteInFile(string message)
        {
            string filePath = "messages.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}

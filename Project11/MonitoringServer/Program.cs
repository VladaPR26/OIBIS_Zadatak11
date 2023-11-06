using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:4000";
            ServiceHost host = new ServiceHost(typeof(MonitoringServerTest));

            host.AddServiceEndpoint(typeof(IMonitoringServerTest), binding, address);
            
            try
            {
                host.Open();
                Console.WriteLine("Monitoring server started successfully");
                Console.ReadKey();
            }catch(Exception e)
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

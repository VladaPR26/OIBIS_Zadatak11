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
            using (ServiceHost host = new ServiceHost(typeof(MonitoringServerTest)))
            {
                host.Open();
                Console.WriteLine("Monitoring server started successfully");
                Console.ReadKey();
            }
        }
    }
}

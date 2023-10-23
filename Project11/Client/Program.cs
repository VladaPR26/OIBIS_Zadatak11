using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IMonitoringServerTest> channelMontoring =
               new ChannelFactory<IMonitoringServerTest>("MonitoringServer");
            IMonitoringServerTest proxyMonitoring = channelMontoring.CreateChannel();
            string testMonitoringServer;
            Console.WriteLine("To monitoring server: ");
            testMonitoringServer=Console.ReadLine();
            proxyMonitoring.TestMonitoringServer(testMonitoringServer);

            ChannelFactory<IServerTest> channelServer =
               new ChannelFactory<IServerTest>("Server");
            IServerTest proxyServer = channelServer.CreateChannel();
            string testServer;
            Console.WriteLine("To server: ");
            testServer = Console.ReadLine();
            proxyServer.TestServer(testServer);

            Console.ReadKey();
        }
    }
}

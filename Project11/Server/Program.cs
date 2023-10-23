using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(ServerTest)))
            {
                host.Open();
                Console.WriteLine("Server started successfully");
                Console.ReadKey();
            }
        }
    }
}

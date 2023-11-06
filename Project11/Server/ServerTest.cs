
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerTest : IServerTest
    {
        public void TestServer(string message)
        {
            Console.WriteLine(message);
        }
    }
}

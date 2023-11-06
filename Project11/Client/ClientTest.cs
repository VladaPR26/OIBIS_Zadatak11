using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientTest : IClientTest
    {
        private static List<string> chatMessages = new List<string>();
        public void SendMessage(string message)
        {
            chatMessages.Add(message);
        }

        public string RecieveMessage()
        {
            if (chatMessages.Count > 0)
            {
                string latestMessage = chatMessages[chatMessages.Count - 1];
                return latestMessage;
            }
            return null;
        }

    }
}

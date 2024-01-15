using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "Common.Audit";
        const string LogName = "MySecTest";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch(Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void ConnectionSuccess()
        {
            if(customLog != null)
            {

                string ConnectionSuccess =
                    AuditEvents.ConnectionSuccess;
                string message = String.Format(ConnectionSuccess);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventId = {0}) to event log.", (int)AuditEventsTypes.Connected));
            }
        }

        public static void DisconnectSuccess()
        {
            
            if (customLog != null)
            {
                string DisconnectSuccess =
                    AuditEvents.DisconnectSuccess;
                string message = String.Format(DisconnectSuccess);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventsTypes.Disconnected));
            }
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="serviceName"> should be read from the OperationContext as follows: OperationContext.Current.IncomingMessageHeaders.Action</param>
		/// <param name="reason">permission name</param>

        public void Dispose()
        {
            if(customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}

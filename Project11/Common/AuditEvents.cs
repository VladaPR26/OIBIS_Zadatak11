using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum AuditEventsTypes
    {
        Connected = 0,
        Disconnected = 1,
        NewCert=2,
        CertCompromited=3
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if(resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string ConnectionSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventsTypes.Connected.ToString());
            }
        }

        public static string DisconnectSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventsTypes.Disconnected.ToString());
            }
        }

        public static string NewCert
        {
            get
            {
                return ResourceMgr.GetString(AuditEventsTypes.NewCert.ToString());
            }
        }

        public static string CertCompromited
        {
            get
            {
                return ResourceMgr.GetString(AuditEventsTypes.CertCompromited.ToString());
            }
        }
    }
}

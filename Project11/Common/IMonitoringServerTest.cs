using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IMonitoringServerTest
    {
        [OperationContract]
        void TestMonitoringServer(string message);

    }
}

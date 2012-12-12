using System.ServiceModel;

namespace AzurePerfTools.TableTransportTestContracts
{
    [ServiceContract]
    public interface IReverse
    {
        [OperationContract]
        string ReverseString(string text);
    }
}
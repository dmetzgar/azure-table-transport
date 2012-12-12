using AzurePerfTools.TableTransportTestContracts;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportTestClient
{
    class ReverseClient : ClientBase<IReverse>, IReverse
    {
        public ReverseClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ReverseClient(string endpointConfigName)
            : base(endpointConfigName)
        {
        }

        public string ReverseString(string text)
        {
            return base.Channel.ReverseString(text);
        }
    }
}

using AzurePerfTools.TableTransportTestContracts;

namespace AzurePerfTools.TableTransportTestService
{
    class Reverse : IReverse
    {
        public string ReverseString(string text)
        {
            return Program.ProcessReflectRequest(text);
        }
    }

}

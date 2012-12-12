using System;
using System.Threading;

namespace AzurePerfTools.TableTransportChannel
{
    internal class DummyAsyncResult : IAsyncResult
    {
        public TimeSpan Timeout { get; set; }

        public object AsyncState { get; set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return new ManualResetEvent(true); }
        }

        public bool CompletedSynchronously
        {
            get { return true; }
        }

        public bool IsCompleted
        {
            get { return true; }
        }
    }
}

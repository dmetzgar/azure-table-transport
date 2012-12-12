using System;
using System.ServiceModel.Channels;
using System.Threading;

namespace AzurePerfTools.TableTransportChannel
{
    internal class WaitForMessageAsyncResult : IAsyncResult
    {
        private ManualResetEvent asyncWaitHandle = new ManualResetEvent(false);
        private bool isCompleted = false;
        internal RequestContext requestContext;

        public TimeSpan Timeout { get; set; }

        public object AsyncState { get; set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return this.asyncWaitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return this.isCompleted; }
        }

        internal bool Result { get; set; }

        internal void Complete()
        {
            this.isCompleted = true;
            this.asyncWaitHandle.Set();
        }
    }
}

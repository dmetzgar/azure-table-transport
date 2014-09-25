using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    partial class AzureTableReplyChannel : AzureTableChannelBase, IReplyChannel
    {
        readonly EndpointAddress localAddress;

        public AzureTableReplyChannel(BufferManager bufferManager, MessageEncoderFactory encoderFactory, EndpointAddress address,
           AzureTableReplyChannelListener parent, CloudTableClient cloudTableClient, string tableName, string partitionKey,
            TimeSpan idleSleep, TimeSpan activeSleep)
            : base(bufferManager, encoderFactory, address, parent, parent.MaxReceivedMessageSize, cloudTableClient, tableName, 
            partitionKey, idleSleep, activeSleep)
        {
            this.localAddress = address;
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return BeginReceiveRequest(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public EndpointAddress LocalAddress
        {
            get { return this.localAddress; }
        }

        private RequestContext ReceiveRequestContext(SoapMessageTableEntity soapMessage)
        {
            ThrowIfDisposedOrNotOpen();
            Message message = this.ReadMessage(string.Format(ConfigurationConstants.RequestTable, this.tableName), soapMessage);
            return new AzureTableRequestContext(message, this);
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            Message message = this.ReadRequestMessage();
            return new AzureTableRequestContext(message, this);
        }

        public RequestContext ReceiveRequest()
        {
            return ReceiveRequest(DefaultReceiveTimeout);
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            context = null;
            SoapMessageTableEntity soapMessage;
            if (this.WaitForRequestMessage(timeout, out soapMessage))
            {
                context = this.ReceiveRequestContext(soapMessage);
                return true;
            }
            return false;
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            WaitForMessageAsyncResult asyncResult = new WaitForMessageAsyncResult() { Timeout = timeout, AsyncState = state };
            System.Threading.Thread thread;
            thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((obj) =>
            {
                WaitForMessageAsyncResult ar = obj as WaitForMessageAsyncResult;
                ar.Result = this.TryReceiveRequest(timeout, out ar.requestContext);
                (ar.AsyncWaitHandle as System.Threading.ManualResetEvent).Set();
                if (!this.IsChannelClosed)
                {
                    callback(ar);
                }
            }));

            thread.Start(asyncResult);
            return asyncResult;
        }

        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            WaitForMessageAsyncResult asyncResult = result as WaitForMessageAsyncResult;
            context = asyncResult.requestContext;
            return asyncResult.Result;
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            SoapMessageTableEntity soapMessage;
            return this.WaitForRequestMessage(timeout, out soapMessage);
        }

        protected override void OnAbort()
        {
            
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.SignalChannelClosed();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }
    }
}

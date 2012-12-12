using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    class AzureTableRequestChannel : AzureTableChannelBase, IRequestChannel
    {
        readonly Uri via;
        readonly object writeLock;

        public AzureTableRequestChannel(BufferManager bufferManager, MessageEncoderFactory encoderFactory, EndpointAddress address,
           AzureTableRequestChannelFactory parent, Uri via, CloudTableClient cloudTableClient, string tableName, string partitionKey)
            : base(bufferManager, encoderFactory, address, parent, parent.MaxReceivedMessageSize, cloudTableClient, tableName, partitionKey)
        {
            this.via = via;
            this.writeLock = new object();
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            return BeginRequest(message, DefaultReceiveTimeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public Message Request(Message requestMessage, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            lock (this.writeLock)
            {
                try
                {
                    // Write the request message
                    this.WriteRequestMessage(requestMessage);

                    // Wait for the response
                    SoapMessageTableEntity soapMessage;
                    if (!this.WaitForReplyMessage(timeout, out soapMessage))
                    {
                        throw new TimeoutException(timeout.ToString());
                    }

                    // Read the reply message
                    Message replyMessage = this.ReadMessage(this.tableName + "Reply", soapMessage);
                    return replyMessage;
                }
                catch (IOException exception)
                {
                    throw ConvertException(exception);
                }
            }
        }

        public Message Request(Message message)
        {
            return Request(message, DefaultReceiveTimeout);
        }

        public Uri Via
        {
            get { return this.via; }
        }

        protected override void OnAbort()
        {
            throw new NotImplementedException();
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

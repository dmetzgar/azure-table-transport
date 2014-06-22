using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    class AzureTableReplyChannelListener : ChannelListenerBase<IReplyChannel>
    {
        readonly BufferManager bufferManager;
        readonly MessageEncoderFactory encoderFactory;
        public readonly long MaxReceivedMessageSize;
        readonly string scheme;
        readonly Uri uri;
        readonly CloudStorageAccount storageAccount;
        readonly string partitionKey;
        CloudTableClient cloudTableClient;
        AzureTableReplyChannel replyChannel;

        public AzureTableReplyChannelListener(AzureTableTransportBindingElement transportElement, BindingContext context)
            : base(context.Binding)
        {
            MessageEncodingBindingElement messageEncodingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
            this.bufferManager = BufferManager.CreateBufferManager(transportElement.MaxBufferPoolSize, int.MaxValue);
            this.encoderFactory = messageEncodingElement.CreateMessageEncoderFactory();
            MaxReceivedMessageSize = transportElement.MaxReceivedMessageSize;
            this.scheme = transportElement.Scheme;
            this.uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
            this.partitionKey = transportElement.PartitionKey;

            if (transportElement.DevelopmentStorage)
            {
                this.storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                this.storageAccount = new CloudStorageAccount(new StorageCredentials(transportElement.XStoreAccountName, transportElement.XStoreAccountKey), true);
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.cloudTableClient = this.storageAccount.CreateCloudTableClient();
            CloudTable requestTable = cloudTableClient.GetTableReference(Uri.AbsolutePath + "Request");
            requestTable.CreateIfNotExists();
            CloudTable replyTable = cloudTableClient.GetTableReference(Uri.AbsolutePath + "Reply");
            replyTable.CreateIfNotExists();
        }

        protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
        {
            if (this.replyChannel == null)
            {
                EndpointAddress address = new EndpointAddress(Uri);
                this.replyChannel = new AzureTableReplyChannel(this.bufferManager, this.encoderFactory, address, this, this.cloudTableClient, this.Uri.AbsolutePath, this.partitionKey);
            }

            return this.replyChannel;
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (this.replyChannel != null)
                System.Threading.Thread.Sleep(-1);

            DummyAsyncResult asyncResult = new DummyAsyncResult() { AsyncState = state, Timeout = timeout };
            callback(asyncResult);
            return asyncResult;
        }

        protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return this.OnAcceptChannel((result as DummyAsyncResult).Timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public override Uri Uri
        {
            get { return this.uri; }
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
    }
}

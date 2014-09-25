using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    class AzureTableRequestChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        readonly BufferManager bufferManager;
        readonly MessageEncoderFactory encoderFactory;
        public readonly long MaxReceivedMessageSize;
        readonly CloudStorageAccount storageAccount;
        readonly string partitionKey;
        readonly TimeSpan idleSleep;
        readonly TimeSpan activeSleep;

        public AzureTableRequestChannelFactory(AzureTableTransportBindingElement transportElement, BindingContext context)
            : base(context.Binding)
        {
            MessageEncodingBindingElement messageEncodingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
            this.bufferManager = BufferManager.CreateBufferManager(transportElement.MaxBufferPoolSize, int.MaxValue);
            this.encoderFactory = messageEncodingElement.CreateMessageEncoderFactory();
            this.MaxReceivedMessageSize = transportElement.MaxReceivedMessageSize;
            this.partitionKey = transportElement.PartitionKey;
            this.storageAccount = CloudStorageAccount.Parse(transportElement.ConnectionString);
            this.idleSleep = TimeSpan.FromMilliseconds(transportElement.IdleSleep);
            this.activeSleep = TimeSpan.FromMilliseconds(transportElement.ActiveSleep);
        }

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            CloudTableClient cloudTableClient = this.storageAccount.CreateCloudTableClient();
            return new AzureTableRequestChannel(this.bufferManager, this.encoderFactory, address, this, via, cloudTableClient, 
                via.AbsolutePath, partitionKey, idleSleep, activeSleep);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
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
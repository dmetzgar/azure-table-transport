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

        public AzureTableRequestChannelFactory(AzureTableTransportBindingElement transportElement, BindingContext context)
            : base(context.Binding)
        {
            MessageEncodingBindingElement messageEncodingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
            this.bufferManager = BufferManager.CreateBufferManager(transportElement.MaxBufferPoolSize, int.MaxValue);
            this.encoderFactory = messageEncodingElement.CreateMessageEncoderFactory();
            this.MaxReceivedMessageSize = transportElement.MaxReceivedMessageSize;
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

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            CloudTableClient cloudTableClient = this.storageAccount.CreateCloudTableClient();
            return new AzureTableRequestChannel(this.bufferManager, this.encoderFactory, address, this, via, cloudTableClient, via.AbsolutePath, partitionKey);
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
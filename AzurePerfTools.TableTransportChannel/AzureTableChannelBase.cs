using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;

namespace AzurePerfTools.TableTransportChannel
{
    abstract class AzureTableChannelBase : ChannelBase
    {
        const int MaxBufferSize = 64 * 1024;
        const int MaxSizeOfHeaders = 4 * 1024;

        readonly EndpointAddress address;
        readonly BufferManager bufferManager;
        readonly MessageEncoder encoder;
        readonly long maxReceivedMessageSize;
        protected readonly CloudTableClient cloudTableClient;
        protected readonly string tableName;
        protected readonly string partitionKey;
        private bool channelClosed;

        public AzureTableChannelBase(BufferManager bufferManager, MessageEncoderFactory encoderFactory, EndpointAddress address, ChannelManagerBase parent, 
            long maxReceivedMessageSize, CloudTableClient cloudTableClient, string tableName, string partitionKey)
            : base(parent)
        {
            this.address = address;
            this.bufferManager = bufferManager;
            this.encoder = encoderFactory.CreateSessionEncoder();
            this.maxReceivedMessageSize = maxReceivedMessageSize;
            this.partitionKey = partitionKey;
            this.channelClosed = false;

            this.cloudTableClient = cloudTableClient;
            this.tableName = tableName;
        }

        protected bool IsChannelClosed
        {
            get { return this.channelClosed; }
        }

        protected Message ReadRequestMessage()
        {
            return this.ReadMessage(this.tableName + "Request");
        }

        protected Message ReadReplyMessage()
        {
            return this.ReadMessage(this.tableName + "Reply");
        }

        private Message ReadMessage(string tableName)
        {
            CloudTable cloudTable = this.cloudTableClient.GetTableReference(tableName);
            IEnumerable<SoapMessageTableEntity> result = cloudTable.ExecuteQuery<SoapMessageTableEntity>(
                new TableQuery<SoapMessageTableEntity>()
                {
                    FilterString = string.Format(@"PartitionKey = ""{0}""", this.partitionKey),
                    TakeCount = 1
                });

            return this.ReadMessage(tableName, result.First());
        }

        protected Message ReadMessage(string tableName, SoapMessageTableEntity soapMessage)
        {
            byte[] data = null;
            int bytesTotal;
            try
            {

                bytesTotal = Encoding.UTF8.GetByteCount(soapMessage.Message);
                if (bytesTotal > int.MaxValue)
                {
                    throw new CommunicationException(
                       String.Format("Message of size {0} bytes is too large to buffer. Use a streamed transfer instead.", bytesTotal)
                    );
                }

                if (bytesTotal > this.maxReceivedMessageSize)
                {
                    throw new CommunicationException(String.Format("Message exceeds maximum size: {0} > {1}.", bytesTotal, this.maxReceivedMessageSize));
                }

                data = this.bufferManager.TakeBuffer(bytesTotal);
                Encoding.UTF8.GetBytes(soapMessage.Message, 0, soapMessage.Message.Length, data, 0);

                ArraySegment<byte> buffer = new ArraySegment<byte>(data, 0, (int)bytesTotal);
                return this.encoder.ReadMessage(buffer, this.bufferManager);
            }
            catch (IOException exception)
            {
                throw ConvertException(exception);
            }
            finally
            {
                if (data != null)
                {
                    this.bufferManager.ReturnBuffer(data);
                    CloudTable cloudTable = this.cloudTableClient.GetTableReference(tableName);
                    cloudTable.Execute(TableOperation.Delete(soapMessage));
                }
            }
        }

        protected void WriteRequestMessage(Message message)
        {
            this.WriteMessage(this.tableName + "Request", message);
        }

        protected void WriteReplyMessage(Message message)
        {
            this.WriteMessage(this.tableName + "Reply", message);
        }

        private void WriteMessage(string tableName, Message message)
        {
            // Create a new customer entity
            SoapMessageTableEntity soapMessage = new SoapMessageTableEntity(partitionKey);
            ArraySegment<byte> buffer;
            using (message)
            {
                this.address.ApplyTo(message);
                buffer = this.encoder.WriteMessage(message, MaxBufferSize, this.bufferManager);
            }
            soapMessage.Message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            this.bufferManager.ReturnBuffer(buffer.Array);

            CloudTable cloudTable = this.cloudTableClient.GetTableReference(tableName);
            cloudTable.Execute(TableOperation.Insert(soapMessage));
        }

        protected bool WaitForRequestMessage(TimeSpan timeout, out SoapMessageTableEntity soapMessage)
        {
            return this.WaitForMessage(this.tableName + "Request", timeout, TimeSpan.FromSeconds(1), out soapMessage);
        }

        protected bool WaitForReplyMessage(TimeSpan timeout, out SoapMessageTableEntity soapMessage)
        {
            return this.WaitForMessage(this.tableName + "Reply", timeout, TimeSpan.FromSeconds(1), out soapMessage);
        }

        private bool WaitForMessage(string tableName, TimeSpan timeout, TimeSpan sleep, out SoapMessageTableEntity soapMessage)
        {
            soapMessage = null;
            if (this.channelClosed)
            {
                return false;
            }

            ThrowIfDisposedOrNotOpen();
            try
            {
                CloudTable cloudTable = this.cloudTableClient.GetTableReference(tableName);
                bool foundRecords = false;
                DateTime endTime = timeout == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + timeout;
                while (!foundRecords && DateTime.Now < endTime && !this.channelClosed)
                {
                    IEnumerable<SoapMessageTableEntity> queryResults = cloudTable.ExecuteQuery<SoapMessageTableEntity>(
                        new TableQuery<SoapMessageTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", this.partitionKey)).Take(1));
                    if (queryResults.Count() > 0)
                    {
                        foundRecords = true;
                        soapMessage = queryResults.First();
                    }
                    else
                    {
                        Thread.Sleep(sleep);
                    }
                }

                return foundRecords;
            }
            catch (IOException exception)
            {
                throw ConvertException(exception);
            }
        }

        public EndpointAddress RemoteAddress
        {
            get { return this.address; }
        }

        protected static Exception ConvertException(Exception exception)
        {
            Type exceptionType = exception.GetType();
            if (exceptionType == typeof(System.IO.DirectoryNotFoundException) ||
                exceptionType == typeof(System.IO.FileNotFoundException) ||
                exceptionType == typeof(System.IO.PathTooLongException))
            {
                return new EndpointNotFoundException(exception.Message, exception);
            }
            return new CommunicationException(exception.Message, exception);
        }

        protected void SignalChannelClosed()
        {
            this.channelClosed = true;
        }
    }
}
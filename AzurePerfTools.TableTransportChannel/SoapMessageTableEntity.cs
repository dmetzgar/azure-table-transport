using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzurePerfTools.TableTransportChannel
{
    class SoapMessageTableEntity : TableEntity
    {
        public SoapMessageTableEntity(string partitionKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public SoapMessageTableEntity() { }

        public string Message { get; set; }
    }
}

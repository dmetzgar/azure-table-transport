using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    partial class AzureTableReplyChannel
    {
        class AzureTableRequestContext : RequestContext
        {
            bool aborted;
            readonly Message message;
            readonly AzureTableReplyChannel parent;
            CommunicationState state;
            readonly object thisLock;
            readonly object writeLock;

            public AzureTableRequestContext(Message message, AzureTableReplyChannel parent)
            {
                this.aborted = false;
                this.message = message;
                this.parent = parent;
                this.state = CommunicationState.Opened;
                this.thisLock = new object();
                this.writeLock = new object();
            }

            public override void Abort()
            {
                lock (thisLock)
                {
                    if (this.aborted)
                    {
                        return;
                    }
                    this.aborted = true;
                    this.state = CommunicationState.Faulted;
                }
            }

            public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
            {
                return BeginReply(message, this.parent.DefaultSendTimeout, callback, state);
            }

            public override void Close(TimeSpan timeout)
            {
                lock (thisLock)
                {
                    this.state = CommunicationState.Closed;
                }
            }

            public override void Close()
            {
                Close(this.parent.DefaultCloseTimeout);
            }

            public override void EndReply(IAsyncResult result)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override void Reply(Message message, TimeSpan timeout)
            {
                lock (thisLock)
                {
                    if (this.aborted)
                    {
                        throw new CommunicationObjectAbortedException();
                    }
                    if (this.state == CommunicationState.Faulted)
                    {
                        throw new CommunicationObjectFaultedException();
                    }
                    if (this.state == CommunicationState.Closed)
                    {
                        throw new ObjectDisposedException("this");
                    }
                }
                this.parent.ThrowIfDisposedOrNotOpen();
                lock (writeLock)
                {
                    this.parent.WriteReplyMessage(message);
                    //this.parent.WriteMessage(AzureTableChannelBase.PathToFile(this.parent.LocalAddress.Uri, "reply"), message);
                }
            }

            public override void Reply(Message message)
            {
                Reply(message, this.parent.DefaultSendTimeout);
            }

            public override Message RequestMessage
            {
                get { return message; }
            }

            public void Dispose()
            {
                Close();
            }
        }
    }
}

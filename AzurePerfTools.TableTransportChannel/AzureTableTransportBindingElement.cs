using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportChannel
{
    public class AzureTableTransportBindingElement : TransportBindingElement
    {
        string deploymentId;
        string roleName;
        string instanceName;
        string partitionKey;

        public AzureTableTransportBindingElement()
        {
        }

        AzureTableTransportBindingElement(AzureTableTransportBindingElement other)
            : base(other)
        {
            this.XStoreAccountName = other.XStoreAccountName;
            this.XStoreAccountKey = other.XStoreAccountKey;
            this.DevelopmentStorage = other.DevelopmentStorage;
            this.DeploymentId = other.DeploymentId;
            this.RoleName = other.RoleName;
            this.InstanceName = other.InstanceName;
            this.partitionKey = other.partitionKey;
        }

        public override string Scheme
        {
            get { return "azure.table"; }
        }

        public string XStoreAccountName { get; set; }
        public string XStoreAccountKey { get; set; }
        public bool DevelopmentStorage { get; set; }

        [DebuggerHidden]
        public string DeploymentId
        {
            get
            {
                if (this.deploymentId == null && RoleEnvironment.IsAvailable)
                {
                    this.deploymentId = RoleEnvironment.DeploymentId;
                    Console.WriteLine(this.deploymentId);
                }
                return this.deploymentId;
            }
            set
            {
                this.deploymentId = value;
            }
        }

        [DebuggerHidden]
        public string RoleName
        {
            get
            {
                if (this.roleName == null && RoleEnvironment.IsAvailable)
                {
                    this.roleName = RoleEnvironment.CurrentRoleInstance.Role.Name;
                    Console.WriteLine(this.roleName);
                }
                return this.roleName;
            }
            set
            {
                this.roleName = value;
            }
        }

        [DebuggerHidden]
        public string InstanceName
        {
            get
            {
                if (this.instanceName == null && RoleEnvironment.IsAvailable)
                {
                    this.instanceName = RoleEnvironment.CurrentRoleInstance.Id;
                    Console.WriteLine(this.instanceName);
                }
                return this.instanceName;
            }
            set
            {
                this.instanceName = value;
            }
        }

        [DebuggerHidden]
        internal string PartitionKey
        {
            get
            {
                if (this.partitionKey == null)
                {
                    this.partitionKey = string.Format("{0}-{1}-{2}", this.DeploymentId, this.RoleName, this.InstanceName.Replace("_", "-"));
                }
                return partitionKey;
            }
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IRequestChannel);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IReplyChannel);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw new ArgumentException(String.Format("Unsupported channel type: {0}.", typeof(TChannel).Name));
            }
            return (IChannelFactory<TChannel>)(object)new AzureTableRequestChannelFactory(this, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!CanBuildChannelListener<TChannel>(context))
            {
                throw new ArgumentException(String.Format("Unsupported channel type: {0}.", typeof(TChannel).Name));
            }
            return (IChannelListener<TChannel>)(object)new AzureTableReplyChannelListener(this, context);
        }

        public override BindingElement Clone()
        {
            return new AzureTableTransportBindingElement(this);
        }
    }

}

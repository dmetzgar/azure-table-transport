using System;
using System.Configuration;
using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace AzurePerfTools.TableTransportChannel
{
    public class AzureTableTransportBindingConfigurationElement : StandardBindingElement
    {
        protected override Type BindingElementType
        {
            get { return typeof(AzureTableTransportBinding); }
        }

        [ConfigurationProperty(ConfigurationConstants.DeploymentId)]
        public string DeploymentId
        {
            get { return (string)base[ConfigurationConstants.DeploymentId]; }
            set { base[ConfigurationConstants.DeploymentId] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.ConnectionString, DefaultValue = ConfigurationConstants.DevelopmentStorage)]
        public string ConnectionString
        {
            get { return (string)base[ConfigurationConstants.ConnectionString]; }
            set { base[ConfigurationConstants.ConnectionString] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.RoleName)]
        public string RoleName
        {
            get { return (string)base[ConfigurationConstants.RoleName]; }
            set { base[ConfigurationConstants.RoleName] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.InstanceName)]
        public string InstanceName
        {
            get { return (string)base[ConfigurationConstants.InstanceName]; }
            set { base[ConfigurationConstants.InstanceName] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.IdleSleep, DefaultValue = ConfigurationConstants.IdleSleepMs)]
        public int IdleSleep
        {
            get { return (int)base[ConfigurationConstants.IdleSleep]; }
            set { base[ConfigurationConstants.IdleSleep] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.ActiveSleep, DefaultValue = ConfigurationConstants.ActiveSleepMs)]
        public int ActiveSleep
        {
            get { return (int)base[ConfigurationConstants.ActiveSleep]; }
            set { base[ConfigurationConstants.ActiveSleep] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                var properties = base.Properties;
                properties.Add(new ConfigurationProperty(ConfigurationConstants.ConnectionString, typeof(string), ConfigurationConstants.DevelopmentStorage));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.DeploymentId, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.RoleName, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.InstanceName, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.IdleSleep, typeof(int), ConfigurationConstants.IdleSleepMs));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.ActiveSleep, typeof(int), ConfigurationConstants.ActiveSleepMs));
                return properties;
            }
        }

        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            AzureTableTransportBinding tableBinding = (AzureTableTransportBinding)binding;
            this.ConnectionString = tableBinding.transportElement.ConnectionString;
            this.DeploymentId = tableBinding.transportElement.DeploymentId;
            this.RoleName = tableBinding.transportElement.RoleName;
            this.InstanceName = tableBinding.transportElement.InstanceName;
            this.IdleSleep = tableBinding.transportElement.IdleSleep;
            this.ActiveSleep = tableBinding.transportElement.ActiveSleep;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            if (binding.GetType() != typeof(AzureTableTransportBinding))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Invalid type for binding. Expected type: {0}. Type passed in: {1}.",
                    typeof(AzureTableTransportBinding).AssemblyQualifiedName,
                    binding.GetType().AssemblyQualifiedName));
            }

            AzureTableTransportBinding tableBinding = (AzureTableTransportBinding)binding;
            tableBinding.transportElement.ConnectionString = this.ConnectionString;
            tableBinding.transportElement.DeploymentId = this.DeploymentId;
            tableBinding.transportElement.RoleName = this.RoleName;
            tableBinding.transportElement.InstanceName = this.InstanceName;
            tableBinding.transportElement.IdleSleep = this.IdleSleep;
            tableBinding.transportElement.ActiveSleep = this.ActiveSleep;
        }
    }

    public class AzureTableTransportBindingCollectionElement :
        StandardBindingCollectionElement<AzureTableTransportBinding, AzureTableTransportBindingConfigurationElement> { }

}

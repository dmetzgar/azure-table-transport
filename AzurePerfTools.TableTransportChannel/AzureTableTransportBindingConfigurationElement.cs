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

        [ConfigurationProperty(ConfigurationConstants.DevelopmentStorage, DefaultValue = true)]
        public bool DevelopmentStorage
        {
            get { return (bool)base[ConfigurationConstants.DevelopmentStorage]; }
            set { base[ConfigurationConstants.DevelopmentStorage] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.DeploymentId)]
        public string DeploymentId
        {
            get { return (string)base[ConfigurationConstants.DeploymentId]; }
            set { base[ConfigurationConstants.DeploymentId] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.XStoreAccountName)]
        public string XStoreAccountName
        {
            get { return (string)base[ConfigurationConstants.XStoreAccountName]; }
            set { base[ConfigurationConstants.XStoreAccountName] = value; }
        }

        [ConfigurationProperty(ConfigurationConstants.XStoreAccountKey)]
        public string XStoreAccountKey
        {
            get { return (string)base[ConfigurationConstants.XStoreAccountKey]; }
            set { base[ConfigurationConstants.XStoreAccountKey] = value; }
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

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                var properties = base.Properties;
                properties.Add(new ConfigurationProperty(ConfigurationConstants.DevelopmentStorage, typeof(Boolean), true));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.DeploymentId, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.XStoreAccountName, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.XStoreAccountKey, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.RoleName, typeof(string)));
                properties.Add(new ConfigurationProperty(ConfigurationConstants.InstanceName, typeof(string)));
                return properties;
            }
        }

        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            AzureTableTransportBinding tableBinding = (AzureTableTransportBinding)binding;
            this.DevelopmentStorage = tableBinding.transportElement.DevelopmentStorage;
            this.DeploymentId = tableBinding.transportElement.DeploymentId;
            this.XStoreAccountName = tableBinding.transportElement.XStoreAccountName;
            this.XStoreAccountKey = tableBinding.transportElement.XStoreAccountKey;
            this.RoleName = tableBinding.transportElement.RoleName;
            this.InstanceName = tableBinding.transportElement.InstanceName;
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
            tableBinding.transportElement.DevelopmentStorage = this.DevelopmentStorage;
            tableBinding.transportElement.DeploymentId = this.DeploymentId;
            tableBinding.transportElement.XStoreAccountName = this.XStoreAccountName;
            tableBinding.transportElement.XStoreAccountKey = this.XStoreAccountKey;
            tableBinding.transportElement.RoleName = this.RoleName;
            tableBinding.transportElement.InstanceName = this.InstanceName;
        }
    }

    public class AzureTableTransportBindingCollectionElement :
        StandardBindingCollectionElement<AzureTableTransportBinding, AzureTableTransportBindingConfigurationElement> { }

}

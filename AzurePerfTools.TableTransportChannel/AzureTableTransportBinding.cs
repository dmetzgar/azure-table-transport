using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Globalization;

namespace AzurePerfTools.TableTransportChannel
{
    public class AzureTableTransportBinding : Binding
    {
        readonly MessageEncodingBindingElement messageElement;
        internal readonly AzureTableTransportBindingElement transportElement;

        public AzureTableTransportBinding()
        {
            this.messageElement = new TextMessageEncodingBindingElement();
            this.transportElement = new AzureTableTransportBindingElement();
        }

        public AzureTableTransportBinding(AzureTableTransportBindingElement transportElement)
        {
            this.messageElement = new TextMessageEncodingBindingElement();
            this.transportElement = transportElement;
        }

        public AzureTableTransportBinding(string configurationName)
            : this()
        {
            AzureTableTransportBindingCollectionElement section = (AzureTableTransportBindingCollectionElement)ConfigurationManager.GetSection(
                "system.serviceModel/bindings/azureTableTransportBinding");
            AzureTableTransportBindingConfigurationElement element = section.Bindings[configurationName];
            if (element == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentCulture,
                    "There is no binding named {0} at {1}.", configurationName, section.BindingName));
            }
            else
            {
                element.ApplyConfiguration(this);
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            elements.Add(this.messageElement);
            elements.Add(this.transportElement);
            return elements.Clone();
        }

        public override string Scheme
        {
            get { return this.transportElement.Scheme; }
        }
    }
}

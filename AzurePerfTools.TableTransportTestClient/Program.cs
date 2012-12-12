using AzurePerfTools.TableTransportChannel;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AzurePerfTools.TableTransportTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ReverseClient client = new ReverseClient("ReverseClient");

            while (true)
            {
                Console.Write("Enter some text (Ctrl-Z to quit): ");
                String text = Console.ReadLine();
                if (text == null)
                    break;
                string response = client.ReverseString(text);
                Console.WriteLine("Reply: {0}", response);
            }

            client.Close();
        }
    }
}

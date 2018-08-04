using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SubscriptionClient = Microsoft.Azure.ServiceBus.SubscriptionClient;

namespace Topic.Receiver.Sample
{
    class Program
    {
        private const string TopicName = "uicommand";
        //private const string TopicName = "uicommand";
        private static IConfiguration _configuration;
        private static ServiceBusConfiguration serviceBusConfiguration;
        private const string SubscriptionName = "Monitor";

        static void Main(string[] args)
        {
            while (true) {
                Console.WriteLine("Inform new Topic and hit enter");

                var topic = Console.ReadLine();

                var monitor = new Monitor();
                monitor.Init(topic);
            }
        }
    }
}
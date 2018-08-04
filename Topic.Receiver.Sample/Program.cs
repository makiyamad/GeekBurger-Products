using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeekBurger.Products.Contract;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SubscriptionClient = Microsoft.Azure.ServiceBus.SubscriptionClient;

namespace Topic.Receiver.Sample
{
    class Program
    {
        private const string TopicName = "ProductChanged";
        private static IConfiguration _configuration;
        private static ServiceBusConfiguration serviceBusConfiguration;
        private static string _storeId;
        private const string SubscriptionName = "Los_Angeles_Pasadena_store";

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Inform storeId and hit enter to get productChange messages");
                _storeId = Console.ReadLine();
            }
            else
                _storeId = args[0];

            //https://github.com/Azure-Samples/service-bus-dotnet-manage-publish-subscribe-with-basic-features

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            serviceBusConfiguration = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();

            var serviceBusNamespace = _configuration.GetServiceBusNamespace();

            var topic = serviceBusNamespace.Topics.GetByName(TopicName);

            topic.Subscriptions.DeleteByName(SubscriptionName);

            if (!topic.Subscriptions.List()
                   .Any(subscription => subscription.Name
                       .Equals(SubscriptionName, StringComparison.InvariantCultureIgnoreCase)))
                topic.Subscriptions
                    .Define(SubscriptionName)
                    .Create();

            ReceiveMessages();

            Console.ReadLine();
        }

        private static async void ReceiveMessages()
        {
            var subscriptionClient = new SubscriptionClient(serviceBusConfiguration.ConnectionString, TopicName, SubscriptionName);

            //by default a 1=1 rule is added when subscription is created, so we need to remove it
            await subscriptionClient.RemoveRuleAsync("$Default");

            await subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter { Label = _storeId },
                Name = "filter-store"
            });

            var mo = new MessageHandlerOptions(ExceptionHandle) { AutoComplete = true };

            subscriptionClient.RegisterMessageHandler(Handle, mo);
            
            Console.ReadLine();
        }

        private static Task Handle(Message message, CancellationToken arg2)
        {
            Console.WriteLine($"message Label: {message.Label}");
            Console.WriteLine($"message CorrelationId: {message.CorrelationId}");
            var productChangesString = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine("Message Received");
            Console.WriteLine(productChangesString);

            Thread.Sleep(40000);

            return Task.CompletedTask;
        }

        private static Task ExceptionHandle(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler encountered an exception {arg.Exception}.");
            var context = arg.ExceptionReceivedContext;
            Console.WriteLine($"- Endpoint: {context.Endpoint}, Path: {context.EntityPath}, Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
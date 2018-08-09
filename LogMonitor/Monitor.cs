using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Topic.Receiver.Sample;
using SubscriptionClient = Microsoft.Azure.ServiceBus.SubscriptionClient;

namespace LogMonitor
{
    public class Monitor : IDisposable
    {
        private string _topicName = "log";
        private static IConfiguration _configuration;
        private static ServiceBusConfiguration _serviceBusConfiguration;
        private const string SubscriptionName = "Monitor";
        private SubscriptionClient _subscriptionClient;

        public void Init(string topicName)
        {
            _topicName = topicName;
            //https://github.com/Azure-Samples/service-bus-dotnet-manage-publish-subscribe-with-basic-features

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _serviceBusConfiguration = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();

            var serviceBusNamespace = _configuration.GetServiceBusNamespace();

            ITopic topic = null;
            try
            {
                topic = serviceBusNamespace.Topics.GetByName(topicName);
            }
            catch (Exception)
            {
                serviceBusNamespace.Topics.Define(topicName).Create();
            }

            try
            {
                topic?.Subscriptions.DeleteByName(SubscriptionName);
            }
            catch (Exception) { }

            if (topic != null && !topic.Subscriptions.List()
                    .Any(subscription => subscription.Name
                        .Equals(SubscriptionName, StringComparison.InvariantCultureIgnoreCase)))
                topic.Subscriptions
                    .Define(SubscriptionName)
                    .Create();

            ReceiveMessages();
        }

        private void ReceiveMessages()
        {
            _subscriptionClient = new SubscriptionClient(_serviceBusConfiguration.ConnectionString, _topicName, SubscriptionName);

            var mo = new MessageHandlerOptions(ExceptionHandle) { AutoComplete = true };

            _subscriptionClient.RegisterMessageHandler(Handle, mo);
        }

        private static Task Handle(Message message, CancellationToken arg2)
        {
            var label = message.Label;
            if (message.Body == null)
            {
                Console.WriteLine($"{label}-empty message");

            }
            else
            {
                var productChangesString = Encoding.UTF8.GetString(message.Body);

                Console.WriteLine($"{label}-{productChangesString}");
            }


            return Task.CompletedTask;
        }

        private static Task ExceptionHandle(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler encountered an exception {arg.Exception}.");
            var context = arg.ExceptionReceivedContext;
            Console.WriteLine($"- Endpoint: {context.Endpoint}, Path: {context.EntityPath}, Action: {context.Action}");
            return Task.CompletedTask;
        }

        public async void Dispose()
        {
            await _subscriptionClient.CloseAsync();
        }
    }
}
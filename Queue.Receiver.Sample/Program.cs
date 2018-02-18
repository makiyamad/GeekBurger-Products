using GeekBurger.Products.Contract;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Queue.Receiver.Sample
{
    class Program
    {
        const string QueuePath = "ProductChanged";
        static IQueueClient _queueClient;
        private static string _storeId;
        private static List<Task> PendingCompleteTasks;
        private static int count;
        private static IConfiguration _configuration;
        static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            PendingCompleteTasks = new List<Task>();
            if (args.Length <= 0)
            {
                Console.WriteLine("Please inform store id and hit enter to receive product change messages");
                _storeId = Console.ReadLine();
            }
            else
                _storeId = args[0];

            ReceiveAsync().GetAwaiter().GetResult();
        }

        public static async Task ReceiveAsync()
        {
            _queueClient = new QueueClient(_configuration["connectionStrings:serviceBusConnectionString"], QueuePath, ReceiveMode.PeekLock);
            var handlerOptions = new MessageHandlerOptions(ExceptionHandler) { AutoComplete = false, MaxConcurrentCalls = 3 };
            _queueClient.RegisterMessageHandler(MessageHandler, handlerOptions);

            Console.ReadLine();
            Console.WriteLine($"Request to close async");
            Console.WriteLine($"pending tasks: {PendingCompleteTasks.Count}");
            await Task.WhenAll(PendingCompleteTasks);
            Console.WriteLine($"All pending tasks were completed");
            await _queueClient.CloseAsync();
            Console.ReadLine();
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler encountered an exception {arg.Exception}.");
            var context = arg.ExceptionReceivedContext;
            Console.WriteLine($"- Endpoint: {context.Endpoint}, Path: {context.EntityPath}, Action: {context.Action}");
            return Task.CompletedTask;
        }

        private static async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            if (_queueClient.IsClosedOrClosing)
                return;

            if (message.Label != _storeId)
            {
                Console.WriteLine($"Message From Store: {message.Label} with id {message.MessageId} not processed");
                return;
            }

            var productChangesString = Encoding.UTF8.GetString(message.Body);
            var productChanges = JsonConvert.DeserializeObject<ProductsChanged>(productChangesString);

            //here message is actually processed
            Thread.Sleep(1500);

            Console.WriteLine($"Message Processed: {productChangesString}");
            Console.WriteLine($"task {count++}");

            Task PendingCompleteTask;
            lock (PendingCompleteTasks)
            {
                PendingCompleteTasks.Add(_queueClient.CompleteAsync(message.SystemProperties.LockToken));
                PendingCompleteTask = PendingCompleteTasks.LastOrDefault();
            }

            Console.WriteLine($"calling complete for task {count}");

            await PendingCompleteTask;

            Console.WriteLine($"remove task {count} from task queue");
            PendingCompleteTasks.Remove(PendingCompleteTask);
        }
    }
}

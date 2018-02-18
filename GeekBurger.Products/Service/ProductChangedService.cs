using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using GeekBurger.Products.Repository;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Products.Service
{
    public class ProductChangedService : IProductChangedService
    {
        private IConfiguration _configuration;
        private IMapper _mapper;
        private List<Message> _messages;
        private Task _lastTask;

        public ProductChangedService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            _messages = new List<Message>();
        }
        public void AddToMessageList(IEnumerable<EntityEntry<Product>> changes)
        {
            _messages.AddRange(changes
            .Where(entity => entity.State != EntityState.Detached
                    && entity.State != EntityState.Unchanged)
            .Select(entity => GetMessage(entity)).ToList());
        }

        public Message GetMessage(EntityEntry<Product> entity)
        {
            var productChanged = Mapper.Map<ProductChangedMessage>(entity);
            var productChangedSerialized = JsonConvert.SerializeObject(productChanged);
            var productChangedByteArray = Encoding.UTF8.GetBytes(productChangedSerialized);

            return new Message
            {
                Body = productChangedByteArray,
                MessageId = Guid.NewGuid().ToString(),
                Label = productChanged.Product.StoreId.ToString()
            };
        }

public async void SendMessagesAsync()
{
    if (_lastTask != null && !_lastTask.IsCompleted)
        return;

    var connectionString = _configuration["connectionStrings:serviceBusConnectionString"];
    var queueClient = new QueueClient(connectionString, "ProductChanged");

    _lastTask = SendAsync(queueClient);

    await _lastTask;

    var closeTask = queueClient.CloseAsync();
    await closeTask;
    HandleException(closeTask);
}

public async Task SendAsync(QueueClient queueClient)
{
    int tries = 0;
    Message message;
    while (true)
    {
        if (_messages.Count <= 0)
            break;

        lock (_messages) {
            message = _messages.FirstOrDefault();
        }

        var sendTask = queueClient.SendAsync(message);
        await sendTask;
        var success = HandleException(sendTask);

        if (!success) 
            Thread.Sleep(10000 * (tries<60?tries++:tries));
        else
            _messages.Remove(message);
    }
}

        public bool HandleException(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList().ForEach(innerException =>
            {
                Console.WriteLine($"Error in SendAsync task: {innerException.Message}. Details:{innerException.StackTrace} ");

                if (innerException is ServiceBusCommunicationException)
                    Console.WriteLine("Connection Problem with Host. Internet Connection can be down");
            });

            return false;
        }
    }
}
using Achievements.Hubs;
using Achievements.Models;
using Achievements.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Achievements
{
    public class EventListener : IHostedService
    {
        private IQueueClient _queueClient;
        private UserAchievementsRepository _repository;
        private IConfiguration _configuration;
        private IHubContext<AchievementsHub> _hubContext;

        public EventListener(IConfiguration configuration, IHubContext<AchievementsHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var serviceBusConnectionString = _configuration.GetConnectionString("ServiceBus");
            var databaseConnectionString = _configuration.GetConnectionString("Database");

            _repository = new UserAchievementsRepository(databaseConnectionString);
            _queueClient = new QueueClient(serviceBusConnectionString, "unlockedachievements");

            RegisterOnMessageHandlerAndReceiveMessages();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _queueClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,
                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var userAchievement = JsonConvert.DeserializeObject<UserAchievement<string>>(Encoding.UTF8.GetString(message.Body));

            await _repository.Add(userAchievement.UserId, userAchievement.Achievement.Id);

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            // Note: Use the cancellationToken passed as necessary to determine if the _queueClient has already been closed.
            // If _queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
            await _hubContext.Clients.All.SendAsync("OnAchievement", userAchievement);
        }

        // Use this handler to examine the exceptions received on the message pump.
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
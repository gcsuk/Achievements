using Achievements.Hubs;
using Achievements.Models;
using Achievements.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Achievements.Events
{
    public class EventListener : IHostedService
    {
        private readonly SubscriptionClient _subscriptionClient;
        private static IUserAchievementsRepository<string> _userAchievementsRepository;
        private readonly IRepository<Achievement, int> _achievementsRepository;
        private readonly IHubContext<AchievementsHub> _hubContext;

        public EventListener(IConfiguration configuration, IHubContext<AchievementsHub> hubContext,
            IUserAchievementsRepository<string> userAchievementsRepository, IRepository<Achievement, int> achievementsRepository)
        {
            _hubContext = hubContext;
            _userAchievementsRepository = userAchievementsRepository;
            _achievementsRepository = achievementsRepository;
            _subscriptionClient = new SubscriptionClient(configuration.GetConnectionString("ServiceBus"), "platformactivity", "Achievements", ReceiveMode.ReceiveAndDelete);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterOnMessageHandlerAndReceiveMessages();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _subscriptionClient.CloseAsync();
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
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var userAchievement = JsonConvert.DeserializeObject<AchievementUnlockedEvent>(Encoding.UTF8.GetString(message.Body));

            var existingAchievements = await _userAchievementsRepository.GetForUserId(userAchievement.UserId);

            if (existingAchievements.All(a => a.Achievement.Id != userAchievement.AchievementId))
            {
                await _userAchievementsRepository.Add(userAchievement.UserId, userAchievement.AchievementId);

                var achievement = await _achievementsRepository.GetByID(userAchievement.AchievementId);

                await _hubContext.Clients.User(userAchievement.UserId).SendAsync("Unlocked", achievement, token);
            }
        }

        // Use this handler to examine the exceptions received on the message pump.
        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
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
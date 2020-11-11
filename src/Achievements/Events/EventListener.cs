using Achievements.Hubs;
using Achievements.Models.Entities;
using Achievements.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Achievements.Events
{
    public class EventListener : IHostedService
    {
        private static IQueueClient _queueClient;
        private static IRepository<UnlockedAchievementEntity> _unlockedAchievementsRepository;
        private readonly IRepository<AchievementEntity> _achievementsRepository;
        private readonly IHubContext<AchievementsHub> _hubContext;

        public EventListener(IQueueClient queueClient, IHubContext<AchievementsHub> hubContext,
            IRepository<UnlockedAchievementEntity> unlockedAchievementsRepository, IRepository<AchievementEntity> achievementsRepository)
        {
            _hubContext = hubContext;
            _unlockedAchievementsRepository = unlockedAchievementsRepository;
            _achievementsRepository = achievementsRepository;
            _queueClient = queueClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterOnMessageHandlerAndReceiveMessages();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _queueClient.CloseAsync();
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,
                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = true
            };

            // Register the function that processes messages.
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var userAchievement = JsonConvert.DeserializeObject<AchievementUnlockedEvent>(Encoding.UTF8.GetString(message.Body));

            // ********************************************************
            // This would normally use a rules engine of some kind here
            // ********************************************************

            var existingUnlockedAchievement = await _unlockedAchievementsRepository.GetEntity(userAchievement.UserId, userAchievement.Achievement);

            // Only update if it isn't there already, otherwise it would set IsNew = true on existing records
            if (existingUnlockedAchievement == null)
            {
                var newAchievement = new UnlockedAchievementEntity
                {
                    PartitionKey = userAchievement.UserId,
                    RowKey = userAchievement.Achievement,
                    IsNew = true
                };

                await _unlockedAchievementsRepository.InsertOrMergeEntity(newAchievement);

                var achievementDetails = await _achievementsRepository.GetEntity("1", newAchievement.RowKey);

                var response = new
                {
                    Name = achievementDetails.RowKey,
                    achievementDetails.Description
                };

                await _hubContext.Clients.User(userAchievement.UserId).SendAsync("Unlocked", response, token);
            }
        }

        // Use this handler to examine the exceptions received on the message pump.
        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            Log.Debug("Achievements Exception: {@context}", context);

            return Task.CompletedTask;
        }
    }
}
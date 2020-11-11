using Achievements.Events;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Achievements.Demo
{
    public class EventSender : IEventSender
    {
        private readonly IQueueClient _queueClient;

        public EventSender(IQueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public async Task Send(IAchievementEvent achievement)
        {
            try
            {
                // Create a new message to send to the queue (serialise the payload)
                var message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(achievement)));

                message.UserProperties.Add("userId", achievement.UserId);

                // Send the message to the topic
                await _queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        // This should be called from an IHostedService.StopAsync some where
        // or the IQueueClient should be shared between sending and listener
        public async Task CloseAsync()
        {
            await _queueClient.CloseAsync();
        }
    }
}
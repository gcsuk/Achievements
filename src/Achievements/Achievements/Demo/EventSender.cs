using Achievements.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Achievements.Demo
{
    public class EventSender
    {
        private readonly QueueClient _queueClient;

        public EventSender(IConfiguration configuration)
        {
            _queueClient = new QueueClient(configuration.GetConnectionString("ServiceBus"), "unlockedachievements");
        }

        public async Task Send(AchievementUnlockedEvent achievement)
        {
            try
            {
                // Create a new message to send to the queue (serialise the payload)
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(achievement)));

                // Send the message to the queue
                await _queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        // This should be called from an IHostedService.StopAsync some where
        // or the IQueueClient should be shared between sending and listener
        public Task CloseAsync()
        {
            return _queueClient.CloseAsync();
        }
    }
}
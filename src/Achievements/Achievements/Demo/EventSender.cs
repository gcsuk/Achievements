using Achievements.Events;
using Achievements.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Achievements.Demo
{
    public class EventSender
    {
        public static async Task Send(string connectionString)
        {
            var queueClient = new QueueClient(connectionString, "unlockedachievements");

            try
            {
                var achievement = new AchievementUnlockedEvent
                {
                    UserId = "1",
                    Achievement = new Achievement
                    {
                        Id = 1
                    }
                };

                // Create a new message to send to the queue (serialise the payload)
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(achievement)));
                // Send the message to the queue
                await queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
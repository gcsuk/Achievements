using System.Threading.Tasks;
using Achievements.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Achievements.Demo
{
    public class DemoController : Controller
    {
        private readonly IEventSender _eventSender;

        public DemoController(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }

        /// <summary>
        /// A test API for sending an event that the app listens for
        /// </summary>
        [HttpPost("/demo/events/send")]
        public async Task<IActionResult> SendEvent()
        {
            var achievement = new AchievementUnlockedEvent
            {
                UserId = "1",
                AchievementId = 1
            };
            
            await _eventSender.Send(achievement);

            return NoContent();
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Achievements.Demo
{
    public class DemoController : Controller
    {
        private readonly IConfiguration _configuration;

        public DemoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// A test API for sending an event that the app listens for
        /// </summary>
        [HttpPost("/demo/events/send")]
        public async Task<IActionResult> SendEvent()
        {
            await EventSender.Send(_configuration.GetConnectionString("ServiceBus"));

            return NoContent();
        }
    }
}
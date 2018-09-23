using System.Collections.Generic;
using Achievements.Responses;
using Achievements.Repositories;
using Microsoft.AspNetCore.Mvc;
using Achievements.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Achievements.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAchievementsController : ControllerBase
    {
        private readonly IUserAchievementsRepository<string> _repository;
        private readonly IRepository<Achievement, int> _achievementsRepository;

        public UserAchievementsController(IUserAchievementsRepository<string> repository, IRepository<Achievement, int> achievementsRepository)
        {
            _repository = repository;
            _achievementsRepository = achievementsRepository;
        }

        // GET api/userachievements/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<GetUserAchievementsResponse>>> GetForUser(string userId)
        {
            var achievements = await _repository.GetForUserId(userId);

            var response = achievements.Select(a => new GetUserAchievementsResponse
            {
                UserId = a.UserId,
                Achievement = a.Achievement,
                DateUnlocked = a.DateUnlocked,
                IsNew = a.IsNew
            });

            return Ok(response);
        }

        // POST api/userachievements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserAchievementRequest request)
        {
            await _repository.Add(request.UserId, request.AchievementId);

            return NoContent();
        }

        // DELETE api/userachievements/5/users/12345
        [HttpDelete("{achievementId}/users/{userId}")]
        public async Task<IActionResult> Delete(string userId, int achievementId)
        {
            await _repository.Delete(userId, achievementId);

            return NoContent();
        }
    }
}

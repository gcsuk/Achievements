using System.Collections.Generic;
using Achievements.Responses;
using Achievements.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Achievements.Models.Entities;

namespace Achievements.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnlockedAchievementsController : ControllerBase
    {
        private readonly IRepository<AchievementEntity> _achievementsRepository;
        private readonly IRepository<UnlockedAchievementEntity> _unlockedAchievementsRepository;

        public UnlockedAchievementsController(IRepository<AchievementEntity> achievementsRepository, IRepository<UnlockedAchievementEntity> unlockedAchievementsRepository)
        {
            _achievementsRepository = achievementsRepository;
            _unlockedAchievementsRepository = unlockedAchievementsRepository;
        }

        // GET api/userachievements/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<GetUserAchievementsResponse>>> GetForUser(string userId)
        {
            var unlockedAchievements = await _unlockedAchievementsRepository.GetEntities(userId);
            var availableAchievements = (await _achievementsRepository.GetEntities("1")).ToList();

            var response = new GetUserAchievementsResponse
            {
                UserId = userId,
                Achievements = unlockedAchievements.Select(a => new GetUserAchievementsResponse.AchievementModel
                {
                    Category = availableAchievements.SingleOrDefault(av => av.RowKey == a.RowKey)?.Category,
                    Name = a.RowKey,
                    Description = availableAchievements.SingleOrDefault(av => av.RowKey == a.RowKey)?.Description,
                    DateUnlocked = a.Timestamp.DateTime,
                })
            };

            return Ok(response);
        }

        // POST api/userachievements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserAchievementRequest request)
        {
            var unlockedAchievement = new UnlockedAchievementEntity
            {
                PartitionKey = request.UserId,
                RowKey = request.AchievementName,
                ETag = "*",
                IsNew = true
            };

            await _unlockedAchievementsRepository.InsertOrMergeEntity(unlockedAchievement);

            return NoContent();
        }

        // DELETE api/userachievements/5/users/12345
        [HttpDelete("{userId}/{achievementName}")]
        public async Task<IActionResult> Delete(string userId, string achievementName)
        {
            var unlockedAchievement = await _unlockedAchievementsRepository.GetEntity(userId, achievementName);

            await _unlockedAchievementsRepository.DeleteEntity(unlockedAchievement);

            return NoContent();
        }
    }
}

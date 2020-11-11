using System.Collections.Generic;
using Achievements.Responses;
using Achievements.Repositories;
using Microsoft.AspNetCore.Mvc;
using Achievements.Models;
using System.Linq;
using System.Threading.Tasks;
using Achievements.Models.Entities;
using System;

namespace Achievements.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IRepository<AchievementEntity> _repository;

        public AchievementsController(IRepository<AchievementEntity> repository)
        {
            _repository = repository;
        }

        // GET api/achievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAchievementsResponse>>> Get()
        {
            var achievements = await _repository.GetEntities("1");

            var response = achievements.Select(a => new GetAchievementsResponse
            {
                Name = a.RowKey,
                Category = a.Category,
                Description = a.Description,
                IsSecret = a.IsSecret
            });

            return Ok(response);
        }

        // GET api/achievements/5
        [HttpGet("{name}")]
        public async Task<ActionResult<GetAchievementsResponse>> Get(string name)
        {
            var achievement = await _repository.GetEntity("1", name);

            if (achievement == null)
                return NotFound();

            var response = new GetAchievementsResponse
            {
                Category = achievement.Category,
                Name = achievement.RowKey,
                Description = achievement.Description,
                IsSecret = achievement.IsSecret
            };

            return Ok(response);
        }

        // PUT api/achievements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddAchievementRequest request)
        {
            var achievement = new AchievementEntity
            {
                PartitionKey = "1",
                RowKey = request.Name,
                Category = request.Category,
                Description = request.Details,
                IsSecret = request.IsSecret
            };

            await _repository.InsertOrMergeEntity(achievement);

            return NoContent();
        }

        // PUT api/achievements/5
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] UpdateAchievementRequest request)
        {
            var achievement = await _repository.GetEntity("1", name);

            if (achievement == null)
                return BadRequest("There is no achievement matching that name");

            achievement.Category = request.Category;
            achievement.Description = request.Details;
            achievement.IsSecret = request.IsSecret;

            await _repository.InsertOrMergeEntity(achievement);

            return NoContent();
        }

        // DELETE api/achievements/5
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            var achievement = await _repository.GetEntity("1", name);

            if (achievement == null)
                return BadRequest("There is no achievement matching that name");

            await _repository.DeleteEntity(achievement);

            return NoContent();
        }
    }
}

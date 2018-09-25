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
    public class AchievementsController : ControllerBase
    {
        private readonly IRepository<Achievement, int> _repository;

        public AchievementsController(IRepository<Achievement, int> repository)
        {
            _repository = repository;
        }

        // GET api/achievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAchievementsResponse>>> Get()
        {
            var achievements = await _repository.GetAll();

            var response = achievements.Select(a => new GetAchievementsResponse
            {
                Id = a.Id,
                Category = a.Category.Name,
                Name = a.Name,
                Details = a.Details,
                IsSecret = a.IsSecret
            });

            return Ok(response);
        }

        // GET api/achievements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetAchievementsResponse>> Get(int id)
        {
            var achievement = await _repository.GetByID(id);

            var response = new GetAchievementsResponse
            {
                Id = achievement.Id,
                Category = achievement.Category.Name,
                Name = achievement.Name,
                Details = achievement.Details,
                IsSecret = achievement.IsSecret
            };

            return Ok(response);
        }

        // POST api/achievements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddAchievementRequest request)
        {
            var achievement = new Achievement
            {
                Category = new Category { Id = request.CategoryId },
                Name = request.Name,
                Details = request.Details,
                IsSecret = request.IsSecret
            };

            await _repository.Add(achievement);

            return NoContent();
        }

        // PUT api/achievements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateAchievementRequest request)
        {
            var achievement = new Achievement
            {
                Id = id,
                Category = new Category { Id = request.CategoryId },
                Name = request.Name,
                Details = request.Details,
                IsSecret = request.IsSecret
            };

            await _repository.Update(achievement);

            return NoContent();
        }

        // DELETE api/achievements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.Delete(id);

            return NoContent();
        }
    }
}

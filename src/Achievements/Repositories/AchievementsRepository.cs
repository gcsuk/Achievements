using Achievements.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace Achievements.Repositories
{
    public class AchievementsRepository : TableStorageRepository<AchievementEntity>, IRepository<AchievementEntity>
    {
        private const string TableName = "Achievements";

        public AchievementsRepository(IConfiguration config) : base(config, TableName)
        {
        }
    }
}
using Achievements.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace Achievements.Repositories
{
    public class UnlockedAchievementsRepository : TableStorageRepository<UnlockedAchievementEntity>, IRepository<UnlockedAchievementEntity>
    {
        private const string TableName = "AchievementsUnlocked";

        public UnlockedAchievementsRepository(IConfiguration config) : base(config, TableName)
        {
        }
    }
}
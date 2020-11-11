using Microsoft.Azure.Cosmos.Table;

namespace Achievements.Models.Entities
{
    public class UnlockedAchievementEntity : TableEntity
    {
        public UnlockedAchievementEntity()
        {
            ETag = "*";
        }

        public bool IsNew { get; set; }
    }
}
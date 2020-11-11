using Microsoft.Azure.Cosmos.Table;

namespace Achievements.Models.Entities
{
    public class AchievementEntity : TableEntity
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsSecret { get; set; }
    }
}

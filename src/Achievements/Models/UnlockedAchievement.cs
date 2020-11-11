using System;

namespace Achievements.Models
{
    public class UnlockedAchievement
    {
        public string UserId { get; set; }
        public string AchievementName { get; set; }
        public DateTimeOffset DateUnlocked { get; set; }
        public bool IsNew { get; set; }
    }
}
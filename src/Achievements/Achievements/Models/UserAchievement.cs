using System;

namespace Achievements.Models
{
    public class UserAchievement<Key>
    {
        public Key UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime DateUnlocked { get; set; }
    }
}
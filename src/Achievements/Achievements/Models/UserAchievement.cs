using System;

namespace Achievements.Models
{
    public class UserAchievement<Key>
    {
        public Key UserId { get; set; }
        public Achievement Achievement { get; set; }
        public DateTime DateUnlocked { get; set; }
        public bool IsNew { get; set; }
    }
}
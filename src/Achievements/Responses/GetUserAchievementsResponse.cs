using System;
using System.Collections.Generic;

namespace Achievements.Responses
{
    public class GetUserAchievementsResponse
    {
        public string UserId { get; set; }
        public IEnumerable<AchievementModel> Achievements { get; set; }

        public class AchievementModel
        {
            public string Category { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime DateUnlocked { get; set; }
        }
    }
}
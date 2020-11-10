using Achievements.Models;
using System;

namespace Achievements.Responses
{
    public class GetUserAchievementsResponse
    {
        public string UserId { get; set; }
        public Achievement Achievement { get; set; }
        public DateTime DateUnlocked{ get; set; }
        public bool IsNew { get; set; }
    }
}
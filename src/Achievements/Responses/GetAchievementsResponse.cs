﻿namespace Achievements.Responses
{
    public class GetAchievementsResponse
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsSecret { get; set; }
    }
}
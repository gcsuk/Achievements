namespace Achievements.Responses
{
    public class UpdateAchievementRequest
    {
        public string Category { get; set; }
        public string Details { get; set; }
        public bool IsSecret { get; set; }
    }
}
namespace Achievements.Responses
{
    public class AddAchievementRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Details { get; set; }
        public bool IsSecret { get; set; }
    }
}
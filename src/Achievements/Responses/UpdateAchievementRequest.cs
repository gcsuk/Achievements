namespace Achievements.Responses
{
    public class UpdateAchievementRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsSecret { get; set; }
    }
}
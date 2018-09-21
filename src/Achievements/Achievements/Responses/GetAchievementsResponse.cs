namespace Achievements.Responses
{
    public class GetAchievementsResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsSecret { get; set; }
    }
}
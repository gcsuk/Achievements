namespace Achievements.Events
{
    public class AchievementUnlockedEvent : IAchievementEvent
    {
        public string UserId { get; set; }
        public int AchievementId { get; set; }
    }
}
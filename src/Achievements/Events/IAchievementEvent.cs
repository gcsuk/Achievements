namespace Achievements.Events
{
    public interface IAchievementEvent
    {
        string UserId { get; set; }
        int AchievementId { get; set; }
    }
}
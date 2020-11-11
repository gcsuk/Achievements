namespace Achievements.Events
{
    public interface IAchievementEvent
    {
        string UserId { get; set; }
        string Achievement { get; set; }
    }
}
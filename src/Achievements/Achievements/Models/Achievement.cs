namespace Achievements.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsSecret { get; set; }
    }
}

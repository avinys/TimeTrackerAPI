namespace TimeTrackerAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int Correction { get; set; }
        public bool IsCompleted { get; set; } = false;
        public ICollection<ProjectTime> ProjectTimes { get; set; } = new List<ProjectTime>();
    }
}

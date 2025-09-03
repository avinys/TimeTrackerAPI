namespace TimeTrackerAPI.Models
{
    public class ProjectTime
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Comment { get; set; }
        public Project Project { get; set; } = default!;
    }
}

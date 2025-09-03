namespace TimeTrackerAPI.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Name { get; set; }
        public int Correction { get; set; }
        public bool IsRunning {  get; set; }
    }
}

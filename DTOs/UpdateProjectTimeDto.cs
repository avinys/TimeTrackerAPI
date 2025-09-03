using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class UpdateProjectTimeDto
    {
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? Comment { get; set; }
    }
}

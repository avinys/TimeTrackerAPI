using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class UpdateProjectTimeDto
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }
    }
}

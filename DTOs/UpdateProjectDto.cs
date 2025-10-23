using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class UpdateProjectDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        public bool? IsCompleted { get; set; }
    }
}

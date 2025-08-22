using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class UpdateProjectTimeDto
    {
        [Required]
        public int Id {  get; set; }
        [Required]
        public DateTime StartTime { get; set; }
    }
}

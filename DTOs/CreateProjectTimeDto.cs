using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class CreateProjectTimeDto
    {
        [Required]
        public int ProjectId { get; set; }
        public int? UserId { get; set; }
    }
}

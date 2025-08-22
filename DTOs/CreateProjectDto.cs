using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}

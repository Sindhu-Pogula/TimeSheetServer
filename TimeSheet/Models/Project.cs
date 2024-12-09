using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? ProjectName { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }
    }
}

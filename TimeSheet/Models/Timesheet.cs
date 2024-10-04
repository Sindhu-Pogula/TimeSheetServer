// Models/Timesheet.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSheet.Models
{
    public class Timesheet
    {
        public int Id { get; set; }

        // Foreign key for the User
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; } // Navigation property

        [Required]
        public DateTime FromDate { get; set; }

       [Required]
        public DateTime ToDate { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Project { get; set; }

        [Range(0, 24)]
        public decimal Monday { get; set; }

        [Range(0, 24)]
        public decimal Tuesday { get; set; }

        [Range(0, 24)]
        public decimal Wednesday { get; set; }

        [Range(0, 24)]
        public decimal Thursday { get; set; }

        [Range(0, 24)]
        public decimal Friday { get; set; }

        [Range(0, 24)]
        public decimal Saturday { get; set; }

        [Range(0, 24)]
        public decimal Sunday { get; set; }

        [Range(0, 24)]
        public decimal TotalHours { get; set; }

    }
}

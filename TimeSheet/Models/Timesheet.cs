// Models/Timesheet.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Models
{
    public class Timesheet
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Project { get; set; }

        [Range(0, 24)]
        public float Monday { get; set; }

        [Range(0, 24)]
        public float Tuesday { get; set; }

        [Range(0, 24)]
        public float Wednesday { get; set; }

        [Range(0, 24)]
        public float Thursday { get; set; }

        [Range(0, 24)]
        public float Friday { get; set; }

        [Range(0, 24)]
        public float Saturday { get; set; }

        [Range(0, 24)]
        public float Sunday { get; set; }

        [Range(0, 24)]
        public float TotalHours { get; set; }
    }
}

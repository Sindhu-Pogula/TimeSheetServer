using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSheet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("UserName")] // If the column in the database is "UserName" instead of "Username"
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [Column("UserEmail")] // If the column in the database is "UserEmail" instead of "Email"
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 16 characters long.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{8,16}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character, with no spaces.")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }
       // [Required]
        public string Role { get; set; } = "User"; // Default to "User"

    }
}

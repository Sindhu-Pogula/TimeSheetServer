using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Models
{
    public class UserDashboard
    {
        [Key]
        public string Username { get; set; }
        public string Email { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace LeaveManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public int MaxDaysAllowed { get; set; } = 25; 

        public bool IsAdmin { get; internal set; }

        public bool IsEmployee { get; set; }
    }

}

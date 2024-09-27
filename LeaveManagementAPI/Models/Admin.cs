using System.ComponentModel.DataAnnotations;

namespace LeaveManagementAPI.Models
{
    public class Admin : User
    {
        [Required]
        public string Role { get; set; } = "admin";
    }
}

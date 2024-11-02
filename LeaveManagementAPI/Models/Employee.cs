using System.ComponentModel.DataAnnotations;

namespace LeaveManagementAPI.Models
{
    public class Employee : User
    {
        [Required]
        public string Department { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public string position { get; set; }    

    }
}

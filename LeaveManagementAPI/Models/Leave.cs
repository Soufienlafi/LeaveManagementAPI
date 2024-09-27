using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementAPI.Models
{
    public class Leave
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        [ForeignKey("LeaveType")]
        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum LeaveType
    {
        CasualLeave,
        SickLeave,
        MaternityLeave,
    }
}

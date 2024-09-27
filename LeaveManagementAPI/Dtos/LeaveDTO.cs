namespace LeaveManagementAPI.Models.Dtos
{
    public class LeaveDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public int LeaveTypeId { get; set; }

    }
}

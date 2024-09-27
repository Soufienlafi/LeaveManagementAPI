namespace LeaveManagementAPI.Dtos
{
    public class LoginResponseDTO

    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmployee { get; set; }
        public int? EmployeeId { get; set; }
    }
}

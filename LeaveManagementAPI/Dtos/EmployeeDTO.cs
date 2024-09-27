namespace LeaveManagementAPI.Models.Dtos
{
    public class EmployeeDTO : UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }

            

    }
}

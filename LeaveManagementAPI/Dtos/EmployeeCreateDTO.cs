namespace LeaveManagementAPI.Dtos
{
    public class EmployeeCreateDTO : UserCreateDTO
    {
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
    }
}

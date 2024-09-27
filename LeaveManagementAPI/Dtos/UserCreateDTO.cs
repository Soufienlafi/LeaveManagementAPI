namespace LeaveManagementAPI.Dtos
{
    public class UserCreateDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int MaxDaysAllowed { get; set; }
    }
}

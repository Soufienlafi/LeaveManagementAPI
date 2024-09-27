namespace LeaveManagementAPI.Models.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int MaxDaysAllowed { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmployee { get; set; }
        public string Token { get; set; }

        public string Department { get; set; }   
        public string FirstName { get; set; }   
        public string LastName { get; set; }    
        public string Position { get; set; }   

    }
}

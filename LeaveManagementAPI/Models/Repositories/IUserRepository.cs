using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagementAPI.Models;


namespace LeaveManagementAPI.Models.Repositories

{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByNameAsync(string name);
        Task<User> UpdateUserAsync(User user);  
        Task<bool> DeleteUserAsync(int id);
        Task DeleteEmployeeAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Admin> AddAdminAsync(Admin admin);
        Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
        bool IsAdmin(int userId);
        bool IsEmployee(int userId);
        Task<Admin> GetAdminByIdAsync(int id);
        Task UpdateAdminAsync(Admin admin);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task UpdateEmployeeAsync(Employee employee);

    }
}

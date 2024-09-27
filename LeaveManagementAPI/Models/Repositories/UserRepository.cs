using Microsoft.EntityFrameworkCore;

namespace LeaveManagementAPI.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch(Exception ex )
            {
                _logger.LogError($"Error adding employee : {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<Admin> AddAdminAsync(Admin admin)
        {
            try
            {
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
                return admin;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error adding Admin :{ex.Message}\n{ex.StackTrace}");
                throw;
            }

        }

       /* public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding user: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }*/

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user by ID: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.UserName == name);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user by name: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }


        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

           
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }


        public bool IsAdmin(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user is Admin;
        }

        public bool IsEmployee(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user is Employee;
        }


        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Users
                .Where(u => u.IsEmployee)  
                .Select(u => new Employee  
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Department = (u as Employee).Department,  
                    firstName = (u as Employee).firstName,  
                    lastName = (u as Employee).lastName,    
                    position = (u as Employee).position,  
                    MaxDaysAllowed = u.MaxDaysAllowed,
                    IsAdmin = u.IsAdmin,
                    IsEmployee = u.IsEmployee
                })
                .ToListAsync();
        }


    }
}

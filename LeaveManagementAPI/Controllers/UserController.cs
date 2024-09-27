using AutoMapper;
using BCrypt.Net;
using LeaveManagementAPI.Models;
using LeaveManagementAPI.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using LeaveManagementAPI.Models.Dtos;
using  LeaveManagementAPI.Dtos;

 using System.IdentityModel.Tokens.Jwt;
using  System.Security.Claims;
using  Microsoft.IdentityModel.Tokens;
using System.Text ;

namespace LeaveManagementAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IMapper mapper , IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                var users = await userRepository.GetAllUsersAsync();
                var userDTOs = mapper.Map<IEnumerable<UserDTO>>(users);
                return Ok(userDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving users: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                if (user is Employee employee)
                {
                    var employeeDTO = mapper.Map<EmployeeDTO>(employee);
                    return Ok(employeeDTO);
                }

                var userDTO = mapper.Map<UserDTO>(user);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving user by ID {id}: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("username/{name}")]
        public async Task<ActionResult<UserDTO>> GetUserByName(string name)
        {
            try
            {
                var user = await userRepository.GetUserByNameAsync(name);

                if (user == null)
                    return NotFound($"User with username '{name}' not found.");

                var userDTO = mapper.Map<UserDTO>(user);
                return Ok(userDTO);
            }
            catch (Exception )
            {
              
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }



        [HttpPost("employee")]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee(EmployeeCreateDTO employeeCreateDTO)
        {
            try
            {
                if (employeeCreateDTO == null)
                    return BadRequest();

                 var hashedPassword = BCrypt.Net.BCrypt.HashPassword(employeeCreateDTO.Password);
                var employee = mapper.Map<Employee>(employeeCreateDTO);
                 employee.Password = hashedPassword;
                employee.IsEmployee = true; 
                var createdEmployee = await userRepository.AddEmployeeAsync(employee);
                 var createdEmployeeDTO = mapper.Map<EmployeeDTO>(createdEmployee);
                return CreatedAtAction(nameof(GetUserById), new { id = createdEmployeeDTO.Id }, createdEmployeeDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error adding employee: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding data to the database");
            }
        }


        [HttpPost("admin")]
        public async Task<ActionResult<AdminDTO>> AddAdmin(AdminCreateDTO adminCreateDTO)
        {
            try
            {
                if (adminCreateDTO == null)
                    return BadRequest("Invalid data.");

                var admin = mapper.Map<Admin>(adminCreateDTO);
                admin.Password = BCrypt.Net.BCrypt.HashPassword(adminCreateDTO.Password);

                admin.IsAdmin = true;
                admin.IsEmployee = false; 
                admin.Role = "admin";

                var createdAdmin = await userRepository.AddAdminAsync(admin);
                var createdAdminDTO = mapper.Map<AdminDTO>(createdAdmin);
                return CreatedAtAction(nameof(GetUserById), new { id = createdAdminDTO.Id }, createdAdminDTO);
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding data to the database.");
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            try
            {
                if (id != userUpdateDTO.Id)
                    return BadRequest("User ID mismatch");

                var userToUpdate = await userRepository.GetUserByIdAsync(id);
                if (userToUpdate == null)
                    return NotFound($"User with ID = {id} not found");

                if (!string.IsNullOrEmpty(userUpdateDTO.Password))
                {
                    userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(userUpdateDTO.Password);
                }

                mapper.Map(userUpdateDTO, userToUpdate);

                var updatedUser = await userRepository.UpdateUserAsync(userToUpdate);
                var updatedUserDTO = mapper.Map<UserDTO>(updatedUser);

                return Ok(updatedUserDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error updating user: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var userToDelete = await userRepository.GetUserByIdAsync(id);
                if (userToDelete == null)
                    return NotFound($"User with ID = {id} not found");

                await userRepository.DeleteUserAsync(id);
                return Ok($"User with ID = {id} deleted");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error deleting user: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }


        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeUpdateDTO)
        {
            if (id != employeeUpdateDTO.Id)
            {
                return BadRequest("Employee ID mismatch.");
            }

            try
            {
                var existingEmployee = await userRepository.GetEmployeeByIdAsync(id);

                            if (existingEmployee == null)
                {
                    return NotFound("Employee not found.");
                }

                existingEmployee.UserName = employeeUpdateDTO.UserName;
                existingEmployee.Email = employeeUpdateDTO.Email;
                existingEmployee.Department = employeeUpdateDTO.Department;
                existingEmployee.firstName = employeeUpdateDTO.FirstName;
                existingEmployee.lastName = employeeUpdateDTO.LastName;
                existingEmployee.position = employeeUpdateDTO.Position;
                if (!string.IsNullOrEmpty(employeeUpdateDTO.Password))
                {
                    existingEmployee.Password = BCrypt.Net.BCrypt.HashPassword(employeeUpdateDTO.Password);
                }

                
                await userRepository.UpdateEmployeeAsync(existingEmployee);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database.");
            }
        }




        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            logger.LogInformation("Attempting login for user: {Username}", loginDTO.Username);

            var user = await userRepository.GetUserByUsernameAndPasswordAsync(loginDTO.Username, loginDTO.Password);
    
            if (user == null)
            {
                logger.LogWarning("Login failed for user: {Username}", loginDTO.Username);
                return Unauthorized("Invalid username or password");
            }

            logger.LogInformation("Login successful for user: {Username}", loginDTO.Username);

            var userDTO = mapper.Map<UserDTO>(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Employee")
        }),
                 Expires = DateTime.UtcNow.AddHours(1),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            userDTO.Token = tokenHandler.WriteToken(token);

            return Ok(userDTO);
        }


            [HttpGet("{userId}/isAdmin")]
        public IActionResult IsAdmin(int userId)
        {
            var isAdmin = userRepository.IsAdmin(userId);
             return Ok(new { IsAdmin = isAdmin });
        }

        [HttpGet("{userId}/isEmployee")]
        public IActionResult IsEmployee(int userId)
        {
             var isEmployee = userRepository.IsEmployee(userId);
            return Ok(new { IsEmployee = isEmployee });
        }


        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAllEmployees()
        {
            try
            {
                var employees = await userRepository.GetAllEmployeesAsync();
                var employeeDTOs = mapper.Map<IEnumerable<EmployeeDTO>>(employees);
                return Ok(employeeDTOs);
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPut("update-admin/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDto adminUpdateDTO)
        {
            if (id != adminUpdateDTO.Id)
            {
                return BadRequest("Admin ID mismatch.");
            }

            try
            {
                var existingAdmin = await userRepository.GetAdminByIdAsync(id);

                if (existingAdmin == null)
                {
                    return NotFound("Admin not found.");
                }

               
                existingAdmin.UserName = adminUpdateDTO.UserName;
                existingAdmin.Email = adminUpdateDTO.Email;

               
                if (!string.IsNullOrEmpty(adminUpdateDTO.Password))
                {
                    existingAdmin.Password = BCrypt.Net.BCrypt.HashPassword(adminUpdateDTO.Password);
                }

                await userRepository.UpdateAdminAsync(existingAdmin);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating admin in the database.");
            }
        }


    }
}

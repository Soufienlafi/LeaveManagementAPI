using AutoMapper;
using LeaveManagementAPI.Models;
using LeaveManagementAPI.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using LeaveManagementAPI.Models.Dtos;

namespace LeaveManagementAPI.Controllers
{
    [Route("api/leaves")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRepository leaveRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger<LeaveController> logger;

        public LeaveController(ILeaveRepository leaveRepository, IUserRepository userRepository, IMapper mapper, ILogger<LeaveController> logger)
        {
            this.leaveRepository = leaveRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveDTO>>> GetAllLeaves()
        {
            try
            {
                var leaves = await leaveRepository.GetAllLeavesAsync();
                var leaveDTOs = mapper.Map<IEnumerable<LeaveDTO>>(leaves);
                return Ok(leaveDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving data from the database: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveDTO>> GetLeaveById(int id)
        {
            try
            {
                var leave = await leaveRepository.GetLeaveByIdAsync(id);
                if (leave == null)
                    return NotFound();

                var leaveDTO = mapper.Map<LeaveDTO>(leave);
                return Ok(leaveDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving data from the database: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<LeaveDTO>>> GetLeavesByEmployeeId(int employeeId)
        {
            try
            {
                var leaves = await leaveRepository.GetLeavesByEmployeeIdAsync(employeeId);
                var leaveDTOs = mapper.Map<IEnumerable<LeaveDTO>>(leaves);
                return Ok(leaveDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving data from the database: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LeaveDTO>> AddLeave(LeaveDTO leaveDTO)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(leaveDTO.EmployeeId);
                if (user == null)
                {
                    return BadRequest("Invalid Employee ID");
                }

                var numberOfLeaveDays = (leaveDTO.EndDate - leaveDTO.StartDate).Days + 1;

                if (numberOfLeaveDays == 1 && leaveDTO.StartDate.DayOfWeek == DayOfWeek.Friday)
                {
                    numberOfLeaveDays += 2;
                }

                if (user.MaxDaysAllowed < numberOfLeaveDays)
                {
                    return BadRequest("Not enough leave days available");
                }

                user.MaxDaysAllowed -= numberOfLeaveDays;
                await userRepository.UpdateUserAsync(user);

                var leave = mapper.Map<Leave>(leaveDTO);
                var addedLeave = await leaveRepository.AddLeaveAsync(leave);

                var createdLeaveDTO = mapper.Map<LeaveDTO>(addedLeave);
                return CreatedAtAction(nameof(GetLeaveById), new { id = createdLeaveDTO.Id }, createdLeaveDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error adding data to the database: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding data to the database");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLeave(int id)
        {
            try
            {
                var leaveToDelete = await leaveRepository.GetLeaveByIdAsync(id);
                if (leaveToDelete == null)
                {
                    return NotFound();
                }

                await leaveRepository.DeleteLeaveAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error deleting data from the database: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data from the database");
            }
        }

        [HttpPost("approve/{id:int}")]
        public async Task<ActionResult> ApproveLeave(int id)
        {
            try
            {
                var leave = await leaveRepository.GetLeaveByIdAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                if (leave.Status != LeaveStatus.Pending)
                {
                    return BadRequest("Only pending leaves can be approved");
                }
                leave.Status = LeaveStatus.Approved;
                await leaveRepository.UpdateLeaveAsync(leave);

                //mta3 il notification ken mech tzidha mba3ed
                // await notificationService.NotifyLeaveApproval(leave);

                return NoContent();
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error approving leave");
            }
        }

        [HttpPost("reject/{id:int}")]
        public async Task<ActionResult> RejectLeave(int id)
        {
            try
            {
                var leave = await leaveRepository.GetLeaveByIdAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                if (leave.Status != LeaveStatus.Pending)
                {
                    return BadRequest("Only pending leaves can be rejected");
                }
                leave.Status = LeaveStatus.Rejected;
                await leaveRepository.UpdateLeaveAsync(leave);

                //mta3 il notification ken mech tzidha mba3ed
                // await notificationService.NotifyLeaveApproval(leave);

                return NoContent();
            }
            catch (Exception )
            {               
                return StatusCode(StatusCodes.Status500InternalServerError, "Error rejecting leave");
            }
        }
    }
}

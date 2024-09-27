using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagementAPI.Models;

namespace LeaveManagementAPI.Models.Repositories
{
    public interface ILeaveRepository
    {
        Task<Leave> AddLeaveAsync(Leave leave);
        Task<bool> DeleteLeaveAsync(int id);
        Task<IEnumerable<Leave>> GetAllLeavesAsync();
        Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task<Leave> GetLeaveByIdAsync(int id);
        Task<Leave> UpdateLeaveAsync(Leave leave);
        
    }
}

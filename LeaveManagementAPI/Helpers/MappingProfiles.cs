using AutoMapper;
using LeaveManagementAPI.Dtos;
using LeaveManagementAPI.Models;
using LeaveManagementAPI.Models.Dtos;

namespace LeaveManagementAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<Admin, AdminDTO>();

            CreateMap<Leave, LeaveDTO>().ReverseMap();

            CreateMap<UserCreateDTO, User>();
            CreateMap<EmployeeCreateDTO, Employee>();
            CreateMap<AdminCreateDTO, Admin>();

            CreateMap<UserUpdateDTO, User>();

            CreateMap<UpdateAdminDto, Admin>()
                .ForMember(dest => dest.Password, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Password)));
        }
    }
}

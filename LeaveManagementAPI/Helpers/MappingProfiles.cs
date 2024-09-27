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

            CreateMap<Leave, LeaveDTO>().ReverseMap(); // hedhi bech ma ta3melch   CreateMap<LeaveDTO, Leave>() hehehe
            CreateMap<Leave, LeaveDTO>();
            CreateMap<UserCreateDTO, User>();
            CreateMap<EmployeeCreateDTO, Employee>();
            CreateMap<AdminCreateDTO, Admin>();

            CreateMap<UserUpdateDTO, User>();

            CreateMap<UpdateAdminDto, Admin>()
           .ForMember(dest => dest.Password, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Password)));
        }
    }
}

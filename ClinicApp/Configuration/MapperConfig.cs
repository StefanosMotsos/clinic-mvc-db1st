using AutoMapper;
using ClinicApp.DTO;
using ClinicApp.Models;

namespace ClinicApp.Configuration
{
    public class MapperConfig : Profile
    {
        
        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<DoctorSignupDTO, User>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId!.Value));

            CreateMap<DoctorSignupDTO, Doctor>();
        }
    }
}

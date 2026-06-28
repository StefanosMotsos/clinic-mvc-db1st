using AutoMapper;
using ClinicApp.DTO;
using ClinicApp.Models;

namespace ClinicApp.Configuration
{
    public class MapperConfig : Profile
    {
        
        public MapperConfig()
        {
            CreateMap<DoctorSignupDTO, User>();
            CreateMap<DoctorSignupDTO, Doctor>();
            CreateMap<DoctorUpdateDTO, User>();
            CreateMap<DoctorUpdateDTO, Doctor>();

            CreateMap<Doctor, DoctorReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.User.Role.Name));

            CreateMap<DoctorReadOnlyDTO, DoctorUpdateDTO>();

            CreateMap<PatientSignupDTO, User>();
            CreateMap<PatientSignupDTO, Patient>();
            CreateMap<PatientUpdateDTO, User>();
            CreateMap<PatientUpdateDTO, Patient>();

            CreateMap<Patient, PatientReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.User.Role.Name));


            CreateMap<StaffSignupDTO, User>();
            CreateMap<UserUpdateDTO, User>();

            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<MedicalProgramCreateDTO, MedicalProgram>();
            CreateMap<MedicalProgramUpdateDTO, MedicalProgram>();

            CreateMap<MedicalProgram, MedicalProgramReadOnlyDTO>()
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.Firstname + " " + src.Doctor.User.Lastname))
            .ForMember(dest => dest.PatientNames, opt => opt.MapFrom(src => src.Patients.Select(p => p.User.Firstname + " " + p.User.Lastname)
            .ToList()));
        }
    }
}

using ClinicApp.Services.DoctorService;
using ClinicApp.Services.PatientService;
using ClinicApp.Services.UserService.UserService;

namespace ClinicApp.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        IDoctorService DoctorService { get; }
        IPatientService PatientService { get; }
    }
}

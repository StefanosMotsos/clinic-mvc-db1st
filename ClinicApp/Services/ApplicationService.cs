using ClinicApp.Services.DoctorService;
using ClinicApp.Services.PatientService;
using ClinicApp.Services.UserService.UserService;

namespace ClinicApp.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }

        public IDoctorService DoctorService { get; }

        public IPatientService PatientService { get; }

        public ApplicationService(IUserService userService, IDoctorService doctorService, IPatientService patientService)
        {
            UserService = userService;
            DoctorService = doctorService;
            PatientService = patientService;
        }
    }
}

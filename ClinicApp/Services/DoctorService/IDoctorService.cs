using ClinicApp.DTO;

namespace ClinicApp.Services.DoctorService
{
    public interface IDoctorService
    {
        Task SignUpUserAsync(DoctorSignupDTO request);
    }
}

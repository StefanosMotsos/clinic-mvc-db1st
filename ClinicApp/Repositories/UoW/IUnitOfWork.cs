using ClinicApp.Repositories.DoctorRepo;
using ClinicApp.Repositories.MedicalProgramRepo;
using ClinicApp.Repositories.PatientRepo;
using ClinicApp.Repositories.UserRepo;

namespace ClinicApp.Repositories.UoW
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPatientRepository PatientRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IMedicalProgramRepository MedicalProgramRepository { get; }

        Task<bool> SaveAsync();
    }
}

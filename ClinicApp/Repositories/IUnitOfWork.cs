using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using ClinicApp.Repositories.DoctorRepo;
using ClinicApp.Repositories.MedicalProgramRepo;
using ClinicApp.Repositories.PatientRepo;
using ClinicApp.Repositories.UserRepo;

namespace ClinicApp.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPatientRepository PatientRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IMedicalProgramRepository MedicalProgramRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }
        IBaseRepository<Capability> CapabilityRepository { get; }

        Task<bool> SaveAsync();
    }
}

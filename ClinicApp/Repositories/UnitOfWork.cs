using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Repositories.Base;
using ClinicApp.Repositories.DoctorRepo;
using ClinicApp.Repositories.MedicalProgramRepo;
using ClinicApp.Repositories.PatientRepo;
using ClinicApp.Repositories.UserRepo;

namespace ClinicApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicMvcdbfirstContext _context;
        public IUserRepository UserRepository { get; }
        public IPatientRepository PatientRepository { get; }
        public IDoctorRepository DoctorRepository { get; }
        public IMedicalProgramRepository MedicalProgramRepository { get; }
        public IBaseRepository<Role> RoleRepository { get; }
        public IBaseRepository<Capability> CapabilityRepository { get; }

        public UnitOfWork(ClinicMvcdbfirstContext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            DoctorRepository = new DoctorRepository(context);
            PatientRepository = new PatientRepository(context);
            MedicalProgramRepository = new MedicalProgramRepository(context);
            RoleRepository = new BaseRepository<Role>(context);
            CapabilityRepository = new BaseRepository<Capability>(context);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

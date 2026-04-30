using AutoMapper;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Models;
using ClinicApp.Repositories.UoW;
using ClinicApp.Security;

namespace ClinicApp.Services.DoctorService
{
    public class DoctorService : IDoctorService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper,  IEncryptionUtil encryptionUtil, ILogger<DoctorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _encryptionUtil = encryptionUtil;
            _logger = logger;
        }
        public async Task SignUpUserAsync(DoctorSignupDTO request)
        {
            Doctor doctor = _mapper.Map<Doctor>(request);
            User user = _mapper.Map<User>(request);

            try
            {
                User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    throw new EntityAlreadyExistsException("User", "User with username " + existingUser.Username + " already exists");
                }

                user.Doctor = doctor;
                user.Password = _encryptionUtil.Encrypt(user.Password);
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.DoctorRepository.AddAsync(doctor);

                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Doctor {Doctor} signed up successfully.", doctor);
            } catch(EntityAlreadyExistsException e)
            {
                _logger.LogError("Error signing up doctor {Doctor}. {Message}", doctor, e.Message);
                throw;
            }
        }
    }
}

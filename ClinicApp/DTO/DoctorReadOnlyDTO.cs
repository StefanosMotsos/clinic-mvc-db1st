namespace ClinicApp.DTO
{
    public record DoctorReadOnlyDTO
    {
        public Guid Uuid { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? RoleName { get; set; }
        public string? Specialty { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}

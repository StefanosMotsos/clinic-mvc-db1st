namespace ClinicApp.DTO
{
    public record PatientReadOnlyDTO
    {
        public Guid Uuid { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? RoleName { get; set; }
        public string? Amka { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? BloodType { get; set; }
        public bool IsDeleted { get; set; }
    }
}

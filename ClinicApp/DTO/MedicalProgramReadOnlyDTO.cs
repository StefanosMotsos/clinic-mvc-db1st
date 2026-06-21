namespace ClinicApp.DTO
{
    public record MedicalProgramReadOnlyDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DoctorName { get; set; }
        public List<string> PatientNames { get; set; } = [];
    }
}

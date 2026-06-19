namespace ClinicApp.Models
{
    public interface BaseEntity
    {
        DateTime InsertedAt { get; set; }
        DateTime ModifiedAt { get; set; }
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid Uuid { get; set; }
    }
}

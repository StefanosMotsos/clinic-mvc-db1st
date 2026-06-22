using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Doctor : BaseEntity
{
    public int Id { get; set; }

    public string Specialty { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid Uuid { get; set; }

    public virtual ICollection<MedicalProgram> MedicalPrograms { get; set; } = new List<MedicalProgram>();

    public virtual User User { get; set; } = null!;
}

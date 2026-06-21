using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Patient
{
    public int Id { get; set; }

    public string Amka { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string? BloodType { get; set; }

    public int UserId { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid Uuid { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<MedicalProgram> Programs { get; set; } = new List<MedicalProgram>();
}

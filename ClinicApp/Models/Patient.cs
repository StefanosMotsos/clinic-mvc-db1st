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

    public virtual User User { get; set; } = null!;

    public virtual ICollection<MedicalProgram> Programs { get; set; } = new List<MedicalProgram>();
}

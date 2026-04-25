using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Doctor
{
    public int Id { get; set; }

    public string Specialty { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<MedicalProgram> MedicalPrograms { get; set; } = new List<MedicalProgram>();

    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class MedicalProgram
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public int DoctorId { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}

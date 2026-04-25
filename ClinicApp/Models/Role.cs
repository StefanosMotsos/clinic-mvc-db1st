using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Capability> Capabilities { get; set; } = new List<Capability>();
}

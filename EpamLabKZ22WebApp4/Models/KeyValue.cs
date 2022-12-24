using System;
using System.Collections.Generic;

namespace EpamLabKZ22WebApp4.Models;

public partial class KeyValue
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Info { get; set; }
}

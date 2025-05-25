using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class Currency
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Coo> Coos { get; set; } = new List<Coo>();
}

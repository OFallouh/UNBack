using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class CooPo
{
    public int Id { get; set; }

    public int? CooId { get; set; }

    public string? PoNum { get; set; }

    public virtual Coo? Coo { get; set; }
}

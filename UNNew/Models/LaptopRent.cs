using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class LaptopRent
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public int Price { get; set; }

    public int LaptopType { get; set; }

    public DateTime? CreatedAt { get; set; }
    public int? ClientId { get; set; }
    public virtual Client? client { get; set; }

    public virtual LaptopType LaptopTypeNavigation { get; set; } = null!;
}

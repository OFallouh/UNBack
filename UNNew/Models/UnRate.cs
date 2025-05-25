using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class UnRate
{
    public int Id { get; set; }

    public int? YearNum { get; set; }
    public int? MonthNum { get; set; }
    public int? UnRate1 { get; set; }
    public int? ClientId { get; set; }
    public virtual Client? client { get; set; }
    public DateTime? CreatedAt { get; set; } 
}

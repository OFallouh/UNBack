using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class PurchaseOrder
{
    public int OrderId { get; set; }

    public string PoNo { get; set; } = null!;

    public double? PoAmount { get; set; }

    public int Cooid { get; set; }

    public virtual Coo Coo { get; set; } = null!;

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

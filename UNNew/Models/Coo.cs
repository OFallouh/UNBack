using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class Coo
{
    public int CooId { get; set; }

    public string? CooNumber { get; set; }

    public DateTime? CooDate { get; set; }

    public double? TotalValue { get; set; }

    public int? ClientId { get; set; }

    public int? CurrencyId { get; set; }
    public string? FolderPath { get; set; }
    public virtual Client? Client { get; set; }

    public virtual ICollection<CooPo> CooPos { get; set; } = new List<CooPo>();

    public virtual Currency? Currency { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<LifeInsurance> LifeInsurances { get; set; } = new List<LifeInsurance>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

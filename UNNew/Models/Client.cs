using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string? ClientName { get; set; }
    public string? Format { get; set; }

    public virtual ICollection<Coo> Coos { get; set; } = new List<Coo>();

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<SalaryTran> SalaryTrans { get; set; } = new List<SalaryTran>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
    public virtual ICollection<UnTransportCompensation> UnTransportCompensations { get; set; } = new List<UnTransportCompensation>();
    public virtual ICollection<UnMobileCompensation> UnMobileCompensations { get; set; } = new List<UnMobileCompensation>();
    public virtual ICollection<UnMonthLeave> UnMonthLeaves { get; set; } = new List<UnMonthLeave>();
    public virtual ICollection<LaptopRent> LaptopRents { get; set; } = new List<LaptopRent>();
}

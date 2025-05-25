using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string? TeamName { get; set; }

    public int? ClientId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<SalaryTran> SalaryTrans { get; set; } = new List<SalaryTran>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

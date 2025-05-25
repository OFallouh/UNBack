using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class TypeOfContract
{
    public int TypeOfContractId { get; set; }

    public string? NmaeEn { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

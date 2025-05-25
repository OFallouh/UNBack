using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class City
{
    public int CityId { get; set; }

    public string? NameAr { get; set; }

    public string? NameEn { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

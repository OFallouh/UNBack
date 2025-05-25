using System;
using System.Collections.Generic;

namespace UNNew.Models;
public enum Activite
{
    ICI,
    Employee,
    NoNeedForLaptop,

}
public partial class LaptopType
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public Activite? Activite { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual ICollection<LaptopRent> LaptopRents { get; set; } = new List<LaptopRent>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}

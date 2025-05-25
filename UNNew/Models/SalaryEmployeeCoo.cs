using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class SalaryEmployeeCoo
{
    public int Id { get; set; }

    public int? SalaryId { get; set; }

    public int? EmployeeCooId { get; set; }

    public virtual EmployeeCoo? EmployeeCoo { get; set; }

    public virtual SalaryTran? Salary { get; set; }
}

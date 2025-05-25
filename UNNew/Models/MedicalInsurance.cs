using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class MedicalInsurance
{
    public int Id { get; set; }

    public int? EmpCooId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? Active { get; set; }

    public virtual EmployeeCoo? EmpCoo { get; set; }
}

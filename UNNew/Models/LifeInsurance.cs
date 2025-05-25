using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class LifeInsurance
{
    public int Id { get; set; }

    public int? EmpId { get; set; }

    public int? CooId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Coo? Coo { get; set; }

    public virtual UnEmp? Emp { get; set; }
}

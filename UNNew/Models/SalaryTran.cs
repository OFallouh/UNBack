using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class SalaryTran
{
    public int TransId { get; set; }

    public DateTime? TransDate { get; set; }

    public int? SalaryUsd { get; set; }

    public int? UnRate { get; set; }

    public int? AnuallLeave { get; set; }

    public int? SickLeave { get; set; }

    public int? DaysOff { get; set; }

    public int? NetSalary { get; set; }

    public int? Transportation { get; set; }

    public int? Dsa { get; set; }

    public int? Mobile { get; set; }

    public int? Ammount { get; set; }

    public int? RefNo { get; set; }

    public int? DownPayment { get; set; }

    public int? DaysOn { get; set; }

    public bool? Status { get; set; }

    public double? OverTimeWages { get; set; }

    public int? TeamId { get; set; }

    public int? ClientId { get; set; }

    public int? SlaryMonth { get; set; }

    public int? SlaryYear { get; set; }

    public int? Laptop { get; set; }

    public DateOnly? TimeSheet { get; set; }

    public int? LaptopRent { get; set; }


    public double? Bonuses { get; set; }

    public double? Deductions { get; set; }
    public int? ContractId { get; set; }

    public virtual EmployeeCoo? EmployeeCoo { get; set; }
    public virtual Client? Client { get; set; }

    public virtual UnEmp? RefNoNavigation { get; set; }

    public virtual ICollection<SalaryEmployeeCoo> SalaryEmployeeCoos { get; set; } = new List<SalaryEmployeeCoo>();

    public virtual Team? Team { get; set; }
}

using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class EmployeeCoo
{
    public int Id { get; set; }

    public int? EmpId { get; set; }

    public int? CooId { get; set; }

    public int? TeamId { get; set; }

    public DateOnly? StartCont { get; set; }

    public DateOnly? EndCont { get; set; }

    public bool? Transportation { get; set; }

    public int? Mobile { get; set; }

    public int? Salary { get; set; }

    public string? Tittle { get; set; }

    public int? ClientId { get; set; }

    public bool? ContractSigned { get; set; }

    public int? CityId { get; set; }

    public int? TypeOfContractId { get; set; }

    public bool? InsuranceLife { get; set; }

    public bool? InsuranceMedical { get; set; }

    public string? SuperVisor { get; set; }

    public string? AreaManager { get; set; }

    public int? OrderId { get; set; }

    public bool? IsMobile { get; set; }

    public bool? IsSendCard { get; set; }

    public bool? IsSendContra { get; set; }

    public string? Note { get; set; }

    public DateOnly? SendInsuranceDate { get; set; }

    public int? Laptop { get; set; }

    public DateOnly? StartLifeDate { get; set; }

    public DateOnly? EndLifeDate { get; set; }

    public bool? IsSendMedical { get; set; }

    public string? ProjectName { get; set; }

    public string? CooPoId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsCancelled { get; set; } 

    public virtual City? City { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Coo? Coo { get; set; }

    public virtual UnEmp? Emp { get; set; }

    public virtual LaptopType? LaptopNavigation { get; set; }

    public virtual ICollection<MedicalInsurance> MedicalInsurances { get; set; } = new List<MedicalInsurance>();

    public virtual PurchaseOrder? Order { get; set; }

    public virtual ICollection<SalaryEmployeeCoo> SalaryEmployeeCoos { get; set; } = new List<SalaryEmployeeCoo>();
    public virtual ICollection<SalaryTran> SalaryTrans { get; set; } = new List<SalaryTran>();

    public virtual Team? Team { get; set; }

    public virtual TypeOfContract? TypeOfContract { get; set; }
}

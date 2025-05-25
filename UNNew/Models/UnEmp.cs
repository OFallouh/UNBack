using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class UnEmp
{
    public int RefNo { get; set; }

    public int? UnNo { get; set; }

    public string? EmpName { get; set; }

    public string? ArabicName { get; set; }

    public string? IdNo { get; set; }

    public string? SuperVisor { get; set; }

    public string? AreaManager { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailAddress { get; set; }

    public bool? ContractSigned { get; set; }

    public string? CooNo { get; set; }

    public DateTime? ContractStartDate { get; set; }

    public DateTime? ContractEndDate { get; set; }

    public string? City { get; set; }

    public int? Salary { get; set; }

    public bool? Transportation { get; set; }

    public string? Tittle { get; set; }

    public string? TypeOfContract { get; set; }

    public bool? InsuranceLife { get; set; }

    public bool? InsuranceMedical { get; set; }

    public bool? MedicalCheck { get; set; }

    public bool? OldEmployment { get; set; }

    public bool? SecurityCheck { get; set; }

    public string? BankName { get; set; }

    public string? TypeOfAcc { get; set; }

    public string? AccountNumber { get; set; }

    public string? ContractCopy { get; set; }

    public string? IdCopy { get; set; }

    public string? SecurityDoc { get; set; }

    public string? InsuranceDoc { get; set; }

    public string? TimeSheet { get; set; }

    public DateTime? CreationDate { get; set; }

    /// <summary>
    /// List
    /// </summary>
    public string? Team { get; set; }

    public byte[] SsmaTimeStamp { get; set; } = null!;

    public string? MotherNameArabic { get; set; }

    public string? FatherNameArabic { get; set; }

    public int? TeamId { get; set; }

    public int? CityId { get; set; }

    public int? TypeOfContractId { get; set; }

    public int? CooId { get; set; }

    public double? TotalMonth { get; set; }

    public int? ClientId { get; set; }

    public int? BankId { get; set; }

    public string? CooNo1 { get; set; }

    public bool FlagContract { get; set; }

    public bool Active { get; set; }

    public int? OrderId { get; set; }

    public string? FolderPath { get; set; }

    public bool? IsMobile { get; set; }

    public string? Gender { get; set; }

    public bool? IsDeleted { get; set; }

    public int? Laptop { get; set; }

    public DateOnly? StartLifeDate { get; set; }

    public DateOnly? EndLifeDate { get; set; }

    public string? ProjectName { get; set; }

    public string? CooPoId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsDelegated { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual City? CityNavigation { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Coo? Coo { get; set; }

    public bool? InsuranceCardDelivered { get; set; }

    public DateOnly? InsuranceCardDeliveredDate { get; set; }

    public virtual ICollection<EmployeeCoo> EmployeeCoos { get; set; } = new List<EmployeeCoo>();

    public virtual LaptopType? LaptopNavigation { get; set; }

    public virtual ICollection<LifeInsurance> LifeInsurances { get; set; } = new List<LifeInsurance>();

    public virtual PurchaseOrder? Order { get; set; }

    public virtual ICollection<SalaryTran> SalaryTrans { get; set; } = new List<SalaryTran>();

    public virtual Team? TeamNavigation { get; set; }

    public virtual TypeOfContract? TypeOfContractNavigation { get; set; }
    public virtual ICollection<UN_Employee_Login> EmployeeLogins { get; set; }
}

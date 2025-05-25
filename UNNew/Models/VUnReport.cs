using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class VUnReport
{
    public int RefNo { get; set; }

    public string? EnglishName { get; set; }

    public string? ArabicName { get; set; }

    public string? IdNo { get; set; }

    public int? MobileNo { get; set; }

    public string? EmailAddress { get; set; }

    public string? CooNumber { get; set; }

    public DateTime? LastStartContra { get; set; }

    public DateTime? LastEndContra { get; set; }

    public string? City { get; set; }

    public bool? Transportation { get; set; }

    public string? BankName { get; set; }

    public string? TeamName { get; set; }

    public string? Laptop { get; set; }

    public string? MotherNameArabic { get; set; }

    public string? FatherNameArabic { get; set; }

    public bool FlagContract { get; set; }

    public string HaveFolde { get; set; } = null!;

    public bool? IsMobile { get; set; }

    public string? Gender { get; set; }

    public string? AccountNumber { get; set; }

    public bool? InsuranceLife { get; set; }

    public bool? InsuranceMedical { get; set; }

    public bool? IsSendCard { get; set; }

    public bool? IsSendContra { get; set; }

    public string? Note { get; set; }

    public DateOnly? StartCont { get; set; }

    public DateOnly? EndCont { get; set; }

    public DateOnly? SendInsuranceDate { get; set; }

    public DateOnly? StartLifeDate { get; set; }

    public DateOnly? EndLifeDate { get; set; }

    public string? Tittle { get; set; }

    public int? Salary { get; set; }

    public string? ProjectName { get; set; }

    public string? CooPoId { get; set; }

    public string Dob { get; set; } = null!;
}

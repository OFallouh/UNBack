using System;
using System.ComponentModel.DataAnnotations;
public enum GenderType
{
    Male,
    Female,
}
public class CreateUNEmployeeDto
{
    [Required(ErrorMessage = "Personal information is required.")]
    public AddPersonal personal { get; set; }


    [Required(ErrorMessage = "Bank information is required.")]
    public AddBankInfo bankInfo { get; set; }

    //[Required(ErrorMessage = "Contract information is required.")]
    //public AddContractInfo contractInfo { get; set; }

    public class AddPersonal
    {
        [Required(ErrorMessage = "Employee name is required.")]

        public string EmpName { get; set; }

        [Required(ErrorMessage = "Arabic name  is required.")]

        public string ArabicName { get; set; }
        [Required(ErrorMessage = "Mother's Arabic name  is required.")]

        public string MotherNameArabic { get; set; }
        [Required(ErrorMessage = "Father's Arabic name  is required.")]
        public string FatherNameArabic { get; set; }

        [Required(ErrorMessage = "ID number is required.")]
        public string IdNo { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public GenderType? Gender { get; set; }

        public bool? OldEmployment { get; set; }

        public bool? SecurityCheck { get; set; }
    }

    //public class AddBackgroundcheck
    //{
    //    [Required(ErrorMessage = "Medical check status is required.")]
    //    public bool MedicalCheck { get; set; } 

    //    [Required(ErrorMessage = "Old employment status is required.")]
    //    public bool OldEmployment { get; set; }

    //    [Required(ErrorMessage = "Security check status is required.")]
    //    public bool SecurityCheck { get; set; }
    //}

    public class AddBankInfo
    {
        [Required(ErrorMessage = "Bank ID is required.")]
        public int? BankId { get; set; }
        public string? TypeOfAcc { get; set; }

        public string? AccountNumber { get; set; }

        public bool IsDelegated { get; set; }
    }

    //public class AddContractInfo
    //{
    //    [Required(ErrorMessage = "Client ID is required.")]
    //    public int? ClientId { get; set; }

    //    [Required(ErrorMessage = "Team ID is required.")]
    //    public int? TeamId { get; set; }

    //    [Required(ErrorMessage = "COO ID is required.")]
    //    public int? CooId { get; set; }

    //    public string? CooPoId { get; set; }

    //    [Required(ErrorMessage = "Contract signed status is required.")]
    //    public bool? ContractSigned { get; set; }

    //    [Required(ErrorMessage = "Contract start date is required.")]
    //    public DateTime? ContractStartDate { get; set; }

    //    [Required(ErrorMessage = "Contract end date is required.")]
    //    public DateTime? ContractEndDate { get; set; }

    //    [Required(ErrorMessage = "City ID is required.")]
    //    public int? CityId { get; set; }

    //    [Required(ErrorMessage = "Title is required.")]
    //    public string? Tittle { get; set; }

    //    [Required(ErrorMessage = "Salary is required.")]
    //    [Range(0, int.MaxValue, ErrorMessage = "Salary must be a positive number.")]
    //    public int? Salary { get; set; }

    //    [Required(ErrorMessage = "Transportation status is required.")]
    //    public bool? Transportation { get; set; }

    //    [Required(ErrorMessage = "Laptop information is required.")]
    //    public int? Laptop { get; set; }

    //    [Required(ErrorMessage = "Mobile status is required.")]
    //    public bool? IsMobile { get; set; }

    //    [Required(ErrorMessage = "Type of contract ID is required.")]
    //    public int? TypeOfContractId { get; set; }

    //    [Required(ErrorMessage = "Insurance life status is required.")]
    //    public bool? InsuranceLife { get; set; }
    //    public DateTime? StartLifeDate { get; set; }

    //    public DateTime? EndLifeDate { get; set; }
    //    [Required(ErrorMessage = "Insurance Medical is required.")]
    //    public bool? InsuranceMedical { get; set; }

    //    public string? SuperVisor { get; set; }

    //    public string? AreaManager { get; set; }

    //    public string? ProjectName { get; set; }
    //    public bool Active { get; set; } = true;
    //}
}
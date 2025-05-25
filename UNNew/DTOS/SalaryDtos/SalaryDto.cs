namespace UNNew.DTOS.SalaryDtos
{
    public class SalaryDto
    {

        public int SalaryId { get; set; }
        public int ContractId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ArabicName { get; set; }
        public string? CooNumber { get; set; }
        public string? PoNumber { get; set; }

        public string? TeamName { get; set; }

        public string? ClientName { get; set; }

        public int? BasicSalaryinUSD { get; set; }

        public int? TotalSalaryCalculatedinSyrianPounds { get; set; }

        public int? SlaryMonth { get; set; }

        public int? SlaryYear { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? FatherNameArabic { get; set; }
        public bool IsDelegated { get; set; }
    }
}

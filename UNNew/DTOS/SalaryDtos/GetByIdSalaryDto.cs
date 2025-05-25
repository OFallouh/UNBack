namespace UNNew.DTOS.SalaryDtos
{
    public class GetByIdSalaryDto
    {
        public string? CooNumber { get; set; }
        public string? PoNumber { get; set; }

        public int? SickLeave { get; set; }

        public int? DaysOff { get; set; }

        public int? Transportation { get; set; }

        public int? Mobile { get; set; }

        public int? DownPayment { get; set; }

        public double? OverTimeWages { get; set; }

        public string? EmployeeName { get; set; }

        public string? TeamName { get; set; }

        public string? ClientName { get; set; }

        public int? BasicSalaryinUSD { get; set; }

        public int? TotalSalaryCalculatedinSyrianPounds { get; set; }

        public double? Bonuses { get; set; }

        public int? NetSalary { get; set; }

        public double? Deductions { get; set; }

        public int? Laptop { get; set; }

        public DateOnly? TimeSheet { get; set; }

        public int? DaysOn { get; set; }

        public int? Dsa { get; set; }

        public int? SlaryMonth { get; set; }

        public int? SlaryYear { get; set; }
        public int? LaptopRent { get; set; }
    }
}

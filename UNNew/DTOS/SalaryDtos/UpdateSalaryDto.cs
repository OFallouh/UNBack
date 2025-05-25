namespace UNNew.DTOS.SalaryDtos
{
    public class UpdateSalaryDto
    {
        public int EmployeeId { get; set; }
        public int ContractId { get; set; }
        public int? SickLeave { get; set; }
        public int? DaysOff { get; set; }
        public int? DownPayment { get; set; }
        public double? OverTimeWages { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
}

namespace UNNew.DTOS.InsuranceDtos
{
    public class InsuranceEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string? EmpName { get; set; }
        public string? ArabicName { get; set; }
        public bool? InsuranceLife { get; set; }
        public bool? InsuranceMedical { get; set; }
        public DateOnly? StartLifeDate { get; set; }
        public DateOnly? EndLifeDate { get; set; }
        public DateOnly? StartMedicalDate { get; set; }
        public DateOnly? EndMedicalDate { get; set; }
        public string? Stauts { get; set; }
        public bool? InsuranceCardDelivered { get; set; }
        public DateOnly? InsuranceCardDeliveredDate { get; set; }
        public int? DaysRemainingLife { get; set; }  // Days remaining for InsuranceLife
        public int? DaysRemainingMedical { get; set; }  // Days remaining for InsuranceMedical
    }
}

using System.ComponentModel.DataAnnotations;

namespace UNNew.DTOS.InsuranceDtos
{
    public class UpdateInsuranceDto
    {
        public int EmployeeId { get; set; }
        public bool? InsuranceLife { get; set; }
        public bool? InsuranceMedical { get; set; }
        public DateOnly? StartLifeDate { get; set; }
        public DateOnly? EndLifeDate { get; set; }
        public bool? InsuranceCardDelivered { get; set; }
        public DateOnly? InsuranceCardDeliveredDate { get; set; }
    }
}

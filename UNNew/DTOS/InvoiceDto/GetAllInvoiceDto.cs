using System.Collections.Generic;

namespace UNNew.DTOS.InvoiceDto
{
    public class GetAllInvoiceDto
    {
        public List<Coos>? Coos { get; set; }
    }

    public class Coos
    {
        public string? CooNumber { get; set; }
        public DateTime? CooDate { get; set; }
        public List<Employess>? Employess { get; set; }
    }

    public class Employess
    {
        public int EmployeeId { get; set; }
        public int ContractId { get; set; }
        public int? SalaryId { get; set; }

        public string? EmpName { get; set; }
        public string? ArabicName { get; set; }
        public int SalaryUsd { get; set; }
        public int PayableSalaryUsd { get; set; }
        public int PayableSalarySYP { get; set; }
        public int Transportion { get; set; }
        public int Mobile { get; set; }
        public int Laptop { get; set; }
        public int LaptopRent { get; set; }

        public Employess()
        {
            SalaryUsd = 0;
            PayableSalaryUsd = 0;
            PayableSalarySYP = 0;
            Transportion = 0;
            Mobile = 0;
            Laptop = 0;
        }
    }

}

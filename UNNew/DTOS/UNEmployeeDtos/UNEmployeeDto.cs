using System.ComponentModel.DataAnnotations;

namespace UNNew.DTOS.UNEmployeeDtos
{
    public class UNEmployeeDto
    {
  
            public int RefNo { get; set; }
            public string? cooNumber { get; set; }
            public string? PoNumber { get; set; }
            public string? EmpName { get; set; }

            public string? ArabicName { get; set; }

            public string? MotherNameArabic { get; set; }

            public string? FatherNameArabic { get; set; }

            public string? IdNo { get; set; }

            public string? EmailAddress { get; set; }

            public string? MobileNo { get; set; }

            public string? Gender { get; set; }
           
            public string? BankName { get; set; }

            public string? TypeOfAcc { get; set; }

            public string? AccountNumber { get; set; }

            public bool Active { get; set; }
            public bool? OldEmployment { get; set; }

            public bool? SecurityCheck { get; set; }
            public bool IsDelegated { get; set; }


    }
}




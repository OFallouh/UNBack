using System.ComponentModel.DataAnnotations;

namespace UNNew.DTOS.UNEmployeeDtos
{
    public class DisplyUNEmployeeDto
    {
        [Required(ErrorMessage = "Personal information is required.")]
        public DisplyPersonal personal { get; set; }

        [Required(ErrorMessage = "Bank information is required.")]
        public DisplyBankInfo bankInfo { get; set; }


        public class DisplyPersonal
        {
            public int RefNo { get; set; }

            public string? EmpName { get; set; }

            public string? ArabicName { get; set; }

            public string? MotherNameArabic { get; set; }

            public string? FatherNameArabic { get; set; }

            public string? IdNo { get; set; }

            public string? EmailAddress { get; set; }

            public string? MobileNo { get; set; }

            public bool? OldEmployment { get; set; }

            public bool? SecurityCheck { get; set; }
            public int? Gender { get; set; }
        }
 
        public class DisplyBankInfo
        {

            public int? BankId { get; set; }

            public string? TypeOfAcc { get; set; }

            public string? AccountNumber { get; set; }
            public bool IsDelegated { get; set; }
        }


    }
}

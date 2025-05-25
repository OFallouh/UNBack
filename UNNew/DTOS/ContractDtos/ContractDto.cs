namespace UNNew.DTOS.ContractDtos
{
    public class ContractDto
    {
        public int Id { get; set; }

        public string? EmployeeName { get; set; }

        public string? ArabicName { get; set; }

        public string? ClientName { get; set; }


        public string? TeamName { get; set; }


        public string? CooNumber { get; set; }


        public DateOnly? ContractStartDate { get; set; }


        public DateOnly? ContractEndDate { get; set; }


        public string? CityName { get; set; }

        public string? Tittle { get; set; }


        public int? Salary { get; set; }


        public bool? Transportation { get; set; }


        public string? LaptopType { get; set; }


        public bool? IsMobile { get; set; }


        public string? TypeOfContract { get; set; }

        public bool? InsuranceLife { get; set; }
        public DateOnly? StartLifeDate { get; set; }

        public DateOnly? EndLifeDate { get; set; }

        public bool? InsuranceMedical { get; set; }

        public string? SuperVisor { get; set; }

        public string? AreaManager { get; set; }

        public string? ProjectName { get; set; }

        public string? ContractDuration { get; set; }

        public string Status { get; set; }
    }
}

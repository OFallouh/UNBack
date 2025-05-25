namespace UNNew.DTOS.ContractDtos
{
    public class GetByIdContractDto
    {
        public int Id { get; set; }

        public string? EmployeeName { get; set; }

        public string? ArabicName { get; set; }

        public int? ClientId{ get; set; }

        public int? TeamId { get; set; }

        public int? CooId { get; set; }

        public string? CooNumber { get; set; }

        public DateOnly? ContractStartDate { get; set; }


        public DateOnly? ContractEndDate { get; set; }


        public int? CityId { get; set; }

        public string? Tittle { get; set; }


        public int? Salary { get; set; }


        public bool? Transportation { get; set; }


        public int? LaptopTypeId { get; set; }


        public bool? IsMobile { get; set; }


        public int? TypeOfContractId { get; set; }


        public string? SuperVisor { get; set; }

        public string? AreaManager { get; set; }

        public string? ProjectName { get; set; }

        public bool? InsuranceLife { get; set; }

        public bool? InsuranceMedical { get; set; }


    }
}

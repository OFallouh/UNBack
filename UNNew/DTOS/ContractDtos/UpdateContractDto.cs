using System.ComponentModel.DataAnnotations;

namespace UNNew.DTOS.ContractDtos
{
    public class UpdateContractDto
    {

        [Required(ErrorMessage = "Team ID is required.")]
        public int? TeamId { get; set; }

        [Required(ErrorMessage = "COO ID is required.")]
        public int? CooId { get; set; }
  
        [Required(ErrorMessage = "Contract start date is required.")]
        public DateTime? ContractStartDate { get; set; }

        [Required(ErrorMessage = "Contract end date is required.")]
        public DateTime? ContractEndDate { get; set; }

        [Required(ErrorMessage = "City ID is required.")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string? Tittle { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public int? Salary { get; set; }

        [Required(ErrorMessage = "Transportation status is required.")]
        public bool? Transportation { get; set; }

        [Required(ErrorMessage = "Laptop information is required.")]
        public int? LaptopTypeId { get; set; }

        [Required(ErrorMessage = "Mobile status is required.")]
        public bool? IsMobile { get; set; }

        [Required(ErrorMessage = "Type of contract ID is required.")]
        public int? TypeOfContractId { get; set; }

        [Required(ErrorMessage = "Insurance life status is required.")]

        public string? SuperVisor { get; set; }

        public string? AreaManager { get; set; }

        public string? ProjectName { get; set; }
        public bool? InsuranceLife { get; set; }

        public bool? InsuranceMedical { get; set; }

    }
}

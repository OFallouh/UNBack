namespace UNNew.DTOS.UnLaptopCompensationDto
{
    public class GetAllLaptopCompensationDto
    {
        public int Id { get; set; }
        public int? YearNum { get; set; }
        public int? MonthNum { get; set; }
        public int? MobileCompensation { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ClientName { get; set; }
    }
}

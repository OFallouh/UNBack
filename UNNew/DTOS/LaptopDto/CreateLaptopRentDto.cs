namespace UNNew.DTOS.LaptopDto
{
    public class CreateLaptopRentDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Price { get; set; }
        public int LaptopType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ClientId { get; set; } 
    }
}

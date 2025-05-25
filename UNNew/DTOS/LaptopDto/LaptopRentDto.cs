namespace UNNew.DTOS.LaptopDto
{
    public class LaptopRentDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string LaptopTypeName { get; set; } 
    }
}

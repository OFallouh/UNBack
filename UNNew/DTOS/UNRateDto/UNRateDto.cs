namespace UNNew.DTOS.UNRateDto
{
    public class UNRateDto
    {
        public int Id { get; set; }

        public int? YearNum { get; set; }

        public int? MonthNum { get; set; }

        public int? ExchangeRate { get; set; }

        public string? ClientName { get; set; }
        public DateTime? CreatedAt { get; set; } 
    }
}

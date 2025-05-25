namespace UNNew.DTOS.UNRateDto
{
    public class CreateUNRateDto
    {
        public int? YearNum { get; set; }

        public int? MonthNum { get; set; }

        public int? ExchangeRate { get; set; }

        public int? ClientId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

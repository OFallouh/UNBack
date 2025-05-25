namespace UNNew.DTOS.TransportCompensationDto
{
    public class AddTransportCompensationDto
    {
        public int? YearNum { get; set; }

        public int? MonthNum { get; set; }

        public int? TransportCompensation { get; set; }

        public int? ClientId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

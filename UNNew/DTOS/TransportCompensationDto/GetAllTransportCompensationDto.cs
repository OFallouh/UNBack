namespace UNNew.DTOS.TransportCompensationDto
{
    public class GetAllTransportCompensationDto
    {
        public int Id { get; set; }
        public int? YearNum { get; set; }

        public int? MonthNum { get; set; }

        public int? TransportCompensation { get; set; }

        public string? ClientName { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

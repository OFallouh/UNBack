namespace UNNew.DTOS.CooDtos
{
    public class UpdateCooDto
    {
        public string CooNumber { get; set; }
        public DateTime? CooDate { get; set; }
        public double? TotalValue { get; set; }
        public int ClientId { get; set; }
        public int CurrencyId { get; set; }
        public string? PoNumber { get; set; }
    }
}

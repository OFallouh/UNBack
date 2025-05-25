namespace UNNew.DTOS.CooDtos
{
    public class GetByIdDto
    {
        public int CooId { get; set; }
        public string? CooNumber { get; set; }
        public DateTime? CooDate { get; set; }
        public double? TotalValue { get; set; }
        public int? ClientId { get; set; }
        public int? CurrencyTypeId { get; set; }
        public string? PoNumber { get; set; }
    }
}

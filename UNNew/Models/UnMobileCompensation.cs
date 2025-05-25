namespace UNNew.Models
{
    public class UnMobileCompensation
    {
        public int Id { get; set; }

        public int? YearNum { get; set; }
        public int? MonthNum { get; set; }
        public int? UnMobileCompensation1 { get; set; }
        public int? ClientId { get; set; }
        public virtual Client? client { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

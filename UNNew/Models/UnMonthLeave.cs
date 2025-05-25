namespace UNNew.Models
{
    public class UnMonthLeave
    {
        public int Id { get; set; }

        public int? YearNum { get; set; }
        public int? MonthNum { get; set; }
        public double? UnMonthLeave1 { get; set; }
        public int? ClientId { get; set; }
        public virtual Client? Client { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

namespace UNNew.DTOS.UnMonthLeaveDto
{
    public class GetAllUnMonthLeaveDto
    {
        public int Id { get; set; }
        public int? YearNum { get; set; }
        public int? MonthNum { get; set; }
        public double? UnMonthLeave1 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ClientName { get; set; }
    }
}

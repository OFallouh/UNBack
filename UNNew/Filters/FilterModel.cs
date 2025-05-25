namespace UNNew.Filters
{
    public class FilterModel
    {
        public int First { get; set; }
        public int Rows { get; set; }
        public int? SortOrder { get; set; }
        public Dictionary<string, FilterCriteria>? Filters { get; set; }
        public string? GlobalFilter { get; set; }
        public List<SortMeta>? MultiSortMeta { get; set; }
    }

    public class FilterCriteria
    {
        public dynamic? Value { get; set; }
        public string? MatchMode { get; set; }
    }

    public class SortMeta
    {
        public string? Field { get; set; }
        public int? Order { get; set; }
    }
}


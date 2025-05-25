namespace UNNew.Response
{
    public class ApiResponse<T>
    {
        public bool Success => Errors == null || Errors.Count == 0;
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public int Count { get; set; }

        public ApiResponse(string message, T? data = default, List<string>? errors = null, int count = 0)
        {
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
            Count = count;
        }
    }


}

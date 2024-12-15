namespace ChatApi.DTOs
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }
    }
}
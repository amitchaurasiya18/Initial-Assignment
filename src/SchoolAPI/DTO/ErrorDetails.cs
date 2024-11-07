namespace SchoolAPI.DTO
{
    public class ErrorDetails
    {
        // public Guid TraceId { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        // public PathString Instance { get; set; }
        public string ExceptionMessage { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
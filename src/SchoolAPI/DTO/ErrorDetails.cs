namespace SchoolAPI.DTO
{
    public class ErrorDetails
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string? ExceptionMessage { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}
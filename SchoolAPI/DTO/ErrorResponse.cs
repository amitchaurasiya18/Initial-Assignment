namespace SchoolAPI.DTO
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorMessage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.DTO
{
    public class ValidationErrorDetails
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string? ExceptionMessage { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}
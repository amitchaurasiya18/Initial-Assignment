using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.DTO
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
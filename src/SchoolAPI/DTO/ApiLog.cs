using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAPI.DTO
{
    public class ApiLog
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
    }
}
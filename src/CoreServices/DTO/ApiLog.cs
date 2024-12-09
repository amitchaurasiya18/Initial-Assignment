using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.DTO
{
    [ExcludeFromCodeCoverage]
    public class ApiLog
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
    }
}
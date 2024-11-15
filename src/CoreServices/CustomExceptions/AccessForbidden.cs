using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreServices.CustomExceptions
{
    public class AccessForbidden : Exception , ICustomException
    {
        public int StatusCode { get; set; }
        public AccessForbidden(string message) : base(message) {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}
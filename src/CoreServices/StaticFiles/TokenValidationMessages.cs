using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.StaticFiles
{
    public static class TokenValidationMessages
    {
        public const string TOKEN_EXPIRED = "Token has expired";
        public const string INVALID_SIGNATURE = "Token has an invalid signature";
        public const string INVALID_FORMAT = "Token has an invalid format";
        public const string TOKEN_VALIDATION_FAILED =  "Token Validation Failed";
        public const string ACCESS_FORBIDDEN = "Access Denied";
        public const string INTERNAL_SERVER_ERROR = "Internal Server Error";
    }
}
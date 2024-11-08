using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.CustomExceptions
{
    public class InvalidCredentials : Exception
    {
        public InvalidCredentials(string message) : base(message) {}
    }
}
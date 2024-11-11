using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAPI.CustomExceptions
{
    public class InvalidPageNumber : Exception
    {
        public InvalidPageNumber(string message) : base(message){}
    }
}
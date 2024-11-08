using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAPI.CustomExceptions
{
    public class NoStudentsFound : Exception
    {
        public NoStudentsFound(string message) : base(message) {}
    }
}
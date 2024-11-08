using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAPI.CustomExceptions
{
    public class StudentNotFound : Exception
    {
        public StudentNotFound(string message) : base(message) {}  
    }
}
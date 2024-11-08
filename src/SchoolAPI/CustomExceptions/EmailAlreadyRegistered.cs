using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAPI.CustomExceptions
{
    public class EmailAlreadyRegistered : Exception
    {
        public EmailAlreadyRegistered(string message) : base(message){}
    }
}
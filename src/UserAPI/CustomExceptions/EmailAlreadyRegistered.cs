using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.CustomExceptions
{
    public class EmailAlreadyRegistered : Exception
    {
        public EmailAlreadyRegistered(string email) : base(email) {}
    }
}
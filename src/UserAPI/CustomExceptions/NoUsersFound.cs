using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.CustomExceptions
{
    public class NoUsersFound : Exception
    {
        public NoUsersFound(string message) : base(message) {}
    }
}
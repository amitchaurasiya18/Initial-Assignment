using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.CustomExceptions
{
    public class EmptyCredentials : Exception
    {
        public EmptyCredentials(string message): base(message) {    }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.CustomExceptions
{
    public interface ICustomException
    {
        int StatusCode { get; set; }
    }
}
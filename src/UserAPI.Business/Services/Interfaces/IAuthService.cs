using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.DTO;
using UserAPI.Business.Models;

namespace UserAPI.Business.Services.Interfaces
{
    public interface IAuthService
    {
        string Login(User user);
    }
}
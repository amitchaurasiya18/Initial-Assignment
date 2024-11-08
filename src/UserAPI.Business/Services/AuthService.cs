using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserAPI.Business.Data;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services.Interfaces;

namespace UserAPI.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserAPIDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(UserAPIDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Login(LoginRequest loginRequest)
        {
            // var user = _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
            return "hello";
        }
    }
}
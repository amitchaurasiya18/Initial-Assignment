using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using CoreServices.DTO;
using CoreServices.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Business.Data;
using UserAPI.Business.Models;
using UserAPI.Business.Services.Interfaces;

namespace UserAPI.Business.Services
{
    public class AuthService : IAuthService
    {
        // private readonly UserAPIDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            // _context = context;
            _configuration = configuration;
        }

        public string Login(User user)
        {
            // User? user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

            var claims = new List<Claim>
            {
                new Claim("Username", user.Username ?? "Anonymous"),
                new Claim("UserId", user.Id.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim("Role", AuthorizationRoles.ADMIN));
            }

            if (!user.IsAdmin)
            {
                claims.Add(new Claim("Role", AuthorizationRoles.TEACHER));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
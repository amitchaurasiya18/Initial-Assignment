using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Business.Models;
using UserAPI.Business.Services.Interfaces;

namespace UserAPI.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private const string ADMIN = "Admin";
        private const string TEACHER = "Teacher";

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Login(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", user.Username ?? "Anonymous"),
                new Claim("UserId", user.Id.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim("Role", ADMIN));
            }

            if (!user.IsAdmin)
            {
                claims.Add(new Claim("Role", TEACHER));
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
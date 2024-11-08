using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services.Interfaces;
using UserAPI.CustomExceptions;
using UserAPI.StaticFiles;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UserLoginController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest loginRequest)
        {
            if(loginRequest == null)
            {
                throw new EmptyCredentials(ErrorMessages.EMPTY_CREDENTIALS);
            }

            var user = await _userRepository.GetByUsername(loginRequest.Username);

            if(user == null)
            {
                throw new InvalidCredentials(ErrorMessages.INVALID_CREDENTIALS);
            }

            if(!BCrypt.Net.BCrypt.EnhancedVerify(loginRequest.Password,user.Password))
            {
                throw new InvalidCredentials(ErrorMessages.INVALID_CREDENTIALS);
            }

            var token = _authService.Login(loginRequest);
            return Ok(token);
        }
    }
}